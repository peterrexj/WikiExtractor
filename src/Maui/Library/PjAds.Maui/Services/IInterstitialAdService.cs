using PjAds.Maui.Models;

namespace PjAds.Maui.Services
{
    /// <summary>
    /// Interface for interstitial ad service
    /// </summary>
    public interface IInterstitialAdService
    {
        /// <summary>
        /// Event fired when an interstitial ad is loaded
        /// </summary>
        event EventHandler<AdLoadedEventArgs>? AdLoaded;

        /// <summary>
        /// Event fired when an interstitial ad fails to load
        /// </summary>
        event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;

        /// <summary>
        /// Event fired when an interstitial ad is opened/shown
        /// </summary>
        event EventHandler<InterstitialAdOpenedEventArgs>? AdOpened;

        /// <summary>
        /// Event fired when an interstitial ad is closed
        /// </summary>
        event EventHandler<InterstitialAdClosedEventArgs>? AdClosed;

        /// <summary>
        /// Event fired when an interstitial ad is clicked
        /// </summary>
        event EventHandler<AdClickedEventArgs>? AdClicked;

        /// <summary>
        /// Event fired when an interstitial ad impression is recorded
        /// </summary>
        event EventHandler<AdImpressionEventArgs>? AdImpression;

        /// <summary>
        /// Loads an interstitial ad
        /// </summary>
        /// <param name="adUnitId">The ad unit ID</param>
        Task LoadInterstitialAdAsync(string adUnitId);

        /// <summary>
        /// Shows the loaded interstitial ad
        /// </summary>
        /// <returns>True if ad was shown, false if no ad was loaded</returns>
        Task<bool> ShowInterstitialAdAsync();

        /// <summary>
        /// Gets whether an interstitial ad is loaded and ready to show
        /// </summary>
        bool IsInterstitialAdLoaded { get; }

        /// <summary>
        /// Gets whether interstitial ads are supported on this platform
        /// </summary>
        bool IsSupported { get; }
    }
}