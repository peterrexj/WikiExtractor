using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace WikiExtractor.Maui.App.Converters
{
    public class ImagePathToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imagePath = value as string;

            if (string.IsNullOrWhiteSpace(imagePath) ||
                imagePath.Equals("NoImageAvailable.png", StringComparison.OrdinalIgnoreCase) ||
                (imagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase) == false && !File.Exists(imagePath)))
            {
                // Always return a new instance so CollectionView recycled cells that share
                // the same sentinel string still get a distinct reference and force a re-render.
                return ImageSource.FromFile("no_image_available.png");
            }

            return ImageSource.FromFile(imagePath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
