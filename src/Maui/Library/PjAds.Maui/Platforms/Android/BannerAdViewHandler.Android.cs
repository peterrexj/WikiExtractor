#if ANDROID
using Android.Content;
using Android.Gms.Ads;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using PjAds.Maui.Controls;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using Android.Views;

namespace PjAds.Maui.Controls
{
    public partial class BannerAdViewHandler : ViewHandler<BannerAdView, Android.Views.View>
    {
        private IBannerAdService? _bannerAdService;
        private ILogger<BannerAdViewHandler>? _logger;
        private AdView? _adView;

        private int BannerHeightPx =>
            (int)(50 * (Context?.Resources?.DisplayMetrics?.Density ?? 1f));

        protected override Android.Views.View CreatePlatformView()
        {
            _bannerAdService = MauiContext?.Services?.GetService<IBannerAdService>();
            _logger = MauiContext?.Services?.GetService<ILogger<BannerAdViewHandler>>();

            System.Diagnostics.Debug.WriteLine($"[PjAds] CreatePlatformView — service={(_bannerAdService != null ? "OK" : "NULL")}");

            var container = new Android.Widget.FrameLayout(Context);
            container.LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                BannerHeightPx);

            _adView = CreateNativeAdView();
            if (_adView != null)
            {
                container.AddView(_adView);
            }

            return container;
        }

        private AdView CreateNativeAdView()
        {
            var adView = new AdView(Context);

            if (!string.IsNullOrEmpty(VirtualView.AdUnitId))
                adView.AdUnitId = VirtualView.AdUnitId;

            adView.AdSize = ConvertToAndroidAdSize(VirtualView.AdSize);

            return adView;
        }

        protected override void ConnectHandler(Android.Views.View platformView)
        {
            base.ConnectHandler(platformView);

            System.Diagnostics.Debug.WriteLine($"[PjAds] ConnectHandler — AdUnitId='{VirtualView?.AdUnitId}' service={(_bannerAdService != null ? "OK" : "NULL")}");

            if (_bannerAdService != null)
            {
                _bannerAdService.AdLoaded += OnAdLoaded;
                _bannerAdService.AdFailedToLoad += OnAdFailedToLoad;
                _bannerAdService.AdClicked += OnAdClicked;
                _bannerAdService.AdImpression += OnAdImpression;
            }

            LoadAd();
        }

        protected override void DisconnectHandler(Android.Views.View platformView)
        {
            if (_bannerAdService != null)
            {
                _bannerAdService.AdLoaded -= OnAdLoaded;
                _bannerAdService.AdFailedToLoad -= OnAdFailedToLoad;
                _bannerAdService.AdClicked -= OnAdClicked;
                _bannerAdService.AdImpression -= OnAdImpression;
            }

            _adView?.Destroy();
            _adView = null;

            base.DisconnectHandler(platformView);
        }

        // --- Partial Method Implementations ---

        partial void UpdateAdUnitId() => RecreateAdView();

        partial void UpdateAdSize() => RecreateAdView();

        partial void UpdateBannerType() => LoadAd();

        private void RecreateAdView()
        {
            if (PlatformView is Android.Widget.FrameLayout container)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] RecreateAdView — AdUnitId='{VirtualView?.AdUnitId}'");

                _adView?.Destroy();
                container.RemoveAllViews();

                _adView = CreateNativeAdView();
                container.AddView(_adView);

                // Ensure container keeps its fixed height after recreation
                container.LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    BannerHeightPx);

                LoadAd();
            }
        }

        private void LoadAd()
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] LoadAd — adView={(_adView != null ? "OK" : "NULL")} service={(_bannerAdService != null ? "OK" : "NULL")} unitId='{VirtualView?.AdUnitId}'");

            if (_adView == null || _bannerAdService == null || string.IsNullOrEmpty(VirtualView?.AdUnitId))
            {
                System.Diagnostics.Debug.WriteLine("[PjAds] LoadAd — bailed out (null guard)");
                return;
            }

            try
            {
                var adView = _adView;
                var unitId = VirtualView.AdUnitId;
                _ = _bannerAdService.LoadBannerAdAsync(adView, unitId)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            System.Diagnostics.Debug.WriteLine($"[PjAds] LoadBannerAdAsync faulted: {t.Exception?.Flatten().InnerException?.Message}");
                        else
                            System.Diagnostics.Debug.WriteLine("[PjAds] LoadBannerAdAsync completed OK");
                    });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] LoadAd exception: {ex.Message}");
                _logger?.LogError(ex, "Failed to load Android banner ad");
            }
        }

        private static Android.Gms.Ads.AdSize ConvertToAndroidAdSize(Models.AdSize adSize)
        {
            return adSize switch
            {
                Models.AdSize.Banner => Android.Gms.Ads.AdSize.Banner,
                Models.AdSize.LargeBanner => Android.Gms.Ads.AdSize.LargeBanner,
                Models.AdSize.MediumRectangle => Android.Gms.Ads.AdSize.MediumRectangle,
                Models.AdSize.FullBanner => Android.Gms.Ads.AdSize.FullBanner,
                Models.AdSize.Leaderboard => Android.Gms.Ads.AdSize.Leaderboard,
                Models.AdSize.SmartBanner => Android.Gms.Ads.AdSize.SmartBanner,
                _ => Android.Gms.Ads.AdSize.Banner
            };
        }

        private void OnAdLoaded(object? sender, AdLoadedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] OnAdLoaded — {e.AdUnitId}");
            VirtualView?.OnAdLoaded(e);
        }

        private void OnAdFailedToLoad(object? sender, AdFailedToLoadEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] OnAdFailedToLoad — {e.AdUnitId} code={e.ErrorCode} msg={e.ErrorMessage}");
            VirtualView?.OnAdFailedToLoad(e);
        }

        private void OnAdClicked(object? sender, AdClickedEventArgs e) => VirtualView?.OnAdClicked(e);
        private void OnAdImpression(object? sender, AdImpressionEventArgs e) => VirtualView?.OnAdImpression(e);
    }
}
#endif