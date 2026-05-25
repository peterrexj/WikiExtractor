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
            System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — start, adUnitId='{adUnitId}' adView type={adView?.GetType().Name ?? "null"}");

            try
            {
                if (adView is not AdView androidAdView)
                {
                    System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — FAIL: adView is not AdView, actual type={adView?.GetType().Name}");
                    throw new ArgumentException("AdView is not a native Android AdView", nameof(adView));
                }

                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — adView.AdUnitId='{androidAdView.AdUnitId}' adView.AdSize={androidAdView.AdSize}");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService — on main thread, setting listener and calling LoadAd for '{adUnitId}'");
                    var adRequest = new AdRequest.Builder().Build();
                    androidAdView.AdListener = new BannerAdListener(adUnitId, this);
                    androidAdView.LoadAd(adRequest);
                    System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService — LoadAd() called successfully for '{adUnitId}'");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — EXCEPTION: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — StackTrace: {ex.StackTrace}");
                _logger?.LogError(ex, "BannerAdService: Load failed");
                AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(adUnitId, -1, ex.Message));
            }
        }

        private class BannerAdListener : AdListener
        {
            private readonly string _adUnitId;
            private readonly BannerAdService _service;

            public BannerAdListener(string adUnitId, BannerAdService service)
            {
                _adUnitId = adUnitId;
                _service = service;
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener created for '{adUnitId}'");
            }

            public override void OnAdLoaded()
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdLoaded — '{_adUnitId}'");
                base.OnAdLoaded();
                _service.AdLoaded?.Invoke(_service, new AdLoadedEventArgs(_adUnitId));
            }

            public override void OnAdFailedToLoad(LoadAdError error)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdFailedToLoad — '{_adUnitId}' code={error.Code} msg='{error.Message}' domain='{error.Domain}'");
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdFailedToLoad — cause={error.Cause?.Message}");
                base.OnAdFailedToLoad(error);
                _service.AdFailedToLoad?.Invoke(_service, new AdFailedToLoadEventArgs(_adUnitId, error.Code, error.Message));
            }

            public override void OnAdClicked()
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdClicked — '{_adUnitId}'");
                base.OnAdClicked();
                _service.AdClicked?.Invoke(_service, new AdClickedEventArgs(_adUnitId));
            }

            public override void OnAdImpression()
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdImpression — '{_adUnitId}'");
                base.OnAdImpression();
                _service.AdImpression?.Invoke(_service, new AdImpressionEventArgs(_adUnitId));
            }

            public override void OnAdOpened()
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdOpened — '{_adUnitId}'");
                base.OnAdOpened();
            }

            public override void OnAdClosed()
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdListener.OnAdClosed — '{_adUnitId}'");
                base.OnAdClosed();
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
        }
    }
}
#endif