using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Maui.App.Services;
using Maui.Wiki.Services;
using WikiExtractor.Process;
using WikiExtractor.Exts;

namespace Maui.Wiki
{
    public partial class App : Application
    {
        private readonly IAppInformation _appInformation;

        public App(IAppInformation appInformation)
        {
            _appInformation = appInformation;
            
            try
            {
                InitializeComponent();

                // Register Syncfusion license
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH1ccXVSQ2dcV0Z0W0A=");

                // Set up comprehensive exception handling
                SetupExceptionHandling();

                // Initialize LocalStorageCacheFolderPath using dependency injection
                ConfigData.LocalStorageCacheFolderPath = _appInformation.ImageCacheFolder;

                // Initialize theme - use fallback approach since ServiceLocator may not be initialized yet
                //CustomServices.ThemeHandler.LoadDefaultStyle();
                InitializeTheme();
                
                // Initialize database and app controller with proper error handling
                InitializeAppController();
            }
            catch (Exception ex)
            {
                // Catch any exceptions during app initialization
                HandleException(ex, "App initialization failed");
            }
        }

        private void InitializeAppController()
        {
            try
            {
                var wikiAppController = new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);
                var flyoutItems = wikiAppController.AppMenuItems();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Failed to initialize WikiAppController");
            }
        }

        private void SetupExceptionHandling()
        {
            // Handle exceptions in the .NET AppDomain
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                HandleException(exception, "AppDomain Unhandled Exception");
            };

            // Handle exceptions in async code
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                HandleException(args.Exception, "Unobserved Task Exception");
                args.SetObserved(); // Prevent the exception from crashing the app
            };

            // Note: MAUI doesn't have DispatchUnhandledException like Xamarin.Forms did
            // UI exceptions are handled by AppDomain.CurrentDomain.UnhandledException
            // and TaskScheduler.UnobservedTaskException
        }

        private void HandleException(Exception ex, string source)
        {
            if (ex == null) return;

            // Log to debug output
            Debug.WriteLine($"[{source}] {ex.GetType().Name}: {ex.Message}");
            Debug.WriteLine(ex.StackTrace);

            // Log to app's exception handler
            WikiExtractor.Maui.App.Exts.ExceptionHandler.CaptureException(ex, source);

            // You could also log to a file, send to a remote service, etc.
        }

        private void InitializeTheme()
        {
            try
            {
                // Try to get the theme handler from the service locator first
                var themeHandler = CustomServices.ThemeHandler;
                if (themeHandler != null)
                {
                    themeHandler.LoadDefaultStyle();
                    return;
                }

                // Fallback: Create a direct instance if service locator is not ready
                // This can happen during app initialization before ServiceLocator.Initialize() is called
                var fallbackThemeHandler = new WikiExtractor.Maui.App.Services.ThemeHandler();
                fallbackThemeHandler.LoadDefaultStyle();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Theme initialization failed");
                
                // If theme loading fails completely, continue without themes
                // The app should still be functional even without custom themes
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                return new Window(new AppShell());
            }
            catch (Exception ex)
            {
                HandleException(ex, "Window creation failed");
                
                // Create a simple fallback window with an error message
                var fallbackPage = new ContentPage
                {
                    Content = new VerticalStackLayout
                    {
                        Children =
                        {
                            new Label { Text = "The application encountered a problem.", HorizontalOptions = LayoutOptions.Center },
                            new Label { Text = "Please restart the application.", HorizontalOptions = LayoutOptions.Center }
                        },
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                
                return new Window(fallbackPage);
            }
        }
    }
}
