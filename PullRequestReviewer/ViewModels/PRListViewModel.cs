using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PullRequestReviewer.Models;
using PullRequestReviewer.Services;

namespace PullRequestReviewer.ViewModels;

public partial class PRListViewModel(IGitHubService gitHubService, ISettingsService settingsService, ILogger<PRListViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<PullRequestModel> _pullRequests = new();

    [ObservableProperty]
    private PullRequestFilterType _currentFilter = PullRequestFilterType.All;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;


    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing PR list view");
        var token = await settingsService.GetGitHubTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("No GitHub token found, navigating to token settings");
            await Shell.Current.GoToAsync("TokenSettingPage");
            return;
        }

        logger.LogDebug("GitHub token found, setting token and loading PRs");
        gitHubService.SetToken(token);
        await LoadPullRequestsAsync();
    }

    [RelayCommand]
    private async Task LoadPullRequestsAsync()
    {
        if (IsLoading)
        {
            logger.LogDebug("Load already in progress, skipping");
            return;
        }

        logger.LogInformation("Loading pull requests with filter: {Filter}", CurrentFilter);
        IsLoading = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var prs = CurrentFilter switch
            {
                PullRequestFilterType.All => await gitHubService.GetAllPullRequestsAsync(),
                PullRequestFilterType.ReviewRequested => await gitHubService.GetReviewRequestedPullRequestsAsync(),
                PullRequestFilterType.Assigned => await gitHubService.GetAssignedPullRequestsAsync(),
                PullRequestFilterType.Authored => await gitHubService.GetAuthoredPullRequestsAsync(),
                _ => new List<PullRequestModel>()
            };

            logger.LogInformation("Received {Count} pull requests from service", prs.Count);

            PullRequests.Clear();
            foreach (var pr in prs)
            {
                PullRequests.Add(pr);
            }

            logger.LogInformation("Successfully loaded {Count} pull requests into ObservableCollection", PullRequests.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load pull requests with filter: {Filter}", CurrentFilter);
            ErrorMessage = $"Failed to load pull requests: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        logger.LogInformation("Refreshing pull requests");
        IsRefreshing = true;
        await LoadPullRequestsAsync();
    }

    [RelayCommand]
    private async Task FilterChangedAsync(string filterType)
    {
        logger.LogDebug("Filter change requested: {FilterType}", filterType);
        if (Enum.TryParse<PullRequestFilterType>(filterType, out var filter))
        {
            logger.LogInformation("Changing filter from {OldFilter} to {NewFilter}", CurrentFilter, filter);
            CurrentFilter = filter;
            await LoadPullRequestsAsync();
        }
        else
        {
            logger.LogWarning("Invalid filter type: {FilterType}", filterType);
        }
    }

    [RelayCommand]
    private async Task OpenPullRequestAsync(PullRequestModel pr)
    {
        if (pr?.HtmlUrl != null)
        {
            logger.LogInformation("Opening pull request in browser: {Repo}#{Number}", pr.RepositoryFullName, pr.Number);
            await Browser.OpenAsync(pr.HtmlUrl, BrowserLaunchMode.SystemPreferred);
        }
        else
        {
            logger.LogWarning("Cannot open pull request: URL is null");
        }
    }

    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        logger.LogInformation("Navigating to token settings page");
        await Shell.Current.GoToAsync("TokenSettingPage");
    }
}
