using PjAds.Maui.Models;
using Microsoft.Extensions.Logging;

namespace PjAds.Maui.Services
{
    /// <summary>
    /// Main implementation of the ad manager
    /// </summary>
    public class AdManager : IAdManager
    {
        private readonly ILogger<AdManager>? _logger;
        private AdConfiguration? _configuration;
        private int _userInteractionCount = 0;
        private bool _hasShownFirstInterstitial = false;

        public AdConfiguration Configuration => _configuration ?? new AdConfiguration();
        public IBannerAdService BannerAdService { get; }
        public IInterstitialAdService InterstitialAdService { get; }

        public bool IsAdsEnabled => _configuration?.AdsEnabled == true && _configuration.IsValid() && IsPlatformSupported;
        public bool IsPlatformSupported => BannerAdService.IsSupported && InterstitialAdService.IsSupported;

        public AdManager(
            IBannerAdService bannerAdService,
            IInterstitialAdService interstitialAdService,
            ILogger<AdManager>? logger = null)
        {
            BannerAdService = bannerAdService ?? throw new ArgumentNullException(nameof(bannerAdService));
            InterstitialAdService = interstitialAdService ?? throw new ArgumentNullException(nameof(interstitialAdService));
            _logger = logger;
        }

        public async Task InitializeAsync(AdConfiguration configuration)
        {
            try
            {
                _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

                if (!configuration.IsValid())
                {
                    throw new ArgumentException("Invalid ad configuration provided", nameof(configuration));
                }

                _logger?.LogInformation("Initializing AdManager with configuration for app ID: {ApplicationId}", configuration.ApplicationId);

                // Initialize Google Mobile Ads SDK
                await InitializeMobileAdsSDK();

                // Preload first interstitial ad
                if (IsAdsEnabled)
                {
                    await PreloadInterstitialAdAsync();
                }

                _logger?.LogInformation("AdManager initialized successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to initialize AdManager");
                throw;
            }
        }

        public void RecordUserInteraction()
        {
            if (!IsAdsEnabled) return;

            _userInteractionCount++;
            _logger?.LogDebug("User interaction recorded. Count: {Count}", _userInteractionCount);
        }

        public void TrackUserInteraction()
        {
            RecordUserInteraction();
        }

        public bool ShouldShowInterstitialAd()
        {
            if (!IsAdsEnabled || !InterstitialAdService.IsInterstitialAdLoaded)
                return false;

            var threshold = _hasShownFirstInterstitial 
                ? Configuration.SubsequentInterstitialAdThreshold 
                : Configuration.FirstInterstitialAdThreshold;

            var shouldShow = _userInteractionCount >= threshold && (_userInteractionCount % threshold == 0);
            
            _logger?.LogDebug("Should show interstitial ad: {ShouldShow} (Count: {Count}, Threshold: {Threshold})", 
                shouldShow, _userInteractionCount, threshold);

            return shouldShow;
        }

        public async Task<bool> TryShowInterstitialAdAsync()
        {
            try
            {
                if (!ShouldShowInterstitialAd())
                    return false;

                var shown = await InterstitialAdService.ShowInterstitialAdAsync();
                if (shown)
                {
                    _hasShownFirstInterstitial = true;
                    _logger?.LogDebug("Interstitial ad shown successfully");
                    
                    // Preload next interstitial ad
                    _ = Task.Run(PreloadInterstitialAdAsync);
                }

                return shown;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to show interstitial ad");
                return false;
            }
        }

        public async Task PreloadInterstitialAdAsync()
        {
            try
            {
                if (!IsAdsEnabled) return;

                await InterstitialAdService.LoadInterstitialAdAsync(Configuration.InterstitialAdUnitId);
                _logger?.LogDebug("Interstitial ad preloaded");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to preload interstitial ad");
            }
        }

        public object? CreateBannerAdView(BannerType bannerType = BannerType.Regular, AdSize adSize = AdSize.Banner)
        {
            try
            {
                if (!IsAdsEnabled) return null;

                var adUnitId = bannerType switch
                {
                    BannerType.Quiz when !string.IsNullOrEmpty(Configuration.QuizBannerAdUnitId) => Configuration.QuizBannerAdUnitId,
                    _ => Configuration.BannerAdUnitId
                };

                var bannerView = BannerAdService.CreateBannerAdView(adUnitId, adSize);
                
                // Auto-load the banner ad
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await BannerAdService.LoadBannerAdAsync(bannerView, adUnitId);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed to auto-load banner ad");
                    }
                });

                return bannerView;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create banner ad view");
                return null;
            }
        }

        private Task InitializeMobileAdsSDK()
        {
            // MobileAds.Initialize is called in MainActivity.OnCreate (Android) and AppDelegate (iOS)
            // before any ad loads — nothing to do here.
            _logger?.LogDebug("Mobile Ads SDK initialization delegated to platform entry point");
            return Task.CompletedTask;
        }
    }
}