using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PullRequestReviewer.Models;
using PullRequestReviewer.Models.GitHub;
using PullRequestReviewer.Models.Options;

namespace PullRequestReviewer.Services;

/// <inheritdoc cref="IGitHubService"/>
public class GitHubService(
    ILogger<GitHubService> logger,
    HttpClient httpClient,
    IOptions<GitHubOptions> gitHubOptions) : IGitHubService
{
    private string? currentUsername;
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc/>
    public void SetToken(string token)
    {
        logger.LogInformation("Setting GitHub token");
        httpClient.DefaultRequestHeaders.Clear();

        // OAuth トークン (gho_) と PAT (ghp_) の両方で Bearer スキームを使用
        var scheme = "Bearer";
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(gitHubOptions.Value.UserAgent);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        currentUsername = null;
        logger.LogDebug("GitHub HTTP client initialized with scheme: {Scheme}, base URL: {BaseUrl}", scheme, gitHubOptions.Value.ApiBaseUrl);
    }

    /// <inheritdoc />
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            logger.LogDebug("Validating GitHub token");

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{gitHubOptions.Value.ApiBaseUrl}/user");
            // トークン検証用にリクエストを作成
            request.Headers.Authorization = new AuthenticationHeaderValue("token", token);
            request.Headers.UserAgent.ParseAdd(gitHubOptions.Value.UserAgent);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Token validation successful");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetReviewRequestedPullRequestsAsync()
    {
        logger.LogDebug("Fetching review-requested pull requests");
        EnsureHttpClientInitialized();

        var username = await GetCurrentUsernameAsync();
        var query = $"review-requested:{username} is:pr is:open";
        var searchUrl = $"{gitHubOptions.Value.ApiBaseUrl}/search/issues?q={Uri.EscapeDataString(query)}&per_page=100";

        logger.LogDebug("Search query: {Query}", query);
        var response = await httpClient.GetAsync(searchUrl);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("GitHub API returned {StatusCode}: {Content}", response.StatusCode, errorContent);
            throw new InvalidOperationException($"GitHub API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResponse<GitHubIssue>>(jsonOptions);
        if (result == null)
        {
            logger.LogWarning("Received null response from GitHub API");
            return [];
        }

        logger.LogInformation("Found {Count} review-requested pull requests", result.TotalCount);
        return await ConvertToPullRequestModelsAsync(result.Items);
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetAssignedPullRequestsAsync()
    {
        logger.LogDebug("Fetching assigned pull requests");
        EnsureHttpClientInitialized();

        var username = await GetCurrentUsernameAsync();
        var query = $"assignee:{username} is:pr is:open";
        var searchUrl = $"{gitHubOptions.Value.ApiBaseUrl}/search/issues?q={Uri.EscapeDataString(query)}&per_page=100";

        logger.LogDebug("Search query: {Query}", query);
        var response = await httpClient.GetAsync(searchUrl);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("GitHub API returned {StatusCode}: {Content}", response.StatusCode, errorContent);
            throw new InvalidOperationException($"GitHub API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResponse<GitHubIssue>>(jsonOptions);
        if (result == null)
        {
            logger.LogWarning("Received null response from GitHub API");
            return [];
        }

        logger.LogInformation("Found {Count} assigned pull requests", result.TotalCount);
        return await ConvertToPullRequestModelsAsync(result.Items);
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetAuthoredPullRequestsAsync()
    {
        logger.LogDebug("Fetching authored pull requests");
        EnsureHttpClientInitialized();

        var username = await GetCurrentUsernameAsync();
        var query = $"author:{username} is:pr is:open";
        var searchUrl = $"{gitHubOptions.Value.ApiBaseUrl}/search/issues?q={Uri.EscapeDataString(query)}&per_page=100";

        logger.LogDebug("Search query: {Query}", query);
        var response = await httpClient.GetAsync(searchUrl);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("GitHub API returned {StatusCode}: {Content}", response.StatusCode, errorContent);
            throw new InvalidOperationException($"GitHub API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResponse<GitHubIssue>>(jsonOptions);
        if (result == null)
        {
            logger.LogWarning("Received null response from GitHub API");
            return [];
        }

        logger.LogInformation("Found {Count} authored pull requests", result.TotalCount);
        return await ConvertToPullRequestModelsAsync(result.Items);
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetAllPullRequestsAsync()
    {
        logger.LogInformation("Fetching all pull requests (review-requested, assigned, and authored)");
        var reviewRequested = await GetReviewRequestedPullRequestsAsync();
        var assigned = await GetAssignedPullRequestsAsync();
        var authored = await GetAuthoredPullRequestsAsync();

        var allPrs = reviewRequested
            .Concat(assigned)
            .Concat(authored)
            .GroupBy(pr => pr.Id)
            .Select(g => g.First())
            .OrderByDescending(pr => pr.UpdatedAt ?? pr.CreatedAt)
            .ToList();

        logger.LogInformation("Total unique pull requests: {Count}", allPrs.Count);
        return allPrs;
    }

    private async Task<string> GetCurrentUsernameAsync()
    {
        if (!string.IsNullOrWhiteSpace(currentUsername))
        {
            logger.LogDebug("Using cached username: {Username}", currentUsername);
            return currentUsername;
        }

        logger.LogDebug("Fetching current user information");
        var response = await httpClient.GetAsync($"{gitHubOptions.Value.ApiBaseUrl}/user");
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<GitHubUser>(jsonOptions);
        if (user == null || string.IsNullOrWhiteSpace(user.Login))
        {
            throw new InvalidOperationException("Failed to fetch current user information or username is empty");
        }

        currentUsername = user.Login;
        logger.LogInformation("Current user: {Username}", currentUsername);
        return currentUsername;
    }

    private async Task<List<PullRequestModel>> ConvertToPullRequestModelsAsync(List<GitHubIssue> issues)
    {
        logger.LogDebug("Converting {Count} issues to PR models", issues.Count);

        // Use SemaphoreSlim to limit concurrency (e.g., 5 simultaneous requests)
        using var semaphore = new SemaphoreSlim(5);

        var tasks = issues.Select(async issue =>
        {
            await semaphore.WaitAsync();
            try
            {
                if (issue.PullRequest == null)
                {
                    logger.LogDebug("Skipping issue #{Number} - not a pull request", issue.Number);
                    return null;
                }

                // Extract repository info
                string owner, repoName;
                try
                {
                    if (issue.Repository != null && !string.IsNullOrEmpty(issue.Repository.FullName))
                    {
                        var repoParts = issue.Repository.FullName.Split('/');
                        if (repoParts.Length != 2) throw new FormatException("Invalid FullName format");
                        owner = repoParts[0];
                        repoName = repoParts[1];
                    }
                    else
                    {
                        // Parse from HTML URL or API URL using Uri class for robustness
                        var targetUrl = !string.IsNullOrEmpty(issue.RepositoryUrl) ? issue.RepositoryUrl : issue.HtmlUrl;
                        if (string.IsNullOrEmpty(targetUrl)) throw new InvalidOperationException("No repository URL available");

                        var uri = new Uri(targetUrl);
                        var segments = uri.Segments.Where(s => s != "/").Select(s => s.TrimEnd('/')).ToArray();

                        // Expected format .../repos/owner/repo or .../owner/repo/...
                        if (targetUrl.Contains("/repos/"))
                        {
                            // api.github.com/repos/owner/repo
                            var repoIndex = Array.IndexOf(segments, "repos");
                            if (repoIndex >= 0 && repoIndex + 2 < segments.Length)
                            {
                                owner = segments[repoIndex + 1];
                                repoName = segments[repoIndex + 2];
                            }
                            else throw new FormatException("Invalid API URL format");
                        }
                        else
                        {
                            // github.com/owner/repo/pull/123
                            if (segments.Length >= 2)
                            {
                                owner = segments[0];
                                repoName = segments[1];
                            }
                            else throw new FormatException("Invalid HTML URL format");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to parse repository info for issue #{Number}", issue.Number);
                    return null;
                }

                var repoFullName = $"{owner}/{repoName}";

                GitHubPullRequest? pr = null;
                try
                {
                    var prUrl = $"{gitHubOptions.Value.ApiBaseUrl}/repos/{owner}/{repoName}/pulls/{issue.Number}";
                    var prResponse = await httpClient.GetAsync(prUrl);
                    prResponse.EnsureSuccessStatusCode();
                    pr = await prResponse.Content.ReadFromJsonAsync<GitHubPullRequest>(jsonOptions);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to fetch PR details for {Repo}#{Number}", repoFullName, issue.Number);
                    return null;
                }

                if (pr == null)
                {
                    logger.LogWarning("Received null PR data for {Repo}#{Number}", repoFullName, issue.Number);
                    return null;
                }

                var model = new PullRequestModel
                {
                    Id = issue.Id,
                    Number = issue.Number,
                    Title = issue.Title,
                    State = issue.State,
                    Body = issue.Body ?? string.Empty,
                    HtmlUrl = issue.HtmlUrl,
                    CreatedAt = issue.CreatedAt,
                    UpdatedAt = issue.UpdatedAt,
                    AuthorLogin = issue.User?.Login ?? "Unknown",
                    AuthorAvatarUrl = issue.User?.AvatarUrl,
                    RepositoryName = repoName,
                    RepositoryOwner = owner,
                    RepositoryFullName = repoFullName,
                    IsDraft = pr.Draft,
                    RequestedReviewers = pr.RequestedReviewers?.Select(r => r.Login).ToList() ?? [],
                    Assignees = issue.Assignees?.Select(a => a.Login).ToList() ?? []
                };

                // Fetch reviews
                try
                {
                    var reviewsUrl = $"{gitHubOptions.Value.ApiBaseUrl}/repos/{owner}/{repoName}/pulls/{issue.Number}/reviews";
                    var reviewsResponse = await httpClient.GetAsync(reviewsUrl);
                    reviewsResponse.EnsureSuccessStatusCode();
                    var reviews = await reviewsResponse.Content.ReadFromJsonAsync<List<GitHubReview>>(jsonOptions);

                    if (reviews != null && reviews.Count > 0)
                    {
                        // Calculate review status based on the latest review for each reviewer
                        var latestReviews = reviews
                            .GroupBy(r => r.User.Login)
                            .Select(g => g.OrderByDescending(r => r.SubmittedAt).First())
                            .ToList();

                        if (latestReviews.Any(r => r.State == "CHANGES_REQUESTED"))
                        {
                            model.ReviewStatus = "Changes Requested";
                        }
                        else if (latestReviews.Any(r => r.State == "APPROVED"))
                        {
                            model.ReviewStatus = "Approved";
                        }
                        else if (latestReviews.Any(r => r.State == "COMMENTED"))
                        {
                            model.ReviewStatus = "Commented";
                        }
                        else
                        {
                            model.ReviewStatus = "Review Requested";
                        }
                    }
                    else
                    {
                        model.ReviewStatus = "No Reviews";
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to fetch reviews for {Repo}#{Number}", repoFullName, issue.Number);
                    model.ReviewStatus = "Error";
                }

                logger.LogDebug("Converted PR: {Repo}#{Number} - {Title}", repoFullName, issue.Number, issue.Title);
                return model;
            }
            finally
            {
                semaphore.Release();
            }
        });

        var results = await Task.WhenAll(tasks);
        var models = results.Where(m => m != null).Cast<PullRequestModel>().ToList();

        logger.LogInformation("Successfully converted {Count} pull requests", models.Count);
        return models;
    }

    private void EnsureHttpClientInitialized()
    {
        if (httpClient.DefaultRequestHeaders.Authorization == null)
        {
            throw new InvalidOperationException("GitHub HTTP client is not initialized. Call SetToken first.");
        }
    }

    #region GraphQL API

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetAllPullRequestsGraphQlAsync()
    {
        logger.LogInformation("Fetching all pull requests via GraphQL API");
        EnsureHttpClientInitialized();

        var username = await GetCurrentUsernameAsync();

        var query = $@"
            query($reviewQuery: String!, $assignedQuery: String!, $authoredQuery: String!) {{
                reviewRequested: search(query: $reviewQuery, type: ISSUE, first: 50) {{
                    ...SearchFields
                }}
                assigned: search(query: $assignedQuery, type: ISSUE, first: 50) {{
                    ...SearchFields
                }}
                authored: search(query: $authoredQuery, type: ISSUE, first: 50) {{
                    ...SearchFields
                }}
            }}
            {SearchFieldsFragment}
        ";

        var variables = new Dictionary<string, string>
        {
            ["reviewQuery"] = $"review-requested:{username} is:pr is:open",
            ["assignedQuery"] = $"assignee:{username} is:pr is:open",
            ["authoredQuery"] = $"author:{username} is:pr is:open"
        };

        var response = await ExecuteGraphQLAsync<GraphQLAllPullRequestsData>(query, variables);

        if (response == null)
        {
            logger.LogWarning("Received null response from GraphQL API");
            return [];
        }

        var allPrs = new List<GraphQLPullRequest>();
        if (response.ReviewRequested?.Nodes != null) allPrs.AddRange(response.ReviewRequested.Nodes);
        if (response.Assigned?.Nodes != null) allPrs.AddRange(response.Assigned.Nodes);
        if (response.Authored?.Nodes != null) allPrs.AddRange(response.Authored.Nodes);

        var uniquePrs = allPrs
            .Where(pr => pr != null)
            .GroupBy(pr => pr.Id)
            .Select(g => g.First())
            .OrderByDescending(pr => pr.UpdatedAt ?? pr.CreatedAt)
            .ToList();

        logger.LogInformation("Found {Count} unique pull requests via GraphQL", uniquePrs.Count);

        return uniquePrs.Select(ConvertGraphQLToPullRequestModel).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetReviewRequestedPullRequestsGraphQlAsync()
    {
        logger.LogInformation("Fetching review-requested pull requests via GraphQL API");
        EnsureHttpClientInitialized();
        var username = await GetCurrentUsernameAsync();
        return await ExecuteSingleSearchQueryAsync($"review-requested:{username} is:pr is:open");
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetAssignedPullRequestsGraphQlAsync()
    {
        logger.LogInformation("Fetching assigned pull requests via GraphQL API");
        EnsureHttpClientInitialized();
        var username = await GetCurrentUsernameAsync();
        return await ExecuteSingleSearchQueryAsync($"assignee:{username} is:pr is:open");
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestModel>> GetAuthoredPullRequestsGraphQlAsync()
    {
        logger.LogInformation("Fetching authored pull requests via GraphQL API");
        EnsureHttpClientInitialized();
        var username = await GetCurrentUsernameAsync();
        return await ExecuteSingleSearchQueryAsync($"author:{username} is:pr is:open");
    }

    private async Task<List<PullRequestModel>> ExecuteSingleSearchQueryAsync(string searchQuery)
    {
        var query = $@"
            query($query: String!) {{
                search(query: $query, type: ISSUE, first: 50) {{
                    ...SearchFields
                }}
            }}
            {SearchFieldsFragment}
        ";

        var variables = new Dictionary<string, string> { ["query"] = searchQuery };

        // Single search returns SearchPullRequestsData structure (search -> nodes)
        var response = await ExecuteGraphQLAsync<SearchPullRequestsData>(query, variables);

        if (response?.Search?.Nodes == null)
        {
            logger.LogWarning("Received null response from GraphQL API for single search");
            return [];
        }

        return response.Search.Nodes
            .Select(ConvertGraphQLToPullRequestModel)
            .OrderByDescending(pr => pr.UpdatedAt ?? pr.CreatedAt)
            .ToList();
    }

    private const string SearchFieldsFragment = @"
        fragment SearchFields on SearchResultItemConnection {
            issueCount
            nodes {
                ... on PullRequest {
                    id
                    number
                    title
                    url
                    state
                    isDraft
                    createdAt
                    updatedAt
                    author { login avatarUrl }
                    repository { nameWithOwner }
                    reviews(first: 20, states: [APPROVED, CHANGES_REQUESTED, COMMENTED]) {
                        nodes { state author { login } }
                    }
                    reviewRequests(first: 10) {
                        nodes { requestedReviewer { ... on User { login } } }
                    }
                }
            }
        }
    ";

    /// <summary>
    /// 指定されたクエリと変数を使用して GraphQL API を実行する。
    /// </summary>
    /// <typeparam name="T">レスポンスデータの型。</typeparam>
    /// <param name="query">GraphQL クエリ文字列。</param>
    /// <param name="variables">クエリ変数。</param>
    /// <returns>API から返されたデータ。エラーが発生した場合は例外をスローする。</returns>
    private async Task<T?> ExecuteGraphQLAsync<T>(string query, Dictionary<string, string> variables) where T : class
    {
        var requestBody = new { query, variables };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(gitHubOptions.Value.GraphQLEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("GraphQL API returned {StatusCode}: {Content}", response.StatusCode, errorContent);
            throw new InvalidOperationException($"GraphQL API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<T>>(jsonOptions);

        if (result?.Errors?.Count > 0)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Message));
            logger.LogError("GraphQL errors: {Errors}", errorMessages);
            throw new InvalidOperationException($"GraphQL error: {errorMessages}");
        }

        return result?.Data;
    }

    /// <summary>
    /// GraphQL の PR データをアプリケーションのモデルに変換する。
    /// </summary>
    /// <param name="pr">GraphQL から取得した PR データ。</param>
    /// <returns>アプリケーションで使用する PullRequestModel。</returns>
    private PullRequestModel ConvertGraphQLToPullRequestModel(GraphQLPullRequest pr)
    {
        var reviewStatus = CalculateGraphQLReviewStatus(pr);

        return new PullRequestModel
        {
            Id = pr.Id.GetHashCode(),
            Number = pr.Number,
            Title = pr.Title,
            HtmlUrl = pr.Url,
            State = pr.IsDraft ? "draft" : pr.State.ToLowerInvariant(),
            IsDraft = pr.IsDraft,
            RepositoryFullName = pr.Repository?.NameWithOwner ?? "",
            AuthorLogin = pr.Author?.Login ?? "unknown",
            AuthorAvatarUrl = pr.Author?.AvatarUrl ?? "",
            CreatedAt = pr.CreatedAt,
            UpdatedAt = pr.UpdatedAt,
            ReviewStatus = reviewStatus,
            ReviewStatusColor = GetGraphQLReviewStatusColor(reviewStatus)
        };
    }

    /// <summary>
    /// PR のレビュー状態（承認、変更要求など）を計算する。
    /// </summary>
    /// <param name="pr">GraphQL から取得した PR データ。</param>
    /// <returns>レビュー状態を表す文字列。</returns>
    private static string CalculateGraphQLReviewStatus(GraphQLPullRequest pr)
    {
        var reviews = pr.Reviews?.Nodes ?? [];

        if (reviews.Count == 0)
        {
            var pendingReviewers = pr.ReviewRequests?.Nodes?.Count ?? 0;
            return pendingReviewers > 0 ? "Pending" : "No reviews";
        }

        var latestReviews = reviews
            .Where(r => r.Author != null)
            .GroupBy(r => r.Author!.Login)
            .Select(g => g.Last())
            .ToList();

        var approved = latestReviews.Count(r => r.State == "APPROVED");
        var changesRequested = latestReviews.Count(r => r.State == "CHANGES_REQUESTED");
        var commented = latestReviews.Count(r => r.State == "COMMENTED");

        if (changesRequested > 0)
            return "Changes requested";
        if (approved > 0)
            return "Approved";
        if (commented > 0)
            return "Commented";

        return "Pending";
    }

    /// <summary>
    /// レビュー状態に対応する表示色を取得する。
    /// </summary>
    /// <param name="status">レビュー状態。</param>
    /// <returns>16進数カラーコード。</returns>
    private static string GetGraphQLReviewStatusColor(string status)
    {
        if (status.StartsWith("Approved"))
            return "#28a745";
        if (status.StartsWith("Changes requested"))
            return "#d73a49";
        if (status.StartsWith("Commented"))
            return "#6f42c1";
        return "#6a737d";
    }

    #endregion
}