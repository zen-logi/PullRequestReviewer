using System.Text.Json.Serialization;

namespace PullRequestReviewer.Models.GitHub;

/// <summary>
/// Response from GitHub Device Flow code request.
/// </summary>
public sealed class DeviceFlowResponse
{
    /// <summary>
    /// The device verification code used to verify the device.
    /// </summary>
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; init; } = string.Empty;

    /// <summary>
    /// The user verification code displayed to the user.
    /// </summary>
    [JsonPropertyName("user_code")]
    public string UserCode { get; init; } = string.Empty;

    /// <summary>
    /// The verification URL where users need to enter the code.
    /// </summary>
    [JsonPropertyName("verification_uri")]
    public string VerificationUri { get; init; } = string.Empty;

    /// <summary>
    /// The number of seconds before the codes expire.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// The minimum number of seconds between polling requests.
    /// </summary>
    [JsonPropertyName("interval")]
    public int Interval { get; init; } = 5;
}

/// <summary>
/// Response from GitHub OAuth access token request.
/// </summary>
public sealed class AccessTokenResponse
{
    /// <summary>
    /// The access token for API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    /// <summary>
    /// The token type (usually "bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }

    /// <summary>
    /// The granted scopes.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Error code if the request failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>
    /// Error description if the request failed.
    /// </summary>
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }
}
