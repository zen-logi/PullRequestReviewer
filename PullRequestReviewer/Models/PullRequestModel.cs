namespace PullRequestReviewer.Models;

/// <summary>
/// GitHubプルリクエストおよびUI固有のプロパティを表すモデル
/// </summary>
public class PullRequestModel
{
    /// <summary>
    /// プルリクエストの一意な識別子を取得または設定する
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// プルリクエスト番号を取得または設定する
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// プルリクエストのタイトルを取得または設定する
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// プルリクエストの状態（open、closedなど）を取得または設定する
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// プルリクエストの本文/説明を取得または設定する
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// プルリクエストのHTML URLを取得または設定する
    /// </summary>
    public string? HtmlUrl { get; set; }

    /// <summary>
    /// プルリクエストの作成日時を取得または設定する
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// プルリクエストの最終更新日時を取得または設定する
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 作成者のユーザー名を取得または設定する
    /// </summary>
    public string AuthorLogin { get; set; } = string.Empty;

    /// <summary>
    /// 作成者のアバターURLを取得または設定する
    /// </summary>
    public string? AuthorAvatarUrl { get; set; }

    /// <summary>
    /// リポジトリ名（例: "RepoName"）を取得または設定する
    /// </summary>
    public string RepositoryName { get; set; } = string.Empty;

    /// <summary>
    /// リポジトリのオーナーを取得または設定する
    /// </summary>
    public string RepositoryOwner { get; set; } = string.Empty;

    /// <summary>
    /// リポジトリのフルネーム（例: "Owner/RepoName"）を取得または設定する
    /// </summary>
    public string RepositoryFullName { get; set; } = string.Empty;

    /// <summary>
    /// プルリクエストがドラフトかどうかを示す値を取得または設定する
    /// </summary>
    public bool IsDraft { get; set; }

    /// <summary>
    /// レビュー依頼されたユーザーのリストを取得または設定する
    /// </summary>
    public List<string> RequestedReviewers { get; set; } = null!;

    /// <summary>
    /// アサインされたユーザーのリストを取得または設定する
    /// </summary>
    public List<string> Assignees { get; set; } = null!;

    /// <summary>
    /// フォーマットされた作成日時文字列を取得する
    /// </summary>
    public string DisplayCreatedAt => CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm");

    /// <summary>
    /// プルリクエストの状態に対応する色を取得する
    /// </summary>
    public string StateColor => State?.ToLower() == "open" ? "#28a745" : "#6f42c1";

    /// <summary>
    /// 計算されたレビュー状態（"Approved"、"Changes Requested"など）を取得または設定する
    /// </summary>
    public ReviewStatus ReviewStatus { get; set; }

    /// <summary>
    /// レビュー状態に対応する色を取得または設定する。
    /// 明示的に設定されていない場合はReviewStatusに基づいて計算する
    /// </summary>
    public string ReviewStatusText => ReviewStatus switch
    {
        ReviewStatus.NoReviews => "No reviews",
        ReviewStatus.Pending => "Pending",
        ReviewStatus.Commented => "Commented",
        ReviewStatus.Approved => "Approved",
        ReviewStatus.ChangesRequested => "Changes requested",
        ReviewStatus.Error => "Error",
        _ => string.Empty
    };

    /// <summary>
    /// Gets the color associated with the review status.
    /// </summary>
    public string ReviewStatusColor => ReviewStatus switch
    {
        ReviewStatus.Approved => "#28a745", // Green
        ReviewStatus.ChangesRequested => "#dc3545", // Red
        ReviewStatus.Pending => "#e36209", // Orange
        ReviewStatus.Commented => "#17a2b8", // Blue
        ReviewStatus.NoReviews => "#6c757d", // Gray
        ReviewStatus.Error => "#dc3545", // Red
        _ => "#6c757d" // Gray
    };
}