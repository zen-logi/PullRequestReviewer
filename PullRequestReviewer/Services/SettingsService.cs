namespace PullRequestReviewer.Services;

/// <inheritdoc cref="ISettingsService"/>
public class SettingsService : ISettingsService
{
    private const string GitHubTokenKey = "github_token";

    /// <inheritdoc/>
    public async Task<string?> GetGitHubTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(GitHubTokenKey);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task SaveGitHubTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync(GitHubTokenKey, token);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save GitHub token", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasGitHubTokenAsync()
    {
        var token = await GetGitHubTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    /// <inheritdoc/>
    public Task ClearGitHubTokenAsync()
    {
        SecureStorage.Remove(GitHubTokenKey);
        return Task.CompletedTask;
    }
}
