using CommunityToolkit.Mvvm.ComponentModel;

namespace PullRequestReviewer.Models;

/// <summary>
/// ステータス絞り込みの選択状態を管理するモデル
/// </summary>
public partial class StatusFilterModel : ObservableObject
{
    /// <summary>
    /// フィルター設定が変更された際に発火するイベント
    /// </summary>
    public event EventHandler? FilterChanged;

    /// <summary>
    /// Openステータスを表示するかどうか
    /// </summary>
    [ObservableProperty]
    private bool _showOpen = true;

    /// <summary>
    /// Closedステータスを表示するかどうか
    /// </summary>
    [ObservableProperty]
    private bool _showClosed = true;

    /// <summary>
    /// Mergedステータスを表示するかどうか
    /// </summary>
    [ObservableProperty]
    private bool _showMerged = true;

    /// <summary>
    /// Draft PRを表示するかどうか
    /// </summary>
    [ObservableProperty]
    private bool _showDraft = true;

    /// <summary>
    /// フィルターがデフォルト状態（全て表示）かどうかを取得する
    /// </summary>
    public bool IsDefault => ShowOpen && ShowClosed && ShowMerged && ShowDraft;

    /// <summary>
    /// フィルターがアクティブ（デフォルトから変更されている）かどうかを取得する
    /// </summary>
    public bool IsActive => !IsDefault;

    partial void OnShowOpenChanged(bool value) => OnFilterSettingChanged();
    partial void OnShowClosedChanged(bool value) => OnFilterSettingChanged();
    partial void OnShowMergedChanged(bool value) => OnFilterSettingChanged();
    partial void OnShowDraftChanged(bool value) => OnFilterSettingChanged();

    /// <summary>
    /// フィルター設定変更時の通知を行う
    /// </summary>
    private void OnFilterSettingChanged()
    {
        FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// フィルターをデフォルト状態（全て表示）にリセットする
    /// </summary>
    public void Reset()
    {
        ShowOpen = true;
        ShowClosed = true;
        ShowMerged = true;
        ShowDraft = true;
    }

    /// <summary>
    /// 指定されたPRがフィルターを通過するかどうかを判定する
    /// </summary>
    /// <param name="state">PRのステータス（open, closed, merged）</param>
    /// <param name="isDraft">ドラフトPRかどうか</param>
    /// <returns>表示する場合はtrue</returns>
    public bool ShouldShow(string? state, bool isDraft)
    {
        // ドラフトフィルター
        if (isDraft && !ShowDraft)
            return false;

        // ステータスフィルター
        if (string.IsNullOrEmpty(state))
            return true;

        return state.ToLowerInvariant() switch
        {
            "open" => ShowOpen,
            "closed" => ShowClosed,
            "merged" => ShowMerged,
            _ => true
        };
    }
}