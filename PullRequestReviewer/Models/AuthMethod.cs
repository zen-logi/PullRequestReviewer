namespace PullRequestReviewer.Models;

/// <summary>
/// Represents the authentication method used for GitHub API access.
/// </summary>
public enum AuthMethod
{
    /// <summary>
    /// No authentication configured.
    /// </summary>
    None,

    /// <summary>
    /// Personal Access Token authentication.
    /// </summary>
    PersonalAccessToken,

    /// <summary>
    /// OAuth Device Flow authentication.
    /// </summary>
    OAuth
}