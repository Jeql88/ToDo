using System.Globalization;

namespace ToDo.Converters;

public class StatusToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status == "inactive" ? "checkfilled.png" : "checkempty.png";
        }
        return "checkempty.png";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}