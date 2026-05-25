using Microsoft.Maui.Storage;
using Syncfusion.Maui.DataSource;
// using Syncfusion.Maui.DataSource; // Temporarily disabled
using WikiExtractor.Maui.App.Exts; // For temporary ListSortDirection enum
using WikiExtractor.Maui.App.Models.Mix;

namespace WikiExtractor.Maui.App.Services
{
    public static class AppSettingsService
    {
        // Setting Keys Constants
        private const string SORT_PROPERTY_NAME_KEY = "SortPropertyName";
        private const string SORT_DIRECTION_KEY = "SortDirection";
        private const string SHOW_ALREADY_READ_ITEM_KEY = "ShowAlreadyReadItem";
        private const string APP_THEME_KEY = "AppTheme";
        private const string APP_THEME_BG_COLOR_KEY = "ThemeBgColor";
        private const string APP_FONT_FAMILY_KEY = "AppFontFamily";
        private const string APP_PARAGRAPH_FONT_SIZE_KEY = "AppParagraphFontSize";

        // Default Values
        private const string DEFAULT_SORT_PROPERTY = "Id";
        private const string DEFAULT_SORT_DIRECTION = "Ascending";
        private const bool DEFAULT_SHOW_READ_ITEMS = true;
        private const AppThemes DEFAULT_APP_THEME = AppThemes.Light;
        private const string DEFAULT_FONT_FAMILY = "Calibri";
        public const double DEFAULT_PARAGRAPH_FONT_SIZE = 14.0;
        public const double MIN_PARAGRAPH_FONT_SIZE = 10.0;
        public const double MAX_PARAGRAPH_FONT_SIZE = 24.0;

        /// <summary>
        /// Gets the current sort descriptor from secure storage
        /// </summary>
        public static async Task<MainListSortDescriptorModel> GetSortDescriptorAsync()
        {
            try
            {
                var propertyName = await SecureStorage.GetAsync(SORT_PROPERTY_NAME_KEY) ?? DEFAULT_SORT_PROPERTY;
                var directionStr = await SecureStorage.GetAsync(SORT_DIRECTION_KEY) ?? DEFAULT_SORT_DIRECTION;
                
                var direction = Enum.TryParse<ListSortDirection>(directionStr, out var parsedDirection) 
                    ? parsedDirection 
                    : ListSortDirection.Ascending;

                return new MainListSortDescriptorModel 
                { 
                    PropertyName = propertyName, 
                    Direction = direction 
                };
            }
            catch
            {
                return new MainListSortDescriptorModel 
                { 
                    PropertyName = DEFAULT_SORT_PROPERTY, 
                    Direction = ListSortDirection.Ascending 
                };
            }
        }

        /// <summary>
        /// Saves the sort descriptor to secure storage
        /// </summary>
        public static async Task SaveSortDescriptorAsync(string propertyName, ListSortDirection direction)
        {
            try
            {
                await SecureStorage.SetAsync(SORT_PROPERTY_NAME_KEY, propertyName);
                await SecureStorage.SetAsync(SORT_DIRECTION_KEY, direction.ToString());
            }
            catch
            {
                // Handle storage errors gracefully
            }
        }

        /// <summary>
        /// Saves the sort descriptor to secure storage
        /// </summary>
        public static async Task SaveSortDescriptorAsync(MainListSortDescriptorModel sortDescriptor)
        {
            await SaveSortDescriptorAsync(sortDescriptor.PropertyName, sortDescriptor.Direction);
        }

        /// <summary>
        /// Gets whether to show already read items
        /// </summary>
        public static async Task<bool> GetShowAlreadyReadItemsAsync()
        {
            try
            {
                var value = await SecureStorage.GetAsync(SHOW_ALREADY_READ_ITEM_KEY);
                return value != null ? bool.Parse(value) : DEFAULT_SHOW_READ_ITEMS;
            }
            catch
            {
                return DEFAULT_SHOW_READ_ITEMS;
            }
        }

        /// <summary>
        /// Sets whether to show already read items
        /// </summary>
        public static async Task SetShowAlreadyReadItemsAsync(bool shouldShow)
        {
            try
            {
                await SecureStorage.SetAsync(SHOW_ALREADY_READ_ITEM_KEY, shouldShow.ToString());
            }
            catch
            {
                // Handle storage errors gracefully
            }
        }

