#if ANDROID
using Android.Gms.Ads;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using Microsoft.Extensions.Logging;

namespace PjAds.Maui.Platforms.Android
{
    public class BannerAdService : IBannerAdService
    {
        private readonly ILogger<BannerAdService>? _logger;
        public event EventHandler<AdLoadedEventArgs>? AdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;
        public event EventHandler<AdClickedEventArgs>? AdClicked;
        public event EventHandler<AdImpressionEventArgs>? AdImpression;

        public bool IsSupported => true;

        public BannerAdService(ILogger<BannerAdService>? logger = null) => _logger = logger;

        public async Task LoadBannerAdAsync(object adView, string adUnitId)
        {
            try
            {
                if (adView is not AdView androidAdView)
                    throw new ArgumentException("AdView is not a native Android AdView", nameof(adView));

                // We must be on the Main Thread to call LoadAd on a native view
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var adRequest = new AdRequest.Builder().Build();

                    // IMPORTANT: Do NOT use Task.Run here. 
                    // The SDK handles background networking itself.
                    androidAdView.AdListener = new BannerAdListener(adUnitId, this);
                    androidAdView.LoadAd(adRequest);
                });

                _logger?.LogDebug("BannerAdService: LoadAd called for {AdUnitId}", adUnitId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "BannerAdService: Load failed");
                AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(adUnitId, -1, ex.Message));
            }
        }

        // ... CreateBannerAdView and ConvertAdSize remain same as your logic ...

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
                _service.AdLoaded?.Invoke(_service, new AdLoadedEventArgs(_adUnitId));
            }
            public override void OnAdFailedToLoad(LoadAdError error)
            {
                base.OnAdFailedToLoad(error);
                _service.AdFailedToLoad?.Invoke(_service, new AdFailedToLoadEventArgs(_adUnitId, error.Code, error.Message));
            }
            public override void OnAdClicked()
            {
                base.OnAdClicked();
                //_service._logger?.LogDebug("Banner ad clicked for unit ID: {AdUnitId}", _adUnitId);
                _service.AdClicked?.Invoke(_service, new AdClickedEventArgs(_adUnitId));
            }
            public override void OnAdImpression()
            {
                base.OnAdImpression();
                _service.AdImpression?.Invoke(_service, new AdImpressionEventArgs(_adUnitId));
            }
        }

        public void DestroyBannerAd(object adView)
        {
            if (adView is AdView androidAdView)
                MainThread.BeginInvokeOnMainThread(() => androidAdView.Destroy());
        }

        public object CreateBannerAdView(string adUnitId, Models.AdSize adSize = Models.AdSize.Banner)
        {
            var context = Platform.CurrentActivity ?? global::Android.App.Application.Context;
            var adView = new AdView(context) { AdUnitId = adUnitId, AdSize = ConvertAdSize(adSize) };
            return adView;
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
            //adSize switch { /* ... your existing switch ... */ _ => global::Android.Gms.Ads.AdSize.Banner };
        }
    }
}
#endif