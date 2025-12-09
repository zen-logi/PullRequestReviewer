using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Represents a GitHub repository.
/// </summary>
public sealed class GitHubRepository
{
    /// <summary>
    /// Gets the unique identifier of the repository.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the name of the repository.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the full name of the repository (owner/name).
    /// </summary>
    [JsonPropertyName("full_name")]
    public required string FullName { get; init; }

    /// <summary>
    /// Gets the owner of the repository.
    /// </summary>
    [JsonPropertyName("owner")]
    public required GitHubUser Owner { get; init; }
}
