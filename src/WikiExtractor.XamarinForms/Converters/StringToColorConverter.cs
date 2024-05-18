using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        static ColorTypeConverter _ColorTypeConverter = new ColorTypeConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                // Custom conversion logic to parse the color string
                if (colorString.StartsWith("#") /*&& colorString.Length == 7*/)
                {
                    try
                    {
                        return ColorConverters.FromHex(colorString);
                    }
                    catch (Exception)
                    {
                        // Return a default color if the conversion fails
                        return Color.Default;
                    }
                }
                else
                {
                    return _ColorTypeConverter.ConvertFromInvariantString(colorString);
                }
            }

            // Return a default color if the conversion fails
            return Color.Default;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToHex();
            }

            // Return null if the conversion fails
            return null;
        }
        public System.Drawing.Color ToColorFromHex(string value)
        {
            return (System.Drawing.Color)Convert(value, typeof(Color), null, CultureInfo.CurrentCulture);
        }
    }
}
