using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;
using Syncfusion.Maui.Core.Hosting;
using System.Diagnostics;
using WikiExtractor.Process;
using WikiExtractor.Exts;
using WikiExtractor.Maui.App.Exts;
using PjAds.Maui.Extensions;
using PjAds.Maui.Models;
using WikiExtractor.Maui.App.Views;
using WikiExtractor.Maui.App.Models;
using Maui.Wiki.Views;

namespace Maui.Wiki
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //// Set up global exception handling at the earliest possible point
            //AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            //TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            IAppInformation appInfo;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("PARCHM.TTF", "Parmch");
                    fonts.AddFont("Font Awesome 5 Free-Solid-900.otf", "FontAwesome");
                    fonts.AddFont("CALIBRI.TTF", "Calibri");
                    fonts.AddFont("Lato-Regular.ttf", "Lato");
                    fonts.AddFont("Nunito-Regular.ttf", "Nunito");
                    fonts.AddFont("Pacifico-Regular.ttf", "Pacifico");
                    fonts.AddFont("Raleway-Regular.ttf", "Raleway");
                });
                // Configure PjAds for Popes app
//                .UsePjAds(new AdConfiguration
//                {
//                    // Test ad unit IDs - replace with production IDs for release
//#if DEBUG
//                    ApplicationId = DeviceInfo.Platform == DevicePlatform.Android
//                        ? "ca-app-pub-3940256099942544~3347511713"  // Test Android App ID
//                        : "ca-app-pub-3940256099942544~1458002511", // Test iOS App ID
//                    BannerAdUnitId = DeviceInfo.Platform == DevicePlatform.Android
//                        ? "ca-app-pub-3940256099942544/6300978111"  // Test Android Banner
//                        : "ca-app-pub-3940256099942544/2934735716", // Test iOS Banner
//                    InterstitialAdUnitId = DeviceInfo.Platform == DevicePlatform.Android
//                        ? "ca-app-pub-3940256099942544/1033173712"  // Test Android Interstitial
//                        : "ca-app-pub-3940256099942544/4411468910", // Test iOS Interstitial
//#else
//                    // Production ad unit IDs for Popes app - replace with actual IDs
//                    ApplicationId = DeviceInfo.Platform == DevicePlatform.Android
//                        ? "ca-app-pub-YOUR_ANDROID_APP_ID"
//                        : "ca-app-pub-YOUR_IOS_APP_ID",
//                    BannerAdUnitId = DeviceInfo.Platform == DevicePlatform.Android
//                        ? "ca-app-pub-YOUR_ANDROID_BANNER_ID"
//                        : "ca-app-pub-YOUR_IOS_BANNER_ID",
//                    InterstitialAdUnitId = DeviceInfo.Platform == DevicePlatform.Android
//                        ? "ca-app-pub-YOUR_ANDROID_INTERSTITIAL_ID"
//                        : "ca-app-pub-YOUR_IOS_INTERSTITIAL_ID",
//#endif
//                    FirstInterstitialAdThreshold = 1, // Show first interstitial after 1 interaction
//                    SubsequentInterstitialAdThreshold = 3, // Show subsequent interstitials every 3 interactions
//                    TestMode = true // Set to false for production
//                })
//                .ConfigurePjAdsHandlers();

            // Register services for dependency injection
            // DatabaseService is static, so we don't need to register it
            builder.Services.AddSingleton<WikiAppController>();
            
            // Register common services
            builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
            builder.Services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<INoAdsService, NoAdsService>();
            
            // Register theme services
            builder.Services.AddSingleton<WikiExtractor.Maui.App.Services.IThemeHandler, WikiExtractor.Maui.App.Services.ThemeHandler>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.ViewModels.StatsPageViewModel>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.StatsPage>();
            builder.Services.AddSingleton<SplashPage>();
            
            // Register the App class for dependency injection
            builder.Services.AddSingleton<App>();
            
            // Register view models
            builder.Services.AddSingleton<WikiExtractor.Maui.App.ViewModels.PersonaListViewModel>();
            builder.Services.AddSingleton<WikiExtractor.Maui.App.ViewModels.PersonaDetailViewModel>();
            
            // Register pages
            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.PersonaDetailPage>();
            
            // Register quiz-related view models and pages
            builder.Services.AddTransient<WikiExtractor.Maui.App.ViewModels.QuizPageViewModel>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.ViewModels.QuizResultsPageViewModel>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.QuizPage>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.QuizResultsPage>();

            // Register platform-specific services
