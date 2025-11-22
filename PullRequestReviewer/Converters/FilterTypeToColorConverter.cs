using System.Globalization;
using PullRequestReviewer.Models;

namespace PullRequestReviewer.Converters;

public class FilterTypeToColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 &&
            values[0] is PullRequestFilterType currentFilter &&
            values[1] is string filterName &&
            Enum.TryParse<PullRequestFilterType>(filterName, out var targetFilter))
        {
            return currentFilter == targetFilter
                ? Application.Current?.Resources["Primary"] ?? Colors.Blue
                : Colors.Transparent;
        }

        return Colors.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
