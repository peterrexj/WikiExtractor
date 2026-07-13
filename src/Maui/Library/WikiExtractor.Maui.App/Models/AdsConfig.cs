using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Maui.App.Models
{
    public class AdsConfig
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
    }
}
