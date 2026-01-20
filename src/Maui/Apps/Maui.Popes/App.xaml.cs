using WikiExtractor.Maui.App.Services;
using WikiExtractor.Exts;
using Pj.Library;

namespace Maui.Wiki
{
    public partial class App : Application
    {
        private readonly IThemeHandler _themeHandler;

        public App(IServiceProvider serviceProvider, IThemeHandler themeHandler)
        {
            try
            {
                InitializeComponent();

                _themeHandler = themeHandler;
                ServiceLocator.ServiceProvider = serviceProvider;

                AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
                TaskScheduler.UnobservedTaskException += HandleTaskSchedulerException;

                // Register Syncfusion license
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWX5fcXZVQ2ZYVE1wVkpWYEs=");

                // Initialize LocalStorageCacheFolderPath using dependency injection

                var cacheFolder = SharedServiceCore.AppInformation?.ImageCacheFolder ?? "";
                if (cacheFolder.HasValue())
                {
                    ConfigData.LocalStorageCacheFolderPath = cacheFolder;
                }

                // INITIALIZE THEME
                // We call the Async version. Even though we aren't awaiting it here, 
                // the ThemeHandler internally uses MainThread.InvokeOnMainThreadAsync 
                // to ensure it applies as soon as the UI loop is ready.
                _themeHandler.LoadDefaultStyle();
                _themeHandler.InitializeQuizColorsBackground();

                //// Initialize database and app controller with proper error handling
                ////InitializeAppController();

            }
            catch (Exception ex)
            {
                // Catch any exceptions during app initialization
                LogException(ex, "App initialization failed");
            }
        }

        //private void InitializeAppController()
        //{
        //    try
        //    {
        //        var wikiAppController = new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);
        //        var flyoutItems = wikiAppController.AppMenuItems();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(ex, "Failed to initialize WikiAppController");
        //    }
        //}

        //private void SetupExceptionHandling()
        //{
        //    // Handle exceptions in the .NET AppDomain
        //    AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        //    {
        //        var exception = args.ExceptionObject as Exception;
        //        HandleException(exception, "AppDomain Unhandled Exception");
        //    };

        //    // Handle exceptions in async code
        //    TaskScheduler.UnobservedTaskException += (sender, args) =>
        //    {
        //        HandleException(args.Exception, "Unobserved Task Exception");
        //        args.SetObserved(); // Prevent the exception from crashing the app
        //    };

        //    // Note: MAUI doesn't have DispatchUnhandledException like Xamarin.Forms did
        //    // UI exceptions are handled by AppDomain.CurrentDomain.UnhandledException
        //    // and TaskScheduler.UnobservedTaskException
        //}

        //private void HandleException(Exception ex, string source)
        //{
        //    if (ex == null) return;

        //    // Log to debug output
        //    Debug.WriteLine($"[{source}] {ex.GetType().Name}: {ex.Message}");
        //    Debug.WriteLine(ex.StackTrace);

        //    // Log to app's exception handler
        //    WikiExtractor.Maui.App.Exts.ExceptionHandler.CaptureException(ex, source);

        //    // You could also log to a file, send to a remote service, etc.
        //}

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject as Exception, "AppDomain Unhandled Exception");
        }

        private void HandleTaskSchedulerException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception, "TaskScheduler Unobserved Task Exception");
            e.SetObserved();
        }

        private void LogException(Exception? exception, string source)
        {
            if (exception == null) return;

            //var errorHandlingService = ServiceLocator.GetService<IErrorHandlingService>();
            //errorHandlingService?.HandleException(exception, "An unhandled exception occurred.");
        }

        

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                return new Window(new AppShell());
            }
            catch (Exception ex)
            {
                LogException(ex, "Window creation failed");

                // Create a simple fallback window with an error message
                return new Window(new ContentPage
                {
                    Content = new Label { Text = "Critical Error. Please restart." }
                });
            }
        }
    }
}
