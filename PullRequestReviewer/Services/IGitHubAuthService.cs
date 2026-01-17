using PullRequestReviewer.Models.GitHub;

namespace PullRequestReviewer.Services;

/// <summary>
/// GitHub OAuth Device Flow を管理するサービスインターフェース。
/// </summary>
public interface IGitHubAuthService
{
    /// <summary>
    /// Device Flow を開始し、ユーザーコードと検証 URL を取得する。
    /// </summary>
    /// <returns>Device Flow のレスポンス（ユーザーコード、検証 URL など）</returns>
    Task<DeviceFlowResponse> StartDeviceFlowAsync();

    /// <summary>
    /// アクセストークンの取得をポーリングする。
    /// </summary>
    /// <param name="deviceCode">Device Flow で取得した device_code</param>
    /// <param name="interval">ポーリング間隔（秒）</param>
    /// <param name="expiresIn">有効期限（秒）</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>アクセストークン、または失敗時は null</returns>
    Task<string?> PollForAccessTokenAsync(string deviceCode, int interval, int expiresIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// 現在のユーザー名を取得する。
    /// </summary>
    /// <param name="accessToken">アクセストークン</param>
    /// <returns>ユーザー名</returns>
    Task<string?> GetUsernameAsync(string accessToken);
}
