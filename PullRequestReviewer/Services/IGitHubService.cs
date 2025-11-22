using PullRequestReviewer.Models;

namespace PullRequestReviewer.Services;

/// <summary>
/// GitHub関連の操作を提供するサービスインターフェース。
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// 指定したGitHubトークンの有効性を検証する。
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// レビュー依頼されたプルリクエストの一覧を取得する。
    /// </summary>
    Task<List<PullRequestModel>> GetReviewRequestedPullRequestsAsync();

    /// <summary>
    /// 担当者として割り当てられたプルリクエストの一覧を取得する。
    /// </summary>
    Task<List<PullRequestModel>> GetAssignedPullRequestsAsync();

    /// <summary>
    /// 作成者としてのプルリクエスト一覧を取得する。
    /// </summary>
    Task<List<PullRequestModel>> GetAuthoredPullRequestsAsync();

    /// <summary>
    /// レビュー依頼・担当・作成した全てのプルリクエストの一覧を取得する。
    /// </summary>
    Task<List<PullRequestModel>> GetAllPullRequestsAsync();

    /// <summary>
    /// GitHubトークンを設定する。
    /// </summary>
    void SetToken(string token);
}
