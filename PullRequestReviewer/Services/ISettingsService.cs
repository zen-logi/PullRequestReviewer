using PullRequestReviewer.Models;

namespace PullRequestReviewer.Services;

/// <summary>
/// アプリケーション設定の管理を行うサービスインターフェース。
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// GitHubトークン（PAT）を安全に取得する。
    /// </summary>
    Task<string?> GetGitHubTokenAsync();

    /// <summary>
    /// GitHubトークン（PAT）を安全に保存する。
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

    /// <summary>
    /// 現在の認証方式を取得する。
    /// </summary>
    AuthMethod GetAuthMethod();

    /// <summary>
    /// 認証方式を保存する。
    /// </summary>
    void SetAuthMethod(AuthMethod method);

    /// <summary>
    /// OAuth アクセストークンを取得する。
    /// </summary>
    Task<string?> GetOAuthAccessTokenAsync();

    /// <summary>
    /// OAuth アクセストークンを保存する。
    /// </summary>
    Task SaveOAuthAccessTokenAsync(string token);

    /// <summary>
    /// OAuth アクセストークンを削除する。
    /// </summary>
    Task ClearOAuthAccessTokenAsync();

    /// <summary>
    /// OAuth ログイン時のユーザー名を取得する。
    /// </summary>
    string? GetOAuthUsername();

    /// <summary>
    /// OAuth ログイン時のユーザー名を保存する。
    /// </summary>
    void SetOAuthUsername(string? username);

    /// <summary>
    /// すべての認証情報をクリアする。
    /// </summary>
    Task ClearAllAuthAsync();
}
