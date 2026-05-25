using WikiExtractor.Maui.App.Services;
using WikiExtractor.Exts;
using Pj.Library;
using Maui.Countries.Views;

namespace Maui.Countries
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

                var cacheFolder = SharedServiceCore.AppInformation?.ImageCacheFolder ?? "";
                if (cacheFolder.HasValue())
                {
                    ConfigData.LocalStorageCacheFolderPath = cacheFolder;
                }

                _themeHandler.LoadDefaultStyle();
                _themeHandler.InitializeQuizColorsBackground();

                FactCacheService.Instance.Initialize();

                InitializeAppControllerAsync();
            }
            catch (Exception ex)
            {
                LogException(ex, "App initialization failed");
            }
        }

        private void InitializeAppControllerAsync()
        {
            Task.Run(async () =>
            {
                try
                {
                    _ = SharedServices.WikiAppController.AppMenuItems();
                    await FactCacheService.Instance.WaitForInitializationAsync(5000);
                }
                catch (Exception ex)
                {
                    LogException(ex, "Background initialization failed");
                }
            });
        }

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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                var window = new Window(_splashPage);
                return window;
            }
            catch (Exception ex)
            {
                LogException(ex, "Window creation failed");
                return new Window(new ContentPage
                {
                    Content = new Label { Text = "Critical Error. Please restart." }
                });
            }
        }
    }
}
