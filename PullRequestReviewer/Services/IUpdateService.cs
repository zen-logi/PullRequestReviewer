namespace PullRequestReviewer.Services;

public interface IUpdateService
{
    Task<bool> CheckForUpdatesAsync();
    Task UpdateAppAsync();
    string? LatestVersion { get; }
    string? ReleaseUrl { get; }
}
