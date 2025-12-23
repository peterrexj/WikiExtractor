#if ANDROID
using Android.Content;
using Android.Gms.Ads;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using Microsoft.Extensions.Logging;

namespace PjAds.Maui.Platforms.Android
{
    /// <summary>
    /// Android implementation of banner ad service using Google Mobile Ads SDK
    /// </summary>
    public class BannerAdService : IBannerAdService
    {
        private readonly ILogger<BannerAdService>? _logger;

        public event EventHandler<AdLoadedEventArgs>? AdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;
        public event EventHandler<AdClickedEventArgs>? AdClicked;
        public event EventHandler<AdImpressionEventArgs>? AdImpression;

        public bool IsSupported => true;

        public BannerAdService(ILogger<BannerAdService>? logger = null)
        {
            _logger = logger;
        }

        public object CreateBannerAdView(string adUnitId, Models.AdSize adSize = Models.AdSize.Banner)
        {
            try
            {
                var context = Platform.CurrentActivity ?? global::Android.App.Application.Context;
                var adView = new AdView(context);
                
                adView.AdUnitId = adUnitId;
                adView.AdSize = ConvertAdSize(adSize);

                // Set up event handlers
                var adListener = new BannerAdListener(adUnitId, this);
                adView.AdListener = adListener;

                _logger?.LogDebug("Created banner ad view for unit ID: {AdUnitId}", adUnitId);
                return adView;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create banner ad view for unit ID: {AdUnitId}", adUnitId);
                throw;
            }
        }

        public async Task LoadBannerAdAsync(object adView, string adUnitId)
        {
            try
            {
                if (adView is not AdView androidAdView)
                {
                    throw new ArgumentException("Invalid ad view type for Android platform", nameof(adView));
                }

                var adRequest = new AdRequest.Builder().Build();
                
                await Task.Run(() =>
                {
                    androidAdView.LoadAd(adRequest);
                });

                _logger?.LogDebug("Loading banner ad for unit ID: {AdUnitId}", adUnitId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load banner ad for unit ID: {AdUnitId}", adUnitId);
                AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(adUnitId, -1, ex.Message));
            }
        }

        public void DestroyBannerAd(object adView)
        {
            try
            {
                if (adView is AdView androidAdView)
                {
                    androidAdView.Destroy();
                    _logger?.LogDebug("Destroyed banner ad view");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to destroy banner ad view");
            }
        }

        private static global::Android.Gms.Ads.AdSize ConvertAdSize(Models.AdSize adSize)
        {
            return adSize switch
            {
                Models.AdSize.Banner => global::Android.Gms.Ads.AdSize.Banner,
                Models.AdSize.LargeBanner => global::Android.Gms.Ads.AdSize.LargeBanner,
                Models.AdSize.MediumRectangle => global::Android.Gms.Ads.AdSize.MediumRectangle,
                Models.AdSize.FullBanner => global::Android.Gms.Ads.AdSize.FullBanner,
                Models.AdSize.Leaderboard => global::Android.Gms.Ads.AdSize.Leaderboard,
                Models.AdSize.SmartBanner => global::Android.Gms.Ads.AdSize.SmartBanner,
                _ => global::Android.Gms.Ads.AdSize.Banner
            };
        }

        private class BannerAdListener : AdListener
        {
            private readonly string _adUnitId;
            private readonly BannerAdService _service;

            public BannerAdListener(string adUnitId, BannerAdService service)
            {
                _adUnitId = adUnitId;
                _service = service;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();
                _service._logger?.LogDebug("Banner ad loaded for unit ID: {AdUnitId}", _adUnitId);
                _service.AdLoaded?.Invoke(_service, new AdLoadedEventArgs(_adUnitId));
            }

            public override void OnAdFailedToLoad(LoadAdError error)
            {
                base.OnAdFailedToLoad(error);
                _service._logger?.LogWarning("Banner ad failed to load for unit ID: {AdUnitId}, Error: {Error}", _adUnitId, error.Message);
                _service.AdFailedToLoad?.Invoke(_service, new AdFailedToLoadEventArgs(_adUnitId, error.Code, error.Message));
            }

            public override void OnAdClicked()
            {
                base.OnAdClicked();
                _service._logger?.LogDebug("Banner ad clicked for unit ID: {AdUnitId}", _adUnitId);
                _service.AdClicked?.Invoke(_service, new AdClickedEventArgs(_adUnitId));
            }

            public override void OnAdImpression()
            {
                base.OnAdImpression();
                _service._logger?.LogDebug("Banner ad impression recorded for unit ID: {AdUnitId}", _adUnitId);
                _service.AdImpression?.Invoke(_service, new AdImpressionEventArgs(_adUnitId));
            }
        }
    }
}
#endif