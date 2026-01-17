namespace PullRequestReviewer.Models.Options;

public sealed class GitHubOptions
{
    public const string SectionName = "GitHub";

    public required string ApiBaseUrl { get; init; }
    public required string UserAgent { get; init; }
    public int RequestTimeout { get; init; } = 30;
    public OAuthOptions OAuth { get; init; } = new();
}

public sealed class OAuthOptions
{
    public string ClientId { get; init; } = string.Empty;
    public string DeviceCodeUrl { get; init; } = "https://github.com/login/device/code";
    public string AccessTokenUrl { get; init; } = "https://github.com/login/oauth/access_token";
    public string Scopes { get; init; } = "repo";
}

