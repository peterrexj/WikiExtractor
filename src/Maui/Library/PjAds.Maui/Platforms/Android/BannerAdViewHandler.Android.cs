#if ANDROID
using Android.Content;
using AndroidX.Fragment.App;
using Android.Gms.Ads;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using PjAds.Maui.Controls;
using PjAds.Maui.Models;
using PjAds.Maui.Services;

namespace PjAds.Maui.Controls
{
    /// <summary>
    /// Android-specific handler for BannerAdView
    /// </summary>
    public partial class BannerAdViewHandler : ViewHandler<BannerAdView, Android.Views.View>
    {
        private IBannerAdService? _bannerAdService;
        private ILogger<BannerAdViewHandler>? _logger;

        protected override Android.Views.View CreatePlatformView()
        {
            var context = Context ?? Platform.CurrentActivity ?? Android.App.Application.Context;
            var adView = new AdView(context);
            
            // Get services
            _bannerAdService = MauiContext?.Services?.GetService<IBannerAdService>();
            _logger = MauiContext?.Services?.GetService<ILogger<BannerAdViewHandler>>();

            // Set up initial configuration
            UpdateAdConfiguration(adView);
            
            // Subscribe to events
            if (_bannerAdService != null)
            {
                _bannerAdService.AdLoaded += OnAdLoaded;
                _bannerAdService.AdFailedToLoad += OnAdFailedToLoad;
                _bannerAdService.AdClicked += OnAdClicked;
                _bannerAdService.AdImpression += OnAdImpression;
            }

            return adView;
        }

        protected override void ConnectHandler(Android.Views.View platformView)
        {
            base.ConnectHandler(platformView);
            
            // Load the ad when the view is connected
            LoadAd();
        }

        protected override void DisconnectHandler(Android.Views.View platformView)
        {
            // Unsubscribe from events
            if (_bannerAdService != null)
            {
                _bannerAdService.AdLoaded -= OnAdLoaded;
                _bannerAdService.AdFailedToLoad -= OnAdFailedToLoad;
                _bannerAdService.AdClicked -= OnAdClicked;
                _bannerAdService.AdImpression -= OnAdImpression;
            }

            // Destroy the ad
            if (platformView is AdView adView)
            {
                adView.Destroy();
            }
            
            base.DisconnectHandler(platformView);
        }

        protected void UpdateAdUnitId()
        {
            if (PlatformView is AdView adView && !string.IsNullOrEmpty(VirtualView.AdUnitId))
            {
                adView.AdUnitId = VirtualView.AdUnitId;
                LoadAd();
            }
        }

        protected void UpdateAdSize()
        {
            if (PlatformView is AdView adView)
            {
                var androidAdSize = ConvertToAndroidAdSize(VirtualView.AdSize);
                adView.AdSize = androidAdSize;
                LoadAd();
            }
        }

        protected void UpdateBannerType()
        {
            // Banner type is handled by the service, just reload the ad
            LoadAd();
        }

        private void UpdateAdConfiguration(Android.Views.View platformView)
        {
            if (platformView is AdView adView)
            {
                if (!string.IsNullOrEmpty(VirtualView.AdUnitId))
                {
                    adView.AdUnitId = VirtualView.AdUnitId;
                }

                var androidAdSize = ConvertToAndroidAdSize(VirtualView.AdSize);
                adView.AdSize = androidAdSize;
            }
        }

        private void LoadAd()
        {
            if (PlatformView == null || _bannerAdService == null || string.IsNullOrEmpty(VirtualView.AdUnitId))
                return;

            try
            {
                _ = Task.Run(async () =>
                {
                    await _bannerAdService.LoadBannerAdAsync(PlatformView, VirtualView.AdUnitId);
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load banner ad");
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
            VirtualView.OnAdLoaded(e);
        }

        private void OnAdFailedToLoad(object? sender, AdFailedToLoadEventArgs e)
        {
            VirtualView.OnAdFailedToLoad(e);
        }

        private void OnAdClicked(object? sender, AdClickedEventArgs e)
        {
            VirtualView.OnAdClicked(e);
        }

        private void OnAdImpression(object? sender, AdImpressionEventArgs e)
        {
            VirtualView.OnAdImpression(e);
        }
    }
}
#endif