using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PullRequestReviewer.Services;

namespace PullRequestReviewer.ViewModels;

public partial class TokenSettingViewModel(
    ISettingsService settingsService,
    IGitHubService gitHubService,
    ILogger<TokenSettingViewModel> logger) : ObservableObject
{

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private bool _isValidating;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isError;

    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing token settings view");
        var savedToken = await settingsService.GetGitHubTokenAsync();
        if (!string.IsNullOrWhiteSpace(savedToken))
        {
            logger.LogDebug("Found saved token");
            Token = savedToken;
        }
        else
        {
            logger.LogDebug("No saved token found");
        }
    }

    [RelayCommand]
    private async Task GenerateTokenAsync()
    {
        logger.LogInformation("Opening GitHub token generation page");
        var url = "https://github.com/settings/tokens/new?scopes=repo&description=PullRequestReviewer";
        await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task ResetTokenAsync()
    {
        try
        {
            logger.LogInformation("Resetting GitHub token");

            bool result;
            if (Shell.Current != null)
            {
                result = await Shell.Current.DisplayAlertAsync(
                    "Reset Token",
                    "Are you sure you want to reset your GitHub token? You will need to enter a new token to continue using the app.",
                    "Reset",
                    "Cancel");
            }
            else if (Application.Current?.Windows.Count > 0)
            {
                var page = Application.Current.Windows[0].Page;
                if (page == null)
                {
                    logger.LogError("Unable to show confirmation dialog: no page available");
                    StatusMessage = "Error: Unable to show confirmation dialog";
                    IsError = true;
                    return;
                }
                result = await page.DisplayAlertAsync(
                    "Reset Token",
                    "Are you sure you want to reset your GitHub token? You will need to enter a new token to continue using the app.",
                    "Reset",
                    "Cancel");
            }
            else
            {
                logger.LogError("Unable to show confirmation dialog: Shell.Current and Application.Current.Windows are null");
                StatusMessage = "Error: Unable to show confirmation dialog";
                IsError = true;
                return;
            }

            if (result)
            {
                logger.LogInformation("User confirmed token reset");
                await settingsService.ClearGitHubTokenAsync();
                Token = string.Empty;
                StatusMessage = "Token has been reset";
                IsError = false;
                logger.LogInformation("Token reset successfully");
            }
            else
            {
                logger.LogInformation("User cancelled token reset");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resetting token");
            StatusMessage = $"Error resetting token: {ex.Message}";
            IsError = true;
        }
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
