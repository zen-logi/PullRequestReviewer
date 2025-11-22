using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

public sealed class GitHubIssue
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("repository_url")]
    public string? RepositoryUrl { get; init; }

    [JsonPropertyName("labels_url")]
    public string? LabelsUrl { get; init; }

    [JsonPropertyName("comments_url")]
    public string? CommentsUrl { get; init; }

    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; init; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; init; }

    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; init; }

    [JsonPropertyName("number")]
    public int Number { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("locked")]
    public bool Locked { get; init; }

    [JsonPropertyName("active_lock_reason")]
    public string? ActiveLockReason { get; init; }

    [JsonPropertyName("assignees")]
    public List<GitHubUser>? Assignees { get; init; }

    [JsonPropertyName("user")]
    public GitHubUser? User { get; init; }

    [JsonPropertyName("labels")]
    public List<GitHubLabel>? Labels { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("state_reason")]
    public string? StateReason { get; init; }

    [JsonPropertyName("assignee")]
    public GitHubUser? Assignee { get; init; }

    [JsonPropertyName("milestone")]
    public GitHubMilestone? Milestone { get; init; }

    [JsonPropertyName("comments")]
    public int Comments { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; init; }

    [JsonPropertyName("body")]
    public string? Body { get; init; }

    [JsonPropertyName("score")]
    public double? Score { get; init; }

    [JsonPropertyName("author_association")]
    public string? AuthorAssociation { get; init; }

    [JsonPropertyName("draft")]
    public bool? Draft { get; init; }

    [JsonPropertyName("pull_request")]
    public GitHubPullRequestReference? PullRequest { get; init; }

    [JsonPropertyName("repository")]
    public GitHubRepository? Repository { get; init; }
}

public sealed class GitHubLabel
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("color")]
    public string? Color { get; init; }

    [JsonPropertyName("default")]
    public bool Default { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

public sealed class GitHubMilestone
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; init; }

    [JsonPropertyName("labels_url")]
    public string? LabelsUrl { get; init; }

    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; init; }

    [JsonPropertyName("number")]
    public int Number { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("creator")]
    public GitHubUser? Creator { get; init; }

    [JsonPropertyName("open_issues")]
    public int OpenIssues { get; init; }

    [JsonPropertyName("closed_issues")]
    public int ClosedIssues { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; init; }

    [JsonPropertyName("due_on")]
    public DateTime? DueOn { get; init; }
}

public sealed class GitHubPullRequestReference
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; init; }

    [JsonPropertyName("diff_url")]
    public string? DiffUrl { get; init; }

    [JsonPropertyName("patch_url")]
    public string? PatchUrl { get; init; }
}
