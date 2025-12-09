using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Represents a GitHub pull request detail.
/// </summary>
public sealed class GitHubPullRequest
{
    /// <summary>
    /// Gets the unique identifier of the pull request.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the pull request number.
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; init; }

    /// <summary>
    /// Gets the title of the pull request.
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// Gets the state of the pull request.
    /// </summary>
    [JsonPropertyName("state")]
    public required string State { get; init; }

    /// <summary>
    /// Gets the body/description of the pull request.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; init; }

    /// <summary>
    /// Gets the HTML URL of the pull request.
    /// </summary>
    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; init; }

    /// <summary>
    /// Gets the creation date and time of the pull request.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the last update date and time of the pull request.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the user who created the pull request.
    /// </summary>
    [JsonPropertyName("user")]
    public required GitHubUser User { get; init; }

    /// <summary>
    /// Gets a value indicating whether the pull request is a draft.
    /// </summary>
    [JsonPropertyName("draft")]
    public bool Draft { get; init; }

    /// <summary>
    /// Gets the list of users requested for review.
    /// </summary>
    [JsonPropertyName("requested_reviewers")]
    public List<GitHubUser>? RequestedReviewers { get; init; }

    /// <summary>
    /// Gets the list of assignees.
    /// </summary>
    [JsonPropertyName("assignees")]
    public List<GitHubUser>? Assignees { get; init; }
}
