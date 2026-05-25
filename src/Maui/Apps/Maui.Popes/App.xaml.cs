using WikiExtractor.Maui.App.Services;
using WikiExtractor.Exts;
using Pj.Library;
using Maui.Wiki.Views;

namespace Maui.Wiki
{
    public partial class App : Application
    {
        private readonly IThemeHandler _themeHandler;
        private readonly SplashPage _splashPage;

        public App(IServiceProvider serviceProvider, IThemeHandler themeHandler, SplashPage splashPage)
        {
            try
            {
                InitializeComponent();

                _themeHandler = themeHandler;
                _splashPage = splashPage;
                ServiceLocator.ServiceProvider = serviceProvider;

                AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
                TaskScheduler.UnobservedTaskException += HandleTaskSchedulerException;

                // Register Syncfusion license
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXpfd3RQR2VZUUFwWERWYEo=");

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

                // INITIALIZE FACT CACHE
                // Pre-load quiz facts in background - non-blocking to prevent ANR
                FactCacheService.Instance.Initialize();
                // Don't block - let cache populate in background

                // INITIALIZE DATABASE AND APP CONTROLLER
                // Warm up database access in background to prevent ANR on first list page load
                InitializeAppControllerAsync();

            }
            catch (Exception ex)
            {
                // Catch any exceptions during app initialization
                LogException(ex, "App initialization failed");
            }
        }

        private void InitializeAppControllerAsync()
        {
            // Warm up database access on background thread - non-blocking
            Task.Run(async () =>
            {
                try
                {
                    // Touch the database to initialize connections
                    _ = SharedServices.WikiAppController.AppMenuItems();
                    
                    // Also ensure fact cache is populated (non-blocking)
                    await FactCacheService.Instance.WaitForInitializationAsync(5000);
                }
                catch (Exception ex)
                {
                    LogException(ex, "Background initialization failed");
                }
            });
        }

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
        //    WikiExtractor.Maui.App.Services.ExceptionHandler.CaptureException(ex, source);

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
                System.Diagnostics.Debug.WriteLine("🪟 [App] CreateWindow START");
                Console.WriteLine("🪟 [App] CreateWindow START");

                var window = new Window(_splashPage);
                System.Diagnostics.Debug.WriteLine("✅ [App] Window created with SplashPage");
                Console.WriteLine("✅ [App] Window created with SplashPage");

                return window;
            }
            catch (Exception ex)
            {
                // Log the full exception with all inner exceptions
                System.Diagnostics.Debug.WriteLine("❌❌❌ [App.CreateWindow] CRITICAL EXCEPTION ❌❌❌");
                Console.WriteLine("❌❌❌ [App.CreateWindow] CRITICAL EXCEPTION ❌❌❌");
                
                var currentEx = ex;
                var depth = 0;
                while (currentEx != null)
                {
                    var prefix = depth == 0 ? "OUTER" : $"INNER-{depth}";
                    System.Diagnostics.Debug.WriteLine($"❌ [{prefix}] Exception Type: {currentEx.GetType().FullName}");
                    System.Diagnostics.Debug.WriteLine($"❌ [{prefix}] Message: {currentEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"❌ [{prefix}] StackTrace: {currentEx.StackTrace}");
                    Console.WriteLine($"❌ [{prefix}] Exception Type: {currentEx.GetType().FullName}");
                    Console.WriteLine($"❌ [{prefix}] Message: {currentEx.Message}");
                    Console.WriteLine($"❌ [{prefix}] StackTrace: {currentEx.StackTrace}");
                    
                    // Check for TargetInvocationException which wraps the real exception
                    if (currentEx is System.Reflection.TargetInvocationException)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ [{prefix}] This is a TargetInvocationException - checking InnerException");
                        Console.WriteLine($"❌ [{prefix}] This is a TargetInvocationException - checking InnerException");
                    }
                    
                    currentEx = currentEx.InnerException;
                    depth++;
                }
                
                System.Diagnostics.Debug.WriteLine("❌❌❌ [App.CreateWindow] END EXCEPTION DETAILS ❌❌❌");
                Console.WriteLine("❌❌❌ [App.CreateWindow] END EXCEPTION DETAILS ❌❌❌");
                
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
