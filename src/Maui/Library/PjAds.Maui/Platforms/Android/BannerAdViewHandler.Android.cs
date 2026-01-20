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

        protected override Android.Views.View CreatePlatformView()
        {
            _bannerAdService = MauiContext?.Services?.GetService<IBannerAdService>();
            _logger = MauiContext?.Services?.GetService<ILogger<BannerAdViewHandler>>();

            // Create a FrameLayout as a container (similar to the UIView container in iOS)
            // This allows us to swap the AdView inside it without breaking the MAUI layout
            var container = new Android.Widget.FrameLayout(Context);
            container.LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);

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
                _logger?.LogInformation("Recreating Android AdView due to property change.");

                // Clean up old view
                _adView?.Destroy();
                container.RemoveAllViews();

                // Create and add new view
                _adView = CreateNativeAdView();
                container.AddView(_adView);

                LoadAd();
            }
        }

        private void LoadAd()
        {
            if (_adView == null || _bannerAdService == null || string.IsNullOrEmpty(VirtualView.AdUnitId))
                return;

            try
            {
                // We pass _adView specifically to the service
                _ = Task.Run(async () =>
                {
                    await _bannerAdService.LoadBannerAdAsync(_adView, VirtualView.AdUnitId);
                });
            }
            catch (Exception ex)
            {
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

        private void OnAdLoaded(object? sender, AdLoadedEventArgs e) => VirtualView?.OnAdLoaded(e);
        private void OnAdFailedToLoad(object? sender, AdFailedToLoadEventArgs e) => VirtualView?.OnAdFailedToLoad(e);
        private void OnAdClicked(object? sender, AdClickedEventArgs e) => VirtualView?.OnAdClicked(e);
        private void OnAdImpression(object? sender, AdImpressionEventArgs e) => VirtualView?.OnAdImpression(e);
    }
}
#endif