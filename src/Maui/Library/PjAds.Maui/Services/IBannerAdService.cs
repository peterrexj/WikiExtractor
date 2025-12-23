using PjAds.Maui.Models;

namespace PjAds.Maui.Services
{
    /// <summary>
    /// Interface for banner ad service
    /// </summary>
    public interface IBannerAdService
    {
        /// <summary>
        /// Event fired when a banner ad is loaded
        /// </summary>
        event EventHandler<AdLoadedEventArgs>? AdLoaded;

        /// <summary>
        /// Event fired when a banner ad fails to load
        /// </summary>
        event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;

        /// <summary>
        /// Event fired when a banner ad is clicked
        /// </summary>
        event EventHandler<AdClickedEventArgs>? AdClicked;

        /// <summary>
        /// Event fired when a banner ad impression is recorded
        /// </summary>
        event EventHandler<AdImpressionEventArgs>? AdImpression;

        /// <summary>
        /// Creates a banner ad view
        /// </summary>
        /// <param name="adUnitId">The ad unit ID</param>
        /// <param name="adSize">The ad size</param>
        /// <returns>Platform-specific banner ad view</returns>
        object CreateBannerAdView(string adUnitId, AdSize adSize = AdSize.Banner);

        /// <summary>
        /// Loads a banner ad
        /// </summary>
        /// <param name="adView">The banner ad view</param>
        /// <param name="adUnitId">The ad unit ID</param>
        Task LoadBannerAdAsync(object adView, string adUnitId);

        /// <summary>
        /// Destroys a banner ad view
        /// </summary>
        /// <param name="adView">The banner ad view to destroy</param>
        void DestroyBannerAd(object adView);

        /// <summary>
        /// Gets whether banner ads are supported on this platform
        /// </summary>
        bool IsSupported { get; }
    }
}