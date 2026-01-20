using System.Globalization;

namespace WikiExtractor.Maui.App.Converters;

public class BusyToOpacityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isBusy && isBusy)
            return 0.3; // Faded out

        return 1.0; // Fully visible
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}