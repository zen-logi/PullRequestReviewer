using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PullRequestReviewer.Models;
using PullRequestReviewer.Services;

namespace PullRequestReviewer.ViewModels;

/// <summary>
/// PRリスト画面のViewModel
/// </summary>
public partial class PRListViewModel(
    IGitHubService gitHubService,
    ISettingsService settingsService,
    ILogger<PRListViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<PullRequestModel> _pullRequests = new();

    [ObservableProperty]
    private ObservableCollection<PullRequestModel> _filteredPullRequests = new();

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

    [ObservableProperty]
    private bool _isFilterPopupVisible;

    [ObservableProperty]
    private StatusFilterModel _currentStatusFilter = new();

    [ObservableProperty]
    private bool _isStatusFilterActive;

    /// <summary>
    /// タブごとのステータスフィルター状態を保持する辞書
    /// </summary>
    private readonly Dictionary<PullRequestFilterType, StatusFilterModel> _statusFiltersPerTab = new()
    {
        { PullRequestFilterType.All, new StatusFilterModel() },
        { PullRequestFilterType.ReviewRequested, new StatusFilterModel() },
        { PullRequestFilterType.Assigned, new StatusFilterModel() },
        { PullRequestFilterType.Authored, new StatusFilterModel() }
    };

    private CancellationTokenSource? _autoRefreshCts;

    /// <summary>
    /// ViewModelを初期化する
    /// </summary>
    public void Initialize()
    {
        foreach (var filter in _statusFiltersPerTab.Values)
        {
            filter.FilterChanged += OnStatusFilterChanged;
        }

        // 初期タブのフィルターを設定
        CurrentStatusFilter = _statusFiltersPerTab[PullRequestFilterType.All];
    }

    private void OnStatusFilterChanged(object? sender, EventArgs e)
    {
        IsStatusFilterActive = CurrentStatusFilter.IsActive;
        ApplyStatusFilter();
    }

    /// <summary>
    /// 非同期初期化を実行する
    /// </summary>
    public async Task InitializeAsync()
    {
        Initialize();
        logger.LogInformation("Initializing PR list view");

        // 認証方式に基づいてトークンを取得
        string? token = null;
        var authMethod = settingsService.GetAuthMethod();
        logger.LogDebug("Current auth method: {AuthMethod}", authMethod);

        if (authMethod == AuthMethod.OAuth)
        {
            token = await settingsService.GetOAuthAccessTokenAsync();
            logger.LogDebug("Using OAuth authentication, token exists: {HasToken}", !string.IsNullOrEmpty(token));
        }
        else if (authMethod == AuthMethod.PersonalAccessToken)
        {
            token = await settingsService.GetGitHubTokenAsync();
            logger.LogDebug("Using PAT authentication, token exists: {HasToken}", !string.IsNullOrEmpty(token));
        }
        else
        {
            // Fallback: Try to find any available token
            token = await settingsService.GetOAuthAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                settingsService.SetAuthMethod(AuthMethod.OAuth);
                logger.LogDebug("Found OAuth token, setting auth method");
            }
            else
            {
                token = await settingsService.GetGitHubTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    settingsService.SetAuthMethod(AuthMethod.PersonalAccessToken);
                    logger.LogDebug("Found PAT token, setting auth method");
                }
            }
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("No authentication found, navigating to settings");
            await Shell.Current.GoToAsync("TokenSettingPage");
            return;
        }

        logger.LogDebug("Token found, setting token and loading PRs");
        gitHubService.SetToken(token);
        await LoadPullRequestsAsync();

        // Start auto-refresh if configured
        StartAutoRefresh();
    }

    public void StartAutoRefresh()
    {
        StopAutoRefresh();

        var intervalMinutes = settingsService.GetAutoRefreshInterval();
        if (intervalMinutes <= 0)
        {
            logger.LogDebug("Auto-refresh is disabled (interval: {Interval})", intervalMinutes);
            return;
        }

        logger.LogInformation("Starting auto-refresh with interval: {Interval} minutes", intervalMinutes);
        _autoRefreshCts = new CancellationTokenSource();
        _ = RunAutoRefreshAsync(intervalMinutes, _autoRefreshCts.Token);
    }

    public void StopAutoRefresh()
    {
        if (_autoRefreshCts != null)
        {
            logger.LogDebug("Stopping auto-refresh");
            _autoRefreshCts.Cancel();
            _autoRefreshCts.Dispose();
            _autoRefreshCts = null;
        }
    }

    private async Task RunAutoRefreshAsync(int intervalMinutes, CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervalMinutes));
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                if (!IsLoading && !IsRefreshing)
                {
                    logger.LogInformation("Auto-refresh triggered");
                    await LoadPullRequestsAsync();
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogDebug("Auto-refresh cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in auto-refresh");
        }
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
                PullRequestFilterType.All => await gitHubService.GetAllPullRequestsGraphQlAsync(),
                PullRequestFilterType.ReviewRequested => await gitHubService.GetReviewRequestedPullRequestsGraphQlAsync(),
                PullRequestFilterType.Assigned => await gitHubService.GetAssignedPullRequestsGraphQlAsync(),
                PullRequestFilterType.Authored => await gitHubService.GetAuthoredPullRequestsGraphQlAsync(),
                _ => new List<PullRequestModel>()
            };

            logger.LogInformation("Received {Count} pull requests from service", prs.Count);

            PullRequests.Clear();
            foreach (var pr in prs)
            {
                PullRequests.Add(pr);
            }

            // ステータスフィルターを適用
            ApplyStatusFilter();

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

            // タブに応じたステータスフィルターを切り替え
            if (_statusFiltersPerTab.TryGetValue(filter, out var statusFilter))
            {
                CurrentStatusFilter = statusFilter;
                IsStatusFilterActive = statusFilter.IsActive;
            }

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

    /// <summary>
    /// フィルターポップアップの表示を切り替える。
    /// </summary>
    [RelayCommand]
    private void ToggleFilterPopup()
    {
        IsFilterPopupVisible = !IsFilterPopupVisible;
        logger.LogDebug("Filter popup visibility toggled: {IsVisible}", IsFilterPopupVisible);
    }

    /// <summary>
    /// フィルターポップアップを閉じる。
    /// </summary>
    [RelayCommand]
    private void CloseFilterPopup()
    {
        IsFilterPopupVisible = false;
    }

    /// <summary>
    /// ステータスフィルターの選択を切り替える。
    /// </summary>
    [RelayCommand]
    private void ToggleStatusFilter(string statusType)
    {
        logger.LogDebug("Toggling status filter: {StatusType}", statusType);

        switch (statusType.ToLowerInvariant())
        {
            case "open":
                CurrentStatusFilter.ShowOpen = !CurrentStatusFilter.ShowOpen;
                break;
            case "closed":
                CurrentStatusFilter.ShowClosed = !CurrentStatusFilter.ShowClosed;
                break;
            case "merged":
                CurrentStatusFilter.ShowMerged = !CurrentStatusFilter.ShowMerged;
                break;
        }

        IsStatusFilterActive = CurrentStatusFilter.IsActive;
        ApplyStatusFilter();
    }

    /// <summary>
    /// ステータスフィルターをリセットする。
    /// </summary>
    [RelayCommand]
    private void ResetStatusFilter()
    {
        logger.LogDebug("Resetting status filter");
        CurrentStatusFilter.Reset();
        IsStatusFilterActive = false;
        ApplyStatusFilter();
    }

    /// <summary>
    /// 現在のステータスフィルター設定に基づいてPR一覧をフィルタリングする。
    /// </summary>
    private void ApplyStatusFilter()
    {
        FilteredPullRequests.Clear();

        foreach (var pr in PullRequests)
        {
            if (CurrentStatusFilter.ShouldShow(pr.State, pr.IsDraft))
            {
                FilteredPullRequests.Add(pr);
            }
        }

        logger.LogDebug("Applied status filter: {Filtered}/{Total} PRs shown",
            FilteredPullRequests.Count, PullRequests.Count);
    }
}