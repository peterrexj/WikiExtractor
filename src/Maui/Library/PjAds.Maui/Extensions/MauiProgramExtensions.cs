using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PjAds.Maui.Controls;
using PjAds.Maui.Models;
using PjAds.Maui.Services;

namespace PjAds.Maui.Extensions
{
    /// <summary>
    /// Extension methods for MauiProgram to configure PjAds services
    /// </summary>
    public static class MauiProgramExtensions
    {
        /// <summary>
        /// Adds PjAds services to the MAUI application
        /// </summary>
        /// <param name="builder">The MauiAppBuilder</param>
        /// <param name="configuration">Ad configuration</param>
        /// <returns>The MauiAppBuilder for chaining</returns>
        public static MauiAppBuilder UsePjAds(this MauiAppBuilder builder, AdConfiguration configuration)
        {
            // Register the configuration
            builder.Services.AddSingleton(configuration);

            // Register platform-specific services
#if ANDROID
            builder.Services.AddSingleton<IBannerAdService, Platforms.Android.BannerAdService>();
            builder.Services.AddSingleton<IInterstitialAdService, Platforms.Android.InterstitialAdService>();
#elif IOS
            builder.Services.AddSingleton<IBannerAdService, Platforms.iOS.BannerAdService>();
            builder.Services.AddSingleton<IInterstitialAdService, Platforms.iOS.InterstitialAdService>();
#else
            // For other platforms, register null implementations
            builder.Services.AddSingleton<IBannerAdService, NullBannerAdService>();
            builder.Services.AddSingleton<IInterstitialAdService, NullInterstitialAdService>();
#endif

            // Register the main ad manager
            builder.Services.AddSingleton<IAdManager, AdManager>();

            // Note: ConfigureLifecycleEvents is not available in all MAUI versions
            // Ad initialization will be handled by the individual services when first used

            return builder;
        }

        /// <summary>
        /// Adds PjAds services with a configuration factory
        /// </summary>
        /// <param name="builder">The MauiAppBuilder</param>
        /// <param name="configurationFactory">Factory function to create ad configuration</param>
        /// <returns>The MauiAppBuilder for chaining</returns>
        public static MauiAppBuilder UsePjAds(this MauiAppBuilder builder, Func<IServiceProvider, AdConfiguration> configurationFactory)
        {
            // Register the configuration factory
            builder.Services.AddSingleton(configurationFactory);

            // Register platform-specific services
#if ANDROID
            builder.Services.AddSingleton<IBannerAdService, Platforms.Android.BannerAdService>();
            builder.Services.AddSingleton<IInterstitialAdService, Platforms.Android.InterstitialAdService>();
#elif IOS
            builder.Services.AddSingleton<IBannerAdService, Platforms.iOS.BannerAdService>();
            builder.Services.AddSingleton<IInterstitialAdService, Platforms.iOS.InterstitialAdService>();
#else
            // For other platforms, register null implementations
            builder.Services.AddSingleton<IBannerAdService, NullBannerAdService>();
            builder.Services.AddSingleton<IInterstitialAdService, NullInterstitialAdService>();
#endif

            // Register the main ad manager
            builder.Services.AddSingleton<IAdManager, AdManager>();

            return builder;
        }

        /// <summary>
        /// Configures PjAds handlers for MAUI
        /// </summary>
        /// <param name="builder">The MauiAppBuilder</param>
        /// <returns>The MauiAppBuilder for chaining</returns>
        public static MauiAppBuilder ConfigurePjAdsHandlers(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                // Configure any fonts needed for ads if required
            });

            // Register the banner ad view handler
            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID || IOS
                handlers.AddHandler<BannerAdView, BannerAdViewHandler>();
#endif
            });

            return builder;
        }

#if ANDROID
        private static void InitializeAds(Android.App.Activity activity, AdConfiguration configuration)
        {
            try
            {
                Android.Gms.Ads.MobileAds.Initialize(activity.ApplicationContext);
                
                // Logger access would need to be passed in from the calling context
                Console.WriteLine("Google Mobile Ads SDK initialized for Android");
            }
            catch (Exception ex)
            {
                // Logger access would need to be passed in from the calling context
                Console.WriteLine($"Failed to initialize Google Mobile Ads SDK for Android: {ex.Message}");
            }
        }
#elif IOS
        private static void InitializeAds(AdConfiguration configuration)
        {
            try
            {
                Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: (status) =>
                {
                    // Initialization completed
                });
                
                // Note: Logger not easily accessible here, would need to be passed in
                Console.WriteLine("Google Mobile Ads SDK initialized for iOS");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Google Mobile Ads SDK for iOS: {ex.Message}");
            }
        }
#endif
    }

    /// <summary>
    /// Null implementation of IBannerAdService for unsupported platforms
    /// </summary>
    internal class NullBannerAdService : IBannerAdService
    {
        public event EventHandler<AdLoadedEventArgs>? AdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;
        public event EventHandler<AdClickedEventArgs>? AdClicked;
        public event EventHandler<AdImpressionEventArgs>? AdImpression;

        public bool IsSupported => false;

        public object CreateBannerAdView(string adUnitId, AdSize adSize = AdSize.Banner)
        {
            return new object(); // Return a dummy object to avoid null reference issues
        }

        public Task LoadBannerAdAsync(object adView, string adUnitId)
        {
            return Task.CompletedTask;
        }

        public void DestroyBannerAd(object adView)
        {
            // No-op
        }
    }

    /// <summary>
    /// Null implementation of IInterstitialAdService for unsupported platforms
    /// </summary>
    internal class NullInterstitialAdService : IInterstitialAdService
    {
        public event EventHandler<AdLoadedEventArgs>? AdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;
        public event EventHandler<InterstitialAdOpenedEventArgs>? AdOpened;
        public event EventHandler<InterstitialAdClosedEventArgs>? AdClosed;
        public event EventHandler<AdClickedEventArgs>? AdClicked;
        public event EventHandler<AdImpressionEventArgs>? AdImpression;

        public bool IsInterstitialAdLoaded => false;
        public bool IsSupported => false;

        public Task LoadInterstitialAdAsync(string adUnitId)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ShowInterstitialAdAsync()
        {
            return Task.FromResult(false);
        }
    }
}