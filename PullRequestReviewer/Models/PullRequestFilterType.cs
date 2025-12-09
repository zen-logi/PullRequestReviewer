namespace PullRequestReviewer.Models;

/// <summary>
/// Defines the types of filters available for pull requests.
/// </summary>
public enum PullRequestFilterType
{
    /// <summary>
    /// Include all pull requests (review requested, assigned, authored).
    /// </summary>
    All,

    /// <summary>
    /// Include only pull requests where a review is requested from the user.
    /// </summary>
    ReviewRequested,

    /// <summary>
    /// Include only pull requests assigned to the user.
    /// </summary>
    Assigned,

    /// <summary>
    /// Include only pull requests authored by the user.
    /// </summary>
    Authored
}
