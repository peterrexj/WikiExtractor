using PjAds.Maui.Models;

namespace PjAds.Maui.Services
{
    /// <summary>
    /// Main interface for managing ads across the application
    /// </summary>
    public interface IAdManager
    {
        /// <summary>
        /// Gets the current ad configuration
        /// </summary>
        AdConfiguration Configuration { get; }

        /// <summary>
        /// Gets the banner ad service
        /// </summary>
        IBannerAdService BannerAdService { get; }

        /// <summary>
        /// Gets the interstitial ad service
        /// </summary>
        IInterstitialAdService InterstitialAdService { get; }

        /// <summary>
        /// Initializes the ad manager with configuration
        /// </summary>
        /// <param name="configuration">Ad configuration</param>
        Task InitializeAsync(AdConfiguration configuration);

        /// <summary>
        /// Gets whether ads are enabled and properly configured
        /// </summary>
        bool IsAdsEnabled { get; }

        /// <summary>
        /// Gets whether the platform supports ads
        /// </summary>
        bool IsPlatformSupported { get; }

        /// <summary>
        /// Records a user interaction for interstitial ad frequency management
        /// </summary>
        void RecordUserInteraction();

        /// <summary>
        /// Tracks a user interaction for interstitial ad frequency management (alias for RecordUserInteraction)
        /// </summary>
        void TrackUserInteraction();

        /// <summary>
        /// Gets whether an interstitial ad should be shown based on user interactions
        /// </summary>
        bool ShouldShowInterstitialAd();

        /// <summary>
        /// Shows an interstitial ad if one should be shown and is loaded
        /// </summary>
        Task<bool> TryShowInterstitialAdAsync();

        /// <summary>
        /// Preloads an interstitial ad for later display
        /// </summary>
        Task PreloadInterstitialAdAsync();

        /// <summary>
        /// Creates a banner ad view for the specified type
        /// </summary>
        /// <param name="bannerType">Type of banner (regular or quiz)</param>
        /// <param name="adSize">Size of the banner ad</param>
        object? CreateBannerAdView(BannerType bannerType = BannerType.Regular, AdSize adSize = AdSize.Banner);
    }

    /// <summary>
    /// Types of banner ads
    /// </summary>
    public enum BannerType
    {
        /// <summary>
        /// Regular banner ad
        /// </summary>
        Regular,
        
        /// <summary>
        /// Quiz-specific banner ad
        /// </summary>
        Quiz
    }
}