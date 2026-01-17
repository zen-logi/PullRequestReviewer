using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PullRequestReviewer.Models;
using PullRequestReviewer.Services;

namespace PullRequestReviewer.ViewModels;

public partial class TokenSettingViewModel(
    ISettingsService settingsService,
    IGitHubService gitHubService,
    IGitHubAuthService gitHubAuthService,
    IUpdateService updateService,
    ILogger<TokenSettingViewModel> logger) : ObservableObject
{
    private CancellationTokenSource? _oauthPollCts;

    // PAT Authentication
    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private bool _isValidating;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isError;

    // OAuth Authentication
    [ObservableProperty]
    private bool _isOAuthLoggedIn;

    [ObservableProperty]
    private string _oAuthUsername = string.Empty;

    [ObservableProperty]
    private bool _isOAuthLoggingIn;

    [ObservableProperty]
    private string _oAuthUserCode = string.Empty;

    [ObservableProperty]
    private string _oAuthVerificationUrl = string.Empty;

    [ObservableProperty]
    private string _oAuthStatusMessage = string.Empty;

    // Auth method selection
    [ObservableProperty]
    private bool _showPATSection;

    [ObservableProperty]
    private bool _showOAuthSection = true;

    // Update checking
    [ObservableProperty]
    private bool _isCheckingUpdate;

    [ObservableProperty]
    private string _updateStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _isUpdateAvailable;

    [ObservableProperty]
    private int _autoRefreshInterval;

    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing token settings view");

        // Load current auth method
        var authMethod = settingsService.GetAuthMethod();
        logger.LogDebug("Current auth method: {AuthMethod}", authMethod);

        if (authMethod == AuthMethod.OAuth)
        {
            var oauthToken = await settingsService.GetOAuthAccessTokenAsync();
            if (!string.IsNullOrEmpty(oauthToken))
            {
                IsOAuthLoggedIn = true;
                OAuthUsername = settingsService.GetOAuthUsername() ?? "Unknown";
                ShowOAuthSection = true;
                ShowPATSection = false;
            }
        }
        else if (authMethod == AuthMethod.PersonalAccessToken)
        {
            var savedToken = await settingsService.GetGitHubTokenAsync();
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                Token = savedToken;
                ShowPATSection = true;
                ShowOAuthSection = false;
            }
        }

        // Load auto-refresh interval
        AutoRefreshInterval = settingsService.GetAutoRefreshInterval();
        logger.LogDebug("Loaded auto-refresh interval: {Interval} minutes", AutoRefreshInterval);
    }

    partial void OnAutoRefreshIntervalChanged(int value)
    {
        settingsService.SetAutoRefreshInterval(value);
        logger.LogInformation("Auto-refresh interval saved: {Interval} minutes", value);
    }

    [RelayCommand]
    private void SwitchToOAuth()
    {
        ShowOAuthSection = true;
        ShowPATSection = false;
    }

    [RelayCommand]
    private void SwitchToPAT()
    {
        ShowPATSection = true;
        ShowOAuthSection = false;
    }

    [RelayCommand]
    private async Task StartOAuthLoginAsync()
    {
        if (IsOAuthLoggingIn) return;

        IsOAuthLoggingIn = true;
        OAuthStatusMessage = "Starting login...";
        OAuthUserCode = string.Empty;
        OAuthVerificationUrl = string.Empty;

        try
        {
            logger.LogInformation("Starting OAuth Device Flow");
            var deviceFlow = await gitHubAuthService.StartDeviceFlowAsync();

            OAuthUserCode = deviceFlow.UserCode;
            OAuthVerificationUrl = deviceFlow.VerificationUri;
            OAuthStatusMessage = "Enter the code above at the URL below, then wait for authentication to complete.";

            // Open browser
            await Browser.OpenAsync(deviceFlow.VerificationUri, BrowserLaunchMode.SystemPreferred);

            // Start polling in background
            _oauthPollCts = new CancellationTokenSource();
            var accessToken = await gitHubAuthService.PollForAccessTokenAsync(
                deviceFlow.DeviceCode,
                deviceFlow.Interval,
                deviceFlow.ExpiresIn,
                _oauthPollCts.Token);

            if (!string.IsNullOrEmpty(accessToken))
            {
                // Get username
                var username = await gitHubAuthService.GetUsernameAsync(accessToken);

                // Save credentials
                await settingsService.SaveOAuthAccessTokenAsync(accessToken);
                settingsService.SetAuthMethod(AuthMethod.OAuth);
                settingsService.SetOAuthUsername(username);

                // Update UI
                IsOAuthLoggedIn = true;
                OAuthUsername = username ?? "Unknown";
                OAuthUserCode = string.Empty;
                OAuthVerificationUrl = string.Empty;
                OAuthStatusMessage = "Login successful!";

                // Set token for GitHubService
                gitHubService.SetToken(accessToken);

                logger.LogInformation("OAuth login successful for user: {Username}", username);

                await Task.Delay(1000);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                OAuthStatusMessage = "Login failed or timed out. Please try again.";
            }
        }
        catch (OperationCanceledException)
        {
            OAuthStatusMessage = "Login cancelled.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "OAuth login failed");
            OAuthStatusMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsOAuthLoggingIn = false;
            _oauthPollCts?.Dispose();
            _oauthPollCts = null;
        }
    }

    [RelayCommand]
    private void CancelOAuthLogin()
    {
        _oauthPollCts?.Cancel();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        try
        {
            logger.LogInformation("Logging out");

            bool result = await Shell.Current.DisplayAlertAsync(
                "Logout",
                "Are you sure you want to logout? You will need to login again to continue using the app.",
                "Logout",
                "Cancel");

            if (result)
            {
                await settingsService.ClearAllAuthAsync();

                // Reset UI state
                IsOAuthLoggedIn = false;
                OAuthUsername = string.Empty;
                Token = string.Empty;
                StatusMessage = "Logged out successfully";
                IsError = false;

                logger.LogInformation("Logout successful");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            StatusMessage = $"Error: {ex.Message}";
            IsError = true;
        }
    }

    [RelayCommand]
    private async Task CheckForUpdatesAsync()
    {
        if (IsCheckingUpdate) return;

        IsCheckingUpdate = true;
        UpdateStatusMessage = "Checking for updates...";
        IsUpdateAvailable = false;

        try
        {
            var hasUpdate = await updateService.CheckForUpdatesAsync();
            if (hasUpdate)
            {
                IsUpdateAvailable = true;
                UpdateStatusMessage = $"Update available: {updateService.LatestVersion}";
            }
            else
            {
                UpdateStatusMessage = "You are using the latest version.";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking for updates");
            UpdateStatusMessage = "Failed to check for updates.";
        }
        finally
        {
            IsCheckingUpdate = false;
        }
    }

    [RelayCommand]
    private async Task UpdateAppAsync()
    {
        await updateService.UpdateAppAsync();
    }

    [RelayCommand]
    private async Task GenerateTokenAsync()
    {
        logger.LogInformation("Opening GitHub token generation page");
        var url = "https://github.com/settings/tokens/new?scopes=repo&description=PullRequestReviewer";
        await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand(CanExecute = nameof(CanSaveToken))]
    private async Task SaveTokenAsync()
    {
        logger.LogInformation("Attempting to save GitHub token");
        IsValidating = true;
        StatusMessage = "Validating token...";
        IsError = false;

        try
        {
            var isValid = await gitHubService.ValidateTokenAsync(Token);

            if (isValid)
            {
                logger.LogInformation("Token validation successful, saving token");
                await settingsService.SaveGitHubTokenAsync(Token);
                settingsService.SetAuthMethod(AuthMethod.PersonalAccessToken);
                gitHubService.SetToken(Token);
                StatusMessage = "Token saved successfully!";
                IsError = false;

                await Task.Delay(1000);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                logger.LogWarning("Token validation failed");
                StatusMessage = "Invalid token. Please check and try again.";
                IsError = true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving token");
            StatusMessage = $"Error: {ex.Message}";
            IsError = true;
        }
        finally
        {
            IsValidating = false;
        }
    }

    private bool CanSaveToken() => !string.IsNullOrWhiteSpace(Token) && !IsValidating;

    partial void OnTokenChanged(string value)
    {
        SaveTokenCommand.NotifyCanExecuteChanged();
    }
}
