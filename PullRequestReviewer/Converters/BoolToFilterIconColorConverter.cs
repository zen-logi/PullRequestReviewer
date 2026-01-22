using System.Globalization;

namespace PullRequestReviewer.Converters;

/// <summary>
/// フィルターがアクティブかどうかに応じてアイコンの色を変換するコンバーター。
/// </summary>
public class BoolToFilterIconColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive && isActive)
        {
            return Color.FromArgb("#FF6B35"); // オレンジ（アクティブ時）
        }
        return Colors.White; // デフォルト
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}