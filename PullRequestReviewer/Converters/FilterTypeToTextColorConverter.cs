using System.Globalization;
using PullRequestReviewer.Models;

namespace PullRequestReviewer.Converters;

public class FilterTypeToTextColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 &&
            values[0] is PullRequestFilterType currentFilter &&
            values[1] is string filterName &&
            Enum.TryParse<PullRequestFilterType>(filterName, out var targetFilter))
        {
            return currentFilter == targetFilter
                ? Colors.White
                : Application.Current?.Resources["Gray900"] ?? Colors.Black;
        }

        return Application.Current?.Resources["Gray900"] ?? Colors.Black;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
