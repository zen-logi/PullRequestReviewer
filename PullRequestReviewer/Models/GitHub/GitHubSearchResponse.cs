using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Represents a generic GitHub search response.
/// </summary>
/// <typeparam name="T">The type of items returned.</typeparam>
public sealed class GitHubSearchResponse<T>
{
    /// <summary>
    /// Gets the total count of items found.
    /// </summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets a value indicating whether the results are incomplete.
    /// </summary>
    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; init; }

    /// <summary>
    /// Gets the list of items found.
    /// </summary>
    [JsonPropertyName("items")]
    public required List<T> Items { get; init; }
}
