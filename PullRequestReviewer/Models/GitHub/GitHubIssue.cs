using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Represents an issue in a GitHub repository.
/// </summary>
public sealed class GitHubIssue
{
    /// <summary>
    /// Gets the URL of the issue.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the repository URL.
    /// </summary>
    [JsonPropertyName("repository_url")]
    public string? RepositoryUrl { get; init; }

    /// <summary>
    /// Gets the labels URL.
    /// </summary>
    [JsonPropertyName("labels_url")]
    public string? LabelsUrl { get; init; }

    /// <summary>
    /// Gets the comments URL.
    /// </summary>
    [JsonPropertyName("comments_url")]
    public string? CommentsUrl { get; init; }

    /// <summary>
    /// Gets the events URL.
    /// </summary>
    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; init; }

    /// <summary>
    /// Gets the HTML URL of the issue.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; init; }

    /// <summary>
    /// Gets the unique identifier of the issue.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the node identifier.
    /// </summary>
    [JsonPropertyName("node_id")]
    public string? NodeId { get; init; }

    /// <summary>
    /// Gets the issue number.
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; init; }

    /// <summary>
    /// Gets the title of the issue.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets a value indicating whether the issue is locked.
    /// </summary>
    [JsonPropertyName("locked")]
    public bool Locked { get; init; }

    /// <summary>
    /// Gets the reason for active lock.
    /// </summary>
    [JsonPropertyName("active_lock_reason")]
    public string? ActiveLockReason { get; init; }

    /// <summary>
    /// Gets the list of assignees.
    /// </summary>
    [JsonPropertyName("assignees")]
    public List<GitHubUser>? Assignees { get; init; }

    /// <summary>
    /// Gets the user who created the issue.
    /// </summary>
    [JsonPropertyName("user")]
    public GitHubUser? User { get; init; }

    /// <summary>
    /// Gets the list of labels associated with the issue.
    /// </summary>
    [JsonPropertyName("labels")]
    public List<GitHubLabel>? Labels { get; init; }

    /// <summary>
    /// Gets the state of the issue.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the reason for the state.
    /// </summary>
    [JsonPropertyName("state_reason")]
    public string? StateReason { get; init; }

    /// <summary>
    /// Gets the assignee.
    /// </summary>
    [JsonPropertyName("assignee")]
    public GitHubUser? Assignee { get; init; }

    /// <summary>
    /// Gets the milestone.
    /// </summary>
    [JsonPropertyName("milestone")]
    public GitHubMilestone? Milestone { get; init; }

    /// <summary>
    /// Gets the number of comments.
    /// </summary>
    [JsonPropertyName("comments")]
    public int Comments { get; init; }

    /// <summary>
    /// Gets the creation date and time.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the last update date and time.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the closure date and time.
    /// </summary>
    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; init; }

    /// <summary>
    /// Gets the body of the issue.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; init; }

    /// <summary>
    /// Gets the score.
    /// </summary>
    [JsonPropertyName("score")]
    public double? Score { get; init; }

    /// <summary>
    /// Gets the author association.
    /// </summary>
    [JsonPropertyName("author_association")]
    public string? AuthorAssociation { get; init; }

    /// <summary>
    /// Gets a value indicating whether the issue is a draft.
    /// </summary>
    [JsonPropertyName("draft")]
    public bool? Draft { get; init; }

    /// <summary>
    /// Gets the related pull request reference, if any.
    /// </summary>
    [JsonPropertyName("pull_request")]
    public GitHubPullRequestReference? PullRequest { get; init; }

    /// <summary>
    /// Gets the repository.
    /// </summary>
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
