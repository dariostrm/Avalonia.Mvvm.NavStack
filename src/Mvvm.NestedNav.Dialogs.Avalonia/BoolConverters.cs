using System.Globalization;
using Avalonia.Data.Converters;

namespace NestedNav.Avalonia.Dialogs;

public class BoolConverters
{
    public static readonly IValueConverter ToDouble = new BoolToDoubleConverter();
}

public class BoolToDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool and true)
        {
            if (double.TryParse(parameter?.ToString(), out double result))
                return result;
            return 1.0;
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double and > 0)
            return true;
        return false;
    }
}