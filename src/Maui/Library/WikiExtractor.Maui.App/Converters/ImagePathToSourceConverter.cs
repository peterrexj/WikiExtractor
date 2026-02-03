using System.Globalization;

namespace WikiExtractor.Maui.App.Converters
{
    /// <summary>
    /// Converts image paths to proper ImageSource, handling missing images with a fallback placeholder.
    /// Returns "no_image_available.png" when the path is null, empty, or "NoImageAvailable.png"
    /// </summary>
    public class ImagePathToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "no_image_available.png";

            var imagePath = value.ToString();

            // Check if image path is empty or the placeholder string
            if (string.IsNullOrWhiteSpace(imagePath) || 
                imagePath.Equals("NoImageAvailable.png", StringComparison.OrdinalIgnoreCase))
            {
                return "no_image_available.png";
            }

            // Return the original path if it's valid
            return imagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
