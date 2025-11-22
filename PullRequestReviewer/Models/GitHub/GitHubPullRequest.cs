using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

public sealed class GitHubPullRequest
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("number")]
    public int Number { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("state")]
    public required string State { get; init; }

    [JsonPropertyName("body")]
    public string? Body { get; init; }

    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    [JsonPropertyName("user")]
    public required GitHubUser User { get; init; }

    [JsonPropertyName("draft")]
    public bool Draft { get; init; }

    [JsonPropertyName("requested_reviewers")]
    public List<GitHubUser>? RequestedReviewers { get; init; }

    [JsonPropertyName("assignees")]
    public List<GitHubUser>? Assignees { get; init; }
}
