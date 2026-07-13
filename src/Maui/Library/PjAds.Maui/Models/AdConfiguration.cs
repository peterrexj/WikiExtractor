namespace PjAds.Maui.Models
{
    /// <summary>
    /// Configuration settings for ads
    /// </summary>
    public class AdConfiguration
    {
        /// <summary>
        /// Google AdMob Application ID
        /// </summary>
        public string ApplicationId { get; set; } = string.Empty;

        /// <summary>
        /// Banner ad unit ID
        /// </summary>
        public string BannerAdUnitId { get; set; } = string.Empty;

        /// <summary>
        /// Quiz banner ad unit ID (optional, for different banner placements)
        /// </summary>
        public string? QuizBannerAdUnitId { get; set; }

        /// <summary>
        /// Interstitial ad unit ID
        /// </summary>
        public string InterstitialAdUnitId { get; set; } = string.Empty;

        /// <summary>
        /// Whether ads are enabled
        /// </summary>
        public bool AdsEnabled { get; set; } = true;

        /// <summary>
        /// Whether interstitial ads are enabled (independent of banner ads)
        /// </summary>
        public bool InterstitialAdsEnabled { get; set; } = true;

        /// <summary>
        /// Test mode - uses test ad unit IDs
        /// </summary>
        public bool TestMode { get; set; } = false;

        /// <summary>
        /// Number of user interactions before showing first interstitial ad
        /// </summary>
        public int FirstInterstitialAdThreshold { get; set; } = 1;

        /// <summary>
        /// Number of user interactions between subsequent interstitial ads
        /// </summary>
        public int SubsequentInterstitialAdThreshold { get; set; } = 5;

        /// <summary>
        /// Validates the configuration
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ApplicationId) &&
                   !string.IsNullOrWhiteSpace(BannerAdUnitId) &&
                   !string.IsNullOrWhiteSpace(InterstitialAdUnitId);
        }

        /// <summary>
        /// Gets test configuration for development
        /// </summary>
        public static AdConfiguration GetTestConfiguration()
        {
            return new AdConfiguration
            {
                ApplicationId = "ca-app-pub-3940256099942544~3347511713", // Test app ID
                BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111", // Test banner ID
                InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712", // Test interstitial ID
                TestMode = true,
                AdsEnabled = true,
                FirstInterstitialAdThreshold = 1,
                SubsequentInterstitialAdThreshold = 3
            };
        }
    }
}