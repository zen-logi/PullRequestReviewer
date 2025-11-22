using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

public sealed class GitHubRepository
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("full_name")]
    public required string FullName { get; init; }

    [JsonPropertyName("owner")]
    public required GitHubUser Owner { get; init; }
}
