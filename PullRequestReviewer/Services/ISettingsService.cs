namespace PullRequestReviewer.Services;

/// <summary>
/// アプリケーション設定の管理を行うサービスインターフェース。
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// GitHubトークンを安全に取得する。
    /// </summary>
    Task<string?> GetGitHubTokenAsync();

    /// <summary>
    /// GitHubトークンを安全に保存する。
    /// </summary>
    Task SaveGitHubTokenAsync(string token);

    /// <summary>
    /// GitHubトークンの存在有無を判定する。
    /// </summary>
    Task<bool> HasGitHubTokenAsync();

    /// <summary>
    /// GitHubトークンを安全に削除する。
    /// </summary>
    Task ClearGitHubTokenAsync();

    /// <summary>
    /// 自動更新間隔（分）を取得する。0の場合は自動更新無効。
    /// </summary>
    int GetAutoRefreshInterval();

    /// <summary>
    /// 自動更新間隔（分）を保存する。0の場合は自動更新無効。
    /// </summary>
    void SetAutoRefreshInterval(int minutes);
}

