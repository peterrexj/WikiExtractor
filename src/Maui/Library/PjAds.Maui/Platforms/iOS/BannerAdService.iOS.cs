#if IOS
using Foundation;
using Google.MobileAds;
using Microsoft.Extensions.Logging;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using UIKit;

namespace PjAds.Maui.Platforms.iOS
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

        public object CreateBannerAdView(string adUnitId, Models.AdSize adSize = Models.AdSize.Banner)
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.CreateBannerAdView — adUnitId='{adUnitId}'");
            var gadSize = ConvertAdSize(adSize);
            var bannerView = new BannerView(gadSize)
            {
                AdUnitId = adUnitId
            };
            return bannerView;
        }

        public async Task LoadBannerAdAsync(object adView, string adUnitId)
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — start, adUnitId='{adUnitId}'");

            try
            {
                if (adView is not BannerView bannerView)
                {
                    System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — FAIL: adView is not BannerView, actual type={adView?.GetType().Name}");
                    throw new ArgumentException("adView is not a Google.MobileAds.BannerView", nameof(adView));
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService — on main thread, setting delegate and calling LoadRequest for '{adUnitId}'");
                    bannerView.Delegate = new BannerDelegate(adUnitId, this);
                    bannerView.LoadRequest(Request.GetDefaultRequest());
                    System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService — LoadRequest called for '{adUnitId}'");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerAdService.LoadBannerAdAsync — EXCEPTION: {ex.GetType().Name}: {ex.Message}");
                _logger?.LogError(ex, "BannerAdService: Load failed");
                AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(adUnitId, -1, ex.Message));
            }
        }

        public void DestroyBannerAd(object adView)
        {
            if (adView is BannerView bannerView)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    bannerView.Delegate = null;
                    bannerView.RemoveFromSuperview();
                });
        }

        private static Google.MobileAds.AdSize ConvertAdSize(Models.AdSize adSize)
        {
            return adSize switch
            {
                Models.AdSize.LargeBanner => AdSizeCons.LargeBanner,
                Models.AdSize.MediumRectangle => AdSizeCons.MediumRectangle,
                Models.AdSize.FullBanner => AdSizeCons.FullBanner,
                Models.AdSize.Leaderboard => AdSizeCons.Leaderboard,
                _ => AdSizeCons.Banner
            };
        }

        private class BannerDelegate : BannerViewDelegate
        {
            private readonly string _adUnitId;
            private readonly BannerAdService _service;

            public BannerDelegate(string adUnitId, BannerAdService service)
            {
                _adUnitId = adUnitId;
                _service = service;
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerDelegate created for '{adUnitId}'");
            }

            public override void DidReceiveAd(BannerView bannerView)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerDelegate.DidReceiveAd — '{_adUnitId}'");
                _service.AdLoaded?.Invoke(_service, new AdLoadedEventArgs(_adUnitId));
            }

            public override void DidFailToReceiveAd(BannerView bannerView, NSError error)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerDelegate.DidFailToReceiveAd — '{_adUnitId}' code={error.Code} msg='{error.LocalizedDescription}'");
                _service.AdFailedToLoad?.Invoke(_service, new AdFailedToLoadEventArgs(_adUnitId, (int)error.Code, error.LocalizedDescription));
            }

            public override void DidRecordClick(BannerView bannerView)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerDelegate.DidRecordClick — '{_adUnitId}'");
                _service.AdClicked?.Invoke(_service, new AdClickedEventArgs(_adUnitId));
            }

            public override void DidRecordImpression(BannerView bannerView)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] BannerDelegate.DidRecordImpression — '{_adUnitId}'");
                _service.AdImpression?.Invoke(_service, new AdImpressionEventArgs(_adUnitId));
            }
        }
    }
}
#endif
