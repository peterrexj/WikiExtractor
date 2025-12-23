// using Syncfusion.Maui.DataSource; // Temporarily disabled

using Syncfusion.Maui.DataSource;
using WikiExtractor.Maui.App.Exts; // For temporary ListSortDirection enum
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Example usage of the AppSettingsService with SecureStorage
    /// This demonstrates how to use the service instead of database-based settings
    /// </summary>
    public static class AppSettingsServiceExample
    {
        /// <summary>
        /// Example: Save and retrieve sort descriptor
        /// Usage: var sortDescriptor = await AppSettingsServiceExample.GetSortDescriptorExample();
        /// </summary>
        public static async Task<MainListSortDescriptorModel> GetSortDescriptorExample()
        {
            // Get current sort descriptor (uses SecureStorage internally)
            var currentSort = await AppSettingsService.GetSortDescriptorAsync();
            
            // Example: If no sort is set, set a default one
            if (currentSort.PropertyName == "Id")
            {
                var newSort = new MainListSortDescriptorModel 
                { 
                    PropertyName = "Name", 
                    Direction = ListSortDirection.Ascending 
                };
                await AppSettingsService.SaveSortDescriptorAsync(newSort);
                return newSort;
            }
            
            return currentSort;
        }

        /// <summary>
        /// Example: Save app theme
        /// Usage: await AppSettingsServiceExample.SetThemeExample(AppThemes.Dark);
        /// </summary>
        public static async Task SetThemeExample(AppThemes theme)
        {
            await AppSettingsService.SetAppThemeAsync(theme);
        }

        /// <summary>
        /// Example: Get app theme
        /// Usage: var theme = await AppSettingsServiceExample.GetThemeExample();
        /// </summary>
        public static async Task<AppThemes> GetThemeExample()
        {
            return await AppSettingsService.GetAppThemeAsync();
        }

        /// <summary>
        /// Example: Toggle show read items setting
        /// Usage: await AppSettingsServiceExample.ToggleShowReadItemsExample();
        /// </summary>
        public static async Task ToggleShowReadItemsExample()
        {
            var currentSetting = await AppSettingsService.GetShowAlreadyReadItemsAsync();
            await AppSettingsService.SetShowAlreadyReadItemsAsync(!currentSetting);
        }

        /// <summary>
        /// Example: Clear all settings (useful for logout or reset)
        /// Usage: await AppSettingsServiceExample.ClearAllSettingsExample();
        /// </summary>
        public static async Task ClearAllSettingsExample()
        {
            await AppSettingsService.ClearAllSettingsAsync();
        }
    }
}