#if ANDROID
            appInfo = new Maui.Wiki.Platforms.Android.DependencyInjection.AppInformation();
            builder.Services.AddSingleton<IAppInformation>(appInfo);
            builder.Services.AddSingleton<IAppEnvironment, WikiExtractor.Maui.App.Platforms.Android.DependencyInjection.AppEnvironment>();
            builder.Services.AddSingleton<IImageService, WikiExtractor.Maui.App.Platforms.Android.DependencyInjection.ImageService>();
            builder.Services.AddSingleton<ILocalStorage, WikiExtractor.Maui.App.Platforms.Android.DependencyInjection.LocalStorage>();
#elif IOS
            appInfo = new Maui.Wiki.Platforms.iOS.DependencyInjection.AppInformation();
            builder.Services.AddSingleton<IAppInformation>(appInfo);
            builder.Services.AddSingleton<IAppEnvironment, WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection.AppEnvironment>();
            builder.Services.AddSingleton<IImageService, WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection.ImageService>();
            builder.Services.AddSingleton<ILocalStorage, WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection.LocalStorage>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var adsConfig = new AdsConfig
            {
                ApplicationId = appInfo.AdsAppId,
                BannerAdUnitId = appInfo.AdsBannerId,
                QuizBannerAdUnitId = appInfo.AdsQuizBannerId,
                InterstitialAdUnitId = appInfo.AdsInterstitialId,
                AdsEnabled = true
            };
            builder.Services.AddSingleton(adsConfig);

            var adConfig = new AdConfiguration
            {
                ApplicationId = adsConfig.ApplicationId,
                BannerAdUnitId = appInfo.AdsBannerId,
                QuizBannerAdUnitId = appInfo.AdsQuizBannerId,
                InterstitialAdUnitId = appInfo.AdsInterstitialId,
                AdsEnabled = true,
                TestMode =
#if DEBUG
            true,
#else
            false,
#endif
                FirstInterstitialAdThreshold = 1,
                SubsequentInterstitialAdThreshold = 3
            };
            builder.UsePjAds(adConfig).ConfigurePjAdsHandlers();

            return builder.Build();
        }

        // Global unhandled exception handler for the AppDomain
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            HandleGlobalException(exception, "AppDomain Unhandled Exception", e.IsTerminating);
        }

        // Global unhandled exception handler for async Tasks
        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleGlobalException(e.Exception, "Unobserved Task Exception", false);
            e.SetObserved(); // Prevent the exception from crashing the app
        }

        // Centralized exception handling method
        private static void HandleGlobalException(Exception ex, string source, bool isTerminating)
        {
            if (ex == null) return;

            try
            {
                // Log to debug output
                Debug.WriteLine($"[GLOBAL EXCEPTION] [{source}] {ex.GetType().Name}: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);

                // Log to app's exception handler
                ExceptionHandler.CaptureException(ex, source, $"IsTerminating: {isTerminating}");

                // Additional platform-specific handling for iOS
#if IOS
                // On iOS, ensure we're not throwing exceptions that could reach the native layer
                if (isTerminating)
                {
                    Debug.WriteLine("Critical exception occurred. Application may terminate.");
                }
#endif
            }
            catch (Exception logEx)
            {
                // Last resort if exception handling itself fails
                Debug.WriteLine($"Error in exception handler: {logEx.Message}");
            }
        }

        /// <summary>
        /// Pre-initialize database access to ensure it's ready before UI components try to use it
        /// </summary>
        private static void InitializeDatabases()
        {
            try
            {
                // Force initialization of databases early to catch any issues
                Debug.WriteLine("Pre-initializing databases...");
                
                // Access the database properties to trigger initialization
                var appDb = WikiExtractor.Maui.App.Repository.DatabaseService.AppDatabase;
                var userDb = WikiExtractor.Maui.App.Repository.DatabaseService.UserStoreDatabase;
                
                // Check if databases are accessible
                if (appDb != null)
                {
                    Debug.WriteLine("AppDatabase initialized successfully");
                }
                
                if (userDb != null)
                {
                    Debug.WriteLine("UserStoreDatabase initialized successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing databases: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Debug.WriteLine(ex.InnerException.StackTrace);
                }
            }
        }

        /// <summary>
        /// Initialize ConfigData early in the application lifecycle
        /// </summary>
        private static void InitializeConfigData(IServiceProvider services)
        {
            try
            {
                var appInformation = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<IAppInformation>(services);
                ConfigData.LocalStorageCacheFolderPath = appInformation.ImageCacheFolder;
                
                Debug.WriteLine($"ConfigData.LocalStorageCacheFolderPath initialized: {ConfigData.LocalStorageCacheFolderPath}");
                
                if (string.IsNullOrEmpty(ConfigData.LocalStorageCacheFolderPath))
                {
                    Debug.WriteLine("WARNING: LocalStorageCacheFolderPath is empty!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing ConfigData: {ex.Message}");
                ExceptionHandler.CaptureException(ex, "ConfigData initialization failed");
            }
        }
    }
}
