using PullRequestReviewer.Models;

namespace PullRequestReviewer.Services;

/// <summary>
/// GitHub関連の操作を提供するサービスインターフェース
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// 指定したGitHubトークンの有効性を検証する
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// レビュー依頼されたプルリクエストの一覧を取得する
    /// </summary>
    Task<List<PullRequestModel>> GetReviewRequestedPullRequestsAsync();

    /// <summary>
    /// 担当者として割り当てられたプルリクエストの一覧を取得する
    /// </summary>
    Task<List<PullRequestModel>> GetAssignedPullRequestsAsync();

    /// <summary>
    /// 作成者としてのプルリクエスト一覧を取得する
    /// </summary>
    Task<List<PullRequestModel>> GetAuthoredPullRequestsAsync();

    /// <summary>
    /// レビュー依頼・担当・作成した全てのプルリクエストの一覧を取得する
    /// </summary>
    Task<List<PullRequestModel>> GetAllPullRequestsAsync();

    /// <summary>
    /// GitHubトークンを設定する
    /// </summary>
    void SetToken(string token);

    /// <summary>
    /// GraphQL APIを使用して全てのプルリクエストを取得する
    /// </summary>
    Task<List<PullRequestModel>> GetAllPullRequestsGraphQlAsync();

    /// <summary>
    /// GraphQL APIを使用してレビュー依頼されたプルリクエストを取得する
    /// </summary>
    Task<List<PullRequestModel>> GetReviewRequestedPullRequestsGraphQlAsync();

    /// <summary>
    /// GraphQL APIを使用して担当者として割り当てられたプルリクエストを取得する
    /// </summary>
    Task<List<PullRequestModel>> GetAssignedPullRequestsGraphQlAsync();

    /// <summary>
    /// GraphQL APIを使用して作成したプルリクエストを取得する
    /// </summary>
    Task<List<PullRequestModel>> GetAuthoredPullRequestsGraphQlAsync();
}