using System.Globalization;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Converters;

/// <summary>
/// Converts List&lt;MetadataViewModel&gt; into a FormattedString that matches the old
/// BindableLayout style: bold key in WikiAppListItemKeyTextColor, regular value in
/// WikiAppListItemDescriptionTextColor, rows separated by a newline with spacing.
/// </summary>
public class MetadataToFormattedStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not List<MetadataViewModel> items || items.Count == 0)
            return null;

        var res = Application.Current?.Resources;

        Color keyColor = Colors.Gray;
        Color valueColor = Colors.Gray;

        if (res != null)
        {
            if (res.TryGetValue("WikiAppListItemKeyTextColor", out var kc) && kc is Color k)
                keyColor = k;
            if (res.TryGetValue("WikiAppListItemDescriptionTextColor", out var vc) && vc is Color v)
                valueColor = v;
        }

        var formatted = new FormattedString();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            // Bold key — uses the distinct per-theme key colour (e.g. purple in Candy/Light)
            formatted.Spans.Add(new Span
            {
                Text = item.Key,
                FontAttributes = FontAttributes.Bold,
                TextColor = keyColor,
            });

            // Colon separator — same colour as key (matches old BindableLayout)
            formatted.Spans.Add(new Span
            {
                Text = ": ",
                TextColor = keyColor,
            });

            // Regular value — description colour
            formatted.Spans.Add(new Span
            {
                Text = item.Description,
                TextColor = valueColor,
            });

            // Row separator — newline between items, none after last
            if (i < items.Count - 1)
            {
                formatted.Spans.Add(new Span { Text = "\n" });
            }
        }

        return formatted;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
