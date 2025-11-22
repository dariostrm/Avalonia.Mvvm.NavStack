using System.Globalization;
using Avalonia.Data.Converters;

namespace NestedNav.Avalonia.Dialogs;

public class ScaleConverters
{
    public static readonly IValueConverter FromBool = new BoolToScaleConverter();
}

public class BoolToScaleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //convert true to "scale(1)" and false to "scale(parameter)" or "scale(0)" if parameter is not provided
        if (value is bool and true)
        {
            return "scale(1)";
        }
        else
        {
            if (double.TryParse(parameter?.ToString(), out double result))
                return $"scale({Math.Round(result, 2)})";
            return "scale(0)";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string and "scale(1)";
    }
}