namespace PullRequestReviewer.Services;

/// <summary>
/// アプリケーションの更新確認と適用を行うサービスインターフェース。
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// 更新の有無を確認する。
    /// </summary>
    /// <returns>更新がある場合はtrue、それ以外はfalse。</returns>
    Task<bool> CheckForUpdatesAsync();

    /// <summary>
    /// アプリケーションの更新を行う（リリースぺージを開く等）。
    /// </summary>
    Task UpdateAppAsync();

    /// <summary>
    /// 最新バージョンのバージョン文字列を取得する。
    /// </summary>
    string? LatestVersion { get; }

    /// <summary>
    /// リリースノートのURLを取得する。
    /// </summary>
    string? ReleaseUrl { get; }
}
