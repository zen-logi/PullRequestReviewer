using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

public sealed class GitHubUser
{
    [JsonPropertyName("login")]
    public required string Login { get; init; }

    [JsonPropertyName("avatar_url")]
    public required string AvatarUrl { get; init; }

    [JsonPropertyName("id")]
    public long Id { get; init; }
}
