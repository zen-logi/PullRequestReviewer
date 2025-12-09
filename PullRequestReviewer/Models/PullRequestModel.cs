namespace PullRequestReviewer.Models;

/// <summary>
/// Represents a GitHub Pull Request with additional UI-specific properties.
/// </summary>
public class PullRequestModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the pull request.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the pull request number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the title of the pull request.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the state of the pull request (e.g., open, closed).
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the body/description of the pull request.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML URL of the pull request.
    /// </summary>
    public string? HtmlUrl { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the pull request.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date and time of the pull request.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the username of the author.
    /// </summary>
    public string AuthorLogin { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the avatar URL of the author.
    /// </summary>
    public string? AuthorAvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the name of the repository (e.g., "RepoName").
    /// </summary>
    public string RepositoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner of the repository.
    /// </summary>
    public string RepositoryOwner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the repository (e.g., "Owner/RepoName").
    /// </summary>
    public string RepositoryFullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the pull request is a draft.
    /// </summary>
    public bool IsDraft { get; set; }

    /// <summary>
    /// Gets or sets the list of users requested for review.
    /// </summary>
    public List<string> RequestedReviewers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the list of assignees.
    /// </summary>
    public List<string> Assignees { get; set; } = null!;

    /// <summary>
    /// Gets a formatted string of the creation date.
    /// </summary>
    public string DisplayCreatedAt => CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm");

    /// <summary>
    /// Gets the color associated with the pull request state.
    /// </summary>
    public string StateColor => State?.ToLower() == "open" ? "#28a745" : "#6f42c1";

    /// <summary>
    /// Gets or sets the calculated review status (e.g., "Approved", "Changes Requested").
    /// </summary>
    public string? ReviewStatus { get; set; }

    /// <summary>
    /// Gets the color associated with the review status.
    /// </summary>
    public string ReviewStatusColor => ReviewStatus switch
    {
        "Approved" => "#28a745", // Green
        "Changes Requested" => "#dc3545", // Red
        "Review Requested" => "#e36209", // Orange
        "Commented" => "#17a2b8", // Blue
        _ => "#6c757d" // Gray
    };
}
