using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Exts;
using Maui.Wiki.ViewModels;
using Syncfusion.Maui.Buttons;
using System;
using System.Threading.Tasks;
using Pj.Library;

namespace Maui.Wiki.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            try
            {
                // Get the required services from the service provider with fallback
                var themeHandler = GetThemeHandlerService();
                var errorHandlingService = GetErrorHandlingService();
                
                // Set the binding context with the required services
                BindingContext = new SettingsViewModel(themeHandler, errorHandlingService);
            }
            catch (Exception ex)
            {
                // Fallback: create ViewModel without services if they're not available
                System.Diagnostics.Debug.WriteLine($"Warning: Failed to initialize SettingsPage with services: {ex.Message}");
                BindingContext = new SettingsViewModel(null, null);
            }
        }

        private WikiExtractor.Maui.App.Services.IThemeHandler GetThemeHandlerService()
        {
            try
            {
                // Try ServiceLocator first
                var service = ServiceLocator.GetService<WikiExtractor.Maui.App.Services.IThemeHandler>();
                if (service != null) return service;
                
                // Try CustomServices fallback
                var customService = CustomServices.ThemeHandler;
                if (customService != null) return customService;
                
                // Create fallback instance
                return new WikiExtractor.Maui.App.Services.ThemeHandler();
            }
            catch
            {
                return new WikiExtractor.Maui.App.Services.ThemeHandler();
            }
        }

        private IErrorHandlingService GetErrorHandlingService()
        {
            try
            {
                return ServiceLocator.GetService<IErrorHandlingService>();
            }
            catch
            {
                return null; // SettingsViewModel should handle null gracefully
            }
        }

        private async void HideViewedSwitch_StateChanged(object sender, SwitchStateChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    // The ViewModel property setter already handles saving the setting
                    // This event handler is mainly for any additional UI feedback if needed
                    if (BindingContext is SettingsViewModel viewModel)
                    {
                        // The binding will automatically update the ViewModel property
                        // which in turn saves the setting via SettingsHelper
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors - you might want to inject an error handling service
                    System.Diagnostics.Debug.WriteLine($"Error in HideViewedSwitch_StateChanged: {ex.Message}");
                }
            });
        }

        private async void SfSegmentSortOrder_SelectionChanged(object sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    // The ViewModel property setter already handles saving the setting
                    // This event handler is mainly for any additional processing if needed
                    if (BindingContext is SettingsViewModel viewModel)
                    {
                        // The binding will automatically update the ViewModel property
                        // which in turn saves the setting via SettingsHelper
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    System.Diagnostics.Debug.WriteLine($"Error in SfSegmentSortOrder_SelectionChanged: {ex.Message}");
                }
            });
        }
    }
}