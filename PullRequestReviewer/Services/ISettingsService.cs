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
}
