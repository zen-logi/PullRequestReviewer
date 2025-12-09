using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Represents a GitHub user.
/// </summary>
public sealed class GitHubUser
{
    /// <summary>
    /// Gets the username (login).
    /// </summary>
    [JsonPropertyName("login")]
    public required string Login { get; init; }

    /// <summary>
    /// Gets the URL of the user's avatar.
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; init; }

    [JsonPropertyName("id")]
    public long Id { get; init; }
}
