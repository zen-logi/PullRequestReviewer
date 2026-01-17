using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// GraphQL API レスポンスのラッパー。
/// </summary>
public sealed class GraphQLResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("errors")]
    public List<GraphQLError>? Errors { get; init; }
}

/// <summary>
/// GraphQL エラー。
/// </summary>
public sealed class GraphQLError
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// PR 検索クエリのレスポンス。
/// </summary>
public sealed class SearchPullRequestsData
{
    [JsonPropertyName("search")]
    public SearchResult? Search { get; init; }

    [JsonPropertyName("viewer")]
    public Viewer? Viewer { get; init; }
}

/// <summary>
/// 検索結果。
/// </summary>
public sealed class SearchResult
{
    [JsonPropertyName("issueCount")]
    public int IssueCount { get; init; }

    [JsonPropertyName("nodes")]
    public List<GraphQLPullRequest>? Nodes { get; init; }
}

/// <summary>
/// 現在のユーザー情報。
/// </summary>
public sealed class Viewer
{
    [JsonPropertyName("login")]
    public string Login { get; init; } = string.Empty;
}

/// <summary>
/// GraphQL API から取得したPR情報。
/// </summary>
public sealed class GraphQLPullRequest
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("number")]
    public int Number { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    [JsonPropertyName("isDraft")]
    public bool IsDraft { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; init; }

    [JsonPropertyName("author")]
    public GraphQLAuthor? Author { get; init; }

    [JsonPropertyName("repository")]
    public GraphQLRepository? Repository { get; init; }

    [JsonPropertyName("reviews")]
    public GraphQLReviewConnection? Reviews { get; init; }

    [JsonPropertyName("reviewRequests")]
    public GraphQLReviewRequestConnection? ReviewRequests { get; init; }
}

/// <summary>
/// PR 作成者。
/// </summary>
public sealed class GraphQLAuthor
{
    [JsonPropertyName("login")]
    public string Login { get; init; } = string.Empty;

    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; init; }
}

/// <summary>
/// リポジトリ情報。
/// </summary>
public sealed class GraphQLRepository
{
    [JsonPropertyName("nameWithOwner")]
    public string NameWithOwner { get; init; } = string.Empty;
}

/// <summary>
/// レビューのコネクション。
/// </summary>
public sealed class GraphQLReviewConnection
{
    [JsonPropertyName("nodes")]
    public List<GraphQLReview>? Nodes { get; init; }
}

/// <summary>
/// レビュー情報。
/// </summary>
public sealed class GraphQLReview
{
    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    [JsonPropertyName("author")]
    public GraphQLAuthor? Author { get; init; }
}

/// <summary>
/// レビューリクエストのコネクション。
/// </summary>
public sealed class GraphQLReviewRequestConnection
{
    [JsonPropertyName("nodes")]
    public List<GraphQLReviewRequest>? Nodes { get; init; }
}

/// <summary>
/// レビューリクエスト情報。
/// </summary>
public sealed class GraphQLReviewRequest
{
    [JsonPropertyName("requestedReviewer")]
    public GraphQLAuthor? RequestedReviewer { get; init; }
}

/// <summary>
/// GraphQL GetAllPullRequestsAsync 用のレスポンス型。
/// </summary>
public sealed class GraphQLAllPullRequestsData
{
    [JsonPropertyName("reviewRequested")]
    public SearchResult? ReviewRequested { get; init; }

    [JsonPropertyName("assigned")]
    public SearchResult? Assigned { get; init; }

    [JsonPropertyName("authored")]
    public SearchResult? Authored { get; init; }
}