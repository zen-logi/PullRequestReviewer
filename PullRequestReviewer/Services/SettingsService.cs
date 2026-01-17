using PullRequestReviewer.Models;

namespace PullRequestReviewer.Services;

/// <inheritdoc cref="ISettingsService"/>
public class SettingsService : ISettingsService
{
    private const string GitHubTokenKey = "github_token";
    private const string OAuthAccessTokenKey = "oauth_access_token";
    private const string AutoRefreshIntervalKey = "auto_refresh_interval";
    private const string AuthMethodKey = "auth_method";
    private const string OAuthUsernameKey = "oauth_username";

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

    /// <inheritdoc/>
    public int GetAutoRefreshInterval()
    {
        return Preferences.Get(AutoRefreshIntervalKey, 0);
    }

    /// <inheritdoc/>
    public void SetAutoRefreshInterval(int minutes)
    {
        Preferences.Set(AutoRefreshIntervalKey, Math.Max(0, minutes));
    }

    /// <inheritdoc/>
    public AuthMethod GetAuthMethod()
    {
        var value = Preferences.Get(AuthMethodKey, (int)AuthMethod.None);
        return (AuthMethod)value;
    }

    /// <inheritdoc/>
    public void SetAuthMethod(AuthMethod method)
    {
        Preferences.Set(AuthMethodKey, (int)method);
    }

    /// <inheritdoc/>
    public async Task<string?> GetOAuthAccessTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(OAuthAccessTokenKey);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task SaveOAuthAccessTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync(OAuthAccessTokenKey, token);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save OAuth access token", ex);
        }
    }

    /// <inheritdoc/>
    public Task ClearOAuthAccessTokenAsync()
    {
        SecureStorage.Remove(OAuthAccessTokenKey);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public string? GetOAuthUsername()
    {
        return Preferences.Get(OAuthUsernameKey, null as string);
    }

    /// <inheritdoc/>
    public void SetOAuthUsername(string? username)
    {
        if (username == null)
        {
            Preferences.Remove(OAuthUsernameKey);
        }
        else
        {
            Preferences.Set(OAuthUsernameKey, username);
        }
    }

    /// <inheritdoc/>
    public async Task ClearAllAuthAsync()
    {
        await ClearGitHubTokenAsync();
        await ClearOAuthAccessTokenAsync();
        SetAuthMethod(AuthMethod.None);
        SetOAuthUsername(null);
    }
}
