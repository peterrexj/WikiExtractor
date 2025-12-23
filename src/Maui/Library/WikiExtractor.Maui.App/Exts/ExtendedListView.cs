using Microsoft.Extensions.Logging;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.ListView.Helpers;
using System.ComponentModel;
using System.Reflection;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Exts
{
    public class ExtendedListView : SfListView
    {
        private VisualContainer? container;
        private readonly ILogger<ExtendedListView>? logger;

        public ExtendedListView()
        {
            try
            {
                // Get logger if available
                logger = ServiceHelper.GetService<ILogger<ExtendedListView>>();
                
                // Get the visual container and subscribe to property changes
                container = this.GetVisualContainer();
                if (container != null)
                {
                    container.PropertyChanged += Container_PropertyChanged;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error initializing ExtendedListView");
            }
        }

        private void Container_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.PropertyName) ||
                    e.PropertyName != "Height" ||
                    this.BindingContext == null ||
                    container == null)
                    return;

                // Use MainThread.BeginInvokeOnMainThread for MAUI
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            // Get TotalExtent using reflection (similar to Xamarin.Forms version)
                            var totalExtentProperty = container.GetType()
                                .GetRuntimeProperties()
                                .FirstOrDefault(prop => prop.Name == "TotalExtent");

                            if (totalExtentProperty != null)
                            {
                                var totalExtent = (double)(totalExtentProperty.GetValue(container) ?? 0);
                                
                                if (totalExtent > 0)
                                {
                                    // Apply minimum height constraint (similar to Xamarin.Forms version)
                                    const double MinLengthOfPictureCaption = 50; // Default minimum height
                                    if (totalExtent < MinLengthOfPictureCaption)
                                    {
                                        totalExtent = MinLengthOfPictureCaption;
                                    }

                                    // Set the ListHeight property on the binding context
                                    if (this.BindingContext is IListDynamicHeight dynamicHeightContext)
                                    {
                                        dynamicHeightContext.ListHeight = totalExtent;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error calculating dynamic height in ExtendedListView");
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error in Container_PropertyChanged");
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            // Clean up when handler is removed
            if (Handler == null && container != null)
            {
                container.PropertyChanged -= Container_PropertyChanged;
            }
        }
    }

    // Helper class for getting services
    public static class ServiceHelper
    {
        public static T? GetService<T>() where T : class
        {
            try
            {
#if WINDOWS
                return MauiWinUIApplication.Current?.Services?.GetService<T>();
#elif ANDROID
                return MauiApplication.Current?.Services?.GetService<T>();
#elif IOS || MACCATALYST
                return MauiUIApplicationDelegate.Current?.Services?.GetService<T>();
#else
                return null;
#endif
            }
            catch
            {
                return null;
            }
        }
    }
}