using System.Globalization;

namespace WikiExtractor.Maui.App.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                // Custom conversion logic to parse the color string
                if (colorString.StartsWith("#"))
                {
                    try
                    {
                        return Color.FromArgb(colorString);
                    }
                    catch (Exception)
                    {
                        // Return a default color if the conversion fails
                        return Colors.Transparent;
                    }
                }
                else
                {
                    try
                    {
                        // Try to parse named colors or other color formats
                        return Color.FromArgb(colorString);
                    }
                    catch (Exception)
                    {
                        // Try to get color from known color names
                        try
                        {
                            var colorProperty = typeof(Colors).GetProperty(colorString,
                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
                            if (colorProperty != null)
                            {
                                return (Color)colorProperty.GetValue(null)!;
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore and fall through to default
                        }
                        
                        // Return a default color if the conversion fails
                        return Colors.Transparent;
                    }
                }
            }

            // Return a default color if the conversion fails
            return Colors.Transparent;
        }
        
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToArgbHex();
            }

            // Return null if the conversion fails
            return null;
        }
        
        public Microsoft.Maui.Graphics.Color ToColorFromHex(string value)
        {
            // Remove # if present
            if (value.StartsWith("#"))
                value = value.Substring(1);
    
            // Parse hex string to ARGB values
            if (value.Length == 6) // RGB format
            {
                var r = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber) / 255f;
                var g = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber) / 255f;
                var b = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber) / 255f;
        
                return new Microsoft.Maui.Graphics.Color(r, g, b);
            }
            else if (value.Length == 8) // ARGB format
            {
                var a = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber) / 255f;
                var r = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber) / 255f;
                var g = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber) / 255f;
                var b = int.Parse(value.Substring(6, 2), NumberStyles.HexNumber) / 255f;
        
                return new Microsoft.Maui.Graphics.Color(r, g, b, a);
            }
    
            // Return default color if parsing fails
            return Microsoft.Maui.Graphics.Colors.Black;
        }
    }
}