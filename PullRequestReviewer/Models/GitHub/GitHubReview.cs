using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Represents a review on a GitHub pull request.
/// </summary>
public sealed class GitHubReview
{
    /// <summary>
    /// Gets the state of the review (e.g., APPROVED, CHANGES_REQUESTED, COMMENTED).
    /// </summary>
    [JsonPropertyName("state")]
    public required string State { get; init; }

    /// <summary>
    /// Gets the date and time when the review was submitted.
    /// </summary>
    [JsonPropertyName("submitted_at")]
    public DateTime SubmittedAt { get; init; }

    /// <summary>
    /// Gets the user who submitted the review.
    /// </summary>
    [JsonPropertyName("user")]
    public required GitHubUser User { get; init; }
}
