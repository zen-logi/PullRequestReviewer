using System.Net.Http.Headers;
using System.Net.Http.Json;
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
    private string? _currentUsername;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc/>
    public void SetToken(string token)
    {
        logger.LogInformation("Setting GitHub token");
        httpClient.DefaultRequestHeaders.Clear();
        // Use "token" scheme for Personal Access Tokens (not "Bearer")
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(gitHubOptions.Value.UserAgent);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        _currentUsername = null;
        logger.LogDebug("GitHub HTTP client initialized with base URL: {BaseUrl}", gitHubOptions.Value.ApiBaseUrl);
    }

    /// <inheritdoc />
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            logger.LogDebug("Validating GitHub token");

            using var testClient = new HttpClient();
            // Use "token" scheme for Personal Access Tokens (not "Bearer")
            testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            testClient.DefaultRequestHeaders.UserAgent.ParseAdd(gitHubOptions.Value.UserAgent);
            testClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            testClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

            var response = await testClient.GetAsync($"{gitHubOptions.Value.ApiBaseUrl}/user");
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

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResponse<GitHubIssue>>(_jsonOptions);
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

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResponse<GitHubIssue>>(_jsonOptions);
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

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResponse<GitHubIssue>>(_jsonOptions);
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
        if (!string.IsNullOrWhiteSpace(_currentUsername))
        {
            logger.LogDebug("Using cached username: {Username}", _currentUsername);
            return _currentUsername;
        }

        logger.LogDebug("Fetching current user information");
        var response = await httpClient.GetAsync($"{gitHubOptions.Value.ApiBaseUrl}/user");
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<GitHubUser>(_jsonOptions);
        if (user == null || string.IsNullOrWhiteSpace(user.Login))
        {
            throw new InvalidOperationException("Failed to fetch current user information or username is empty");
        }

        _currentUsername = user.Login;
        logger.LogInformation("Current user: {Username}", _currentUsername);
        return _currentUsername;
    }

    private async Task<List<PullRequestModel>> ConvertToPullRequestModelsAsync(List<GitHubIssue> issues)
    {
        logger.LogDebug("Converting {Count} issues to PR models", issues.Count);
        var models = new List<PullRequestModel>();

        foreach (var issue in issues)
        {
            if (issue.PullRequest == null)
            {
                logger.LogDebug("Skipping issue #{Number} - not a pull request", issue.Number);
                continue;
            }

            // Extract repository info from repository_url or html_url
            string owner, repoName;
            if (issue.Repository != null && !string.IsNullOrEmpty(issue.Repository.FullName))
            {
                var repoParts = issue.Repository.FullName.Split('/');
                if (repoParts.Length != 2)
                {
                    logger.LogWarning("Invalid repository full name: {FullName}", issue.Repository.FullName);
                    continue;
                }
                owner = repoParts[0];
                repoName = repoParts[1];
            }
            else if (!string.IsNullOrEmpty(issue.RepositoryUrl))
            {
                // Parse from repository_url: https://api.github.com/repos/owner/repo
                var urlParts = issue.RepositoryUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (urlParts.Length < 2)
                {
                    logger.LogWarning("Invalid repository URL: {Url}", issue.RepositoryUrl);
                    continue;
                }
                owner = urlParts[^2];
                repoName = urlParts[^1];
            }
            else if (!string.IsNullOrEmpty(issue.HtmlUrl))
            {
                // Parse from html_url: https://github.com/owner/repo/pull/123
                var urlParts = issue.HtmlUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (urlParts.Length < 4)
                {
                    logger.LogWarning("Invalid HTML URL: {Url}", issue.HtmlUrl);
                    continue;
                }
                owner = urlParts[^4];
                repoName = urlParts[^3];
            }
            else
            {
                logger.LogWarning("Cannot determine repository for issue #{Number}", issue.Number);
                continue;
            }

            var repoFullName = $"{owner}/{repoName}";

            GitHubPullRequest? pr = null;
            try
            {
                var prUrl = $"{gitHubOptions.Value.ApiBaseUrl}/repos/{owner}/{repoName}/pulls/{issue.Number}";
                var prResponse = await httpClient.GetAsync(prUrl);
                prResponse.EnsureSuccessStatusCode();
                pr = await prResponse.Content.ReadFromJsonAsync<GitHubPullRequest>(_jsonOptions);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch PR details for {Repo}#{Number}", repoFullName, issue.Number);
                continue;
            }

            if (pr == null)
            {
                logger.LogWarning("Received null PR data for {Repo}#{Number}", repoFullName, issue.Number);
                continue;
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

            models.Add(model);
            logger.LogDebug("Converted PR: {Repo}#{Number} - {Title}", repoFullName, issue.Number, issue.Title);
        }

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
}
