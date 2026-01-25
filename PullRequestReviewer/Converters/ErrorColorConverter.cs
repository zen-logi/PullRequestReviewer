using System.Globalization;

namespace PullRequestReviewer.Converters;

/// <summary>
/// エラー状態に応じて色を変換するコンバーター
/// </summary>
public class ErrorColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isError)
        {
            return isError ? Colors.Red : Colors.Green;
        }
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}