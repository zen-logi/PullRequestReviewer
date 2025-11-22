using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

public sealed class GitHubSearchResponse<T>
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; init; }

    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; init; }

    [JsonPropertyName("items")]
    public required List<T> Items { get; init; }
}
