namespace PullRequestReviewer.Models;

/// <summary>
/// Represents the review status of a pull request.
/// </summary>
public enum ReviewStatus
{
    /// <summary>
    /// No reviews have been submitted.
    /// </summary>
    NoReviews,

    /// <summary>
    /// Review is pending (re-requested or awaiting initial review).
    /// </summary>
    Pending,

    /// <summary>
    /// Reviewers have commented on the pull request.
    /// </summary>
    Commented,

    /// <summary>
    /// The pull request has been approved.
    /// </summary>
    Approved,

    /// <summary>
    /// Changes have been requested on the pull request.
    /// </summary>
    ChangesRequested,

    /// <summary>
    /// An error occurred while fetching review status.
    /// </summary>
    Error
}
