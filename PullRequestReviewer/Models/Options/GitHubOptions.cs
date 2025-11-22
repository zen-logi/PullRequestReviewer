namespace PullRequestReviewer.Models.Options;

public sealed class GitHubOptions
{
    public const string SectionName = "GitHub";

    public required string ApiBaseUrl { get; init; }
    public required string UserAgent { get; init; }
    public int RequestTimeout { get; init; } = 30;
}
