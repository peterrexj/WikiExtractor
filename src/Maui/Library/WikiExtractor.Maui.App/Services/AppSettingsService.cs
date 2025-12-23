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

        // Default Values
        private const string DEFAULT_SORT_PROPERTY = "Id";
        private const string DEFAULT_SORT_DIRECTION = "Ascending";
        private const bool DEFAULT_SHOW_READ_ITEMS = true;
        private const AppThemes DEFAULT_APP_THEME = AppThemes.Light;

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
            }
            catch
            {
                // Handle storage errors gracefully
            }
        }
    }
}
