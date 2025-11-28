using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace PullRequestReviewer.Services;

public class UpdateService(ILogger<UpdateService> logger) : IUpdateService
{
    private const string GitHubApiUrl = "https://api.github.com/repos/zen-logi/PullRequestReviewer/releases/latest";
    private readonly HttpClient _httpClient = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "PullRequestReviewer-App" } }
    };

    public string? LatestVersion { get; private set; }
    public string? ReleaseUrl { get; private set; }

    public async Task<bool> CheckForUpdatesAsync()
    {
        try
        {
            logger.LogInformation("Checking for updates...");
            var release = await _httpClient.GetFromJsonAsync<GitHubRelease>(GitHubApiUrl);
            
            if (release != null)
            {
                LatestVersion = release.TagName;
                ReleaseUrl = release.HtmlUrl;
                
                var currentVersion = AppInfo.VersionString;
                
                // Simple string comparison for now, assuming semantic versioning with 'v' prefix in tag
                var cleanLatest = LatestVersion?.TrimStart('v');
                
                if (Version.TryParse(cleanLatest, out var latest) && Version.TryParse(currentVersion, out var current))
                {
                    if (latest > current)
                    {
                        logger.LogInformation("Update available: {LatestVersion} (Current: {CurrentVersion})", LatestVersion, currentVersion);
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check for updates");
        }

        logger.LogInformation("No updates found or check failed");
        return false;
    }

    public async Task UpdateAppAsync()
    {
        if (!string.IsNullOrEmpty(ReleaseUrl))
        {
            await Browser.OpenAsync(ReleaseUrl, BrowserLaunchMode.SystemPreferred);
        }
    }

    private class GitHubRelease
    {
        public string TagName { get; set; } = string.Empty;
        public string HtmlUrl { get; set; } = string.Empty;
    }
}
