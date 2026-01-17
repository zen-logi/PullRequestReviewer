using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PullRequestReviewer.Models.GitHub;
using PullRequestReviewer.Models.Options;

namespace PullRequestReviewer.Services;

/// <inheritdoc cref="IGitHubAuthService"/>
public class GitHubAuthService(
    ILogger<GitHubAuthService> logger,
    IHttpClientFactory httpClientFactory,
    IOptions<GitHubOptions> gitHubOptions) : IGitHubAuthService
{
    private readonly GitHubOptions _options = gitHubOptions.Value;

    /// <inheritdoc/>
    public async Task<DeviceFlowResponse> StartDeviceFlowAsync()
    {
        logger.LogInformation("Starting GitHub Device Flow");

        var httpClient = httpClientFactory.CreateClient("GitHubAuth");

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.OAuth.ClientId,
            ["scope"] = _options.OAuth.Scopes
        });

        var request = new HttpRequestMessage(HttpMethod.Post, _options.OAuth.DeviceCodeUrl)
        {
            Content = content
        };
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DeviceFlowResponse>();
        if (result == null)
        {
            throw new InvalidOperationException("Failed to parse device flow response");
        }

        logger.LogInformation("Device Flow started. User code: {UserCode}, Verification URI: {VerificationUri}",
            result.UserCode, result.VerificationUri);

        return result;
    }

    /// <inheritdoc/>
    public async Task<string?> PollForAccessTokenAsync(string deviceCode, int interval, int expiresIn, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting to poll for access token");

        var httpClient = httpClientFactory.CreateClient("GitHubAuth");
        var startTime = DateTime.UtcNow;
        var currentInterval = interval;

        while (!cancellationToken.IsCancellationRequested)
        {
            // デバイスコードの有効期限チェック
            if ((DateTime.UtcNow - startTime).TotalSeconds >= expiresIn)
            {
                logger.LogWarning("Device code expired");
                return null;
            }

            await Task.Delay(TimeSpan.FromSeconds(currentInterval), cancellationToken);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _options.OAuth.ClientId,
                ["device_code"] = deviceCode,
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code"
            });

            var request = new HttpRequestMessage(HttpMethod.Post, _options.OAuth.AccessTokenUrl)
            {
                Content = content
            };
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await httpClient.SendAsync(request, cancellationToken);
                var result = await response.Content.ReadFromJsonAsync<AccessTokenResponse>(cancellationToken: cancellationToken);

                if (result == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(result.AccessToken))
                {
                    logger.LogInformation("Access token received successfully");
                    return result.AccessToken;
                }

                switch (result.Error)
                {
                    case "authorization_pending":
                        logger.LogDebug("Authorization pending, continuing to poll");
                        continue;

                    case "slow_down":
                        currentInterval += 5;
                        logger.LogDebug("Received slow_down, increasing interval to {Interval}", currentInterval);
                        continue;

                    case "expired_token":
                        logger.LogWarning("Device code expired");
                        return null;

                    case "access_denied":
                        logger.LogWarning("User denied access");
                        return null;

                    default:
                        logger.LogWarning("Unknown error: {Error} - {Description}", result.Error, result.ErrorDescription);
                        return null;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error polling for access token");
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<string?> GetUsernameAsync(string accessToken)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("GitHubAuth");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.ApiBaseUrl}/user");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
            return user?.Login;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get username");
            return null;
        }
    }
}