        /// <summary>
        /// Gets the current app theme
        /// </summary>
        public static async Task<AppThemes> GetAppThemeAsync()
        {
            try
            {
                var themeStr = await SecureStorage.GetAsync(APP_THEME_KEY);
                return themeStr != null && Enum.TryParse<AppThemes>(themeStr, out var theme) 
                    ? theme 
                    : DEFAULT_APP_THEME;
            }
            catch
            {
                return DEFAULT_APP_THEME;
            }
        }

        /// <summary>
        /// Sets the app theme
        /// </summary>
        public static async Task SetAppThemeAsync(AppThemes theme)
        {
            try
            {
                await SecureStorage.SetAsync(APP_THEME_KEY, theme.ToString());
            }
            catch
            {
                // Handle storage errors gracefully
            }
        }

        /// <summary>
        /// Saves the background color hex for the selected theme so it can be read
        /// synchronously on next launch (e.g. to tint the Android window background).
        /// </summary>
        public static void SetThemeBackgroundColor(AppThemes theme)
        {
            var hex = theme switch
            {
                AppThemes.Light  => "#FAF5FF",
                AppThemes.Forest => "#0D2818",
                AppThemes.Candy  => "#FFF0F7",
                AppThemes.Sunset => "#FFF7ED",
                AppThemes.Ocean  => "#EFF6FF",
                _                => "#0F172A",   // Dark (default)
            };
            try { Preferences.Set(APP_THEME_BG_COLOR_KEY, hex); }
            catch { }
        }

        /// <summary>
        /// Returns the background color hex that was saved for the last selected theme.
        /// Falls back to the Dark theme color if nothing has been saved yet.
        /// </summary>
        public static string GetThemeBackgroundColor()
        {
            try { return Preferences.Get(APP_THEME_BG_COLOR_KEY, "#222831"); }
            catch { return "#222831"; }
        }

        /// <summary>
        /// Gets the current app font family
        /// </summary>
        public static Task<string> GetAppFontFamilyAsync()
        {
            try
            {
                var fontFamily = Preferences.Get(APP_FONT_FAMILY_KEY, DEFAULT_FONT_FAMILY);
                return Task.FromResult(fontFamily);
            }
            catch
            {
                return Task.FromResult(DEFAULT_FONT_FAMILY);
            }
        }

        /// <summary>
        /// Sets the app font family
        /// </summary>
        public static Task SetAppFontFamilyAsync(string fontFamily)
        {
            try
            {
                Preferences.Set(APP_FONT_FAMILY_KEY, fontFamily);
            }
            catch
            {
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the saved paragraph font size
        /// </summary>
        public static Task<double> GetParagraphFontSizeAsync()
        {
            try
            {
                var size = Preferences.Get(APP_PARAGRAPH_FONT_SIZE_KEY, DEFAULT_PARAGRAPH_FONT_SIZE);
                return Task.FromResult(Math.Max(MIN_PARAGRAPH_FONT_SIZE, Math.Min(MAX_PARAGRAPH_FONT_SIZE, size)));
            }
            catch
            {
                return Task.FromResult(DEFAULT_PARAGRAPH_FONT_SIZE);
            }
        }

        /// <summary>
        /// Saves the paragraph font size
        /// </summary>
        public static Task SetParagraphFontSizeAsync(double size)
        {
            try
            {
                var clamped = Math.Max(MIN_PARAGRAPH_FONT_SIZE, Math.Min(MAX_PARAGRAPH_FONT_SIZE, size));
                Preferences.Set(APP_PARAGRAPH_FONT_SIZE_KEY, clamped);
            }
            catch
            {
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears all app settings from secure storage
        /// </summary>
        public static async Task ClearAllSettingsAsync()
        {
            try
            {
                SecureStorage.Remove(SORT_PROPERTY_NAME_KEY);
                SecureStorage.Remove(SORT_DIRECTION_KEY);
                SecureStorage.Remove(SHOW_ALREADY_READ_ITEM_KEY);
                SecureStorage.Remove(APP_THEME_KEY);
                Preferences.Remove(APP_FONT_FAMILY_KEY);
                Preferences.Remove(APP_PARAGRAPH_FONT_SIZE_KEY);
            }
            catch
            {
                // Handle storage errors gracefully
            }
        }
    }
}
