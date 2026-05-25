#if IOS
using Google.MobileAds;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using PjAds.Maui.Controls;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using UIKit;

namespace PjAds.Maui.Controls
{
    public partial class BannerAdViewHandler : ViewHandler<BannerAdView, UIView>
    {
        private IBannerAdService? _bannerAdService;
        private ILogger<BannerAdViewHandler>? _logger;
        private BannerView? _bannerView;

        protected override UIView CreatePlatformView()
        {
            _bannerAdService = MauiContext?.Services?.GetService<IBannerAdService>();
            _logger = MauiContext?.Services?.GetService<ILogger<BannerAdViewHandler>>();

            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS CreatePlatformView — service={(_bannerAdService != null ? "OK" : "NULL")}");

            var container = new UIView();
            container.BackgroundColor = UIColor.Clear;

            _bannerView = CreateNativeBannerView();
            if (_bannerView != null)
                container.AddSubview(_bannerView);

            return container;
        }

        private BannerView? CreateNativeBannerView()
        {
            if (_bannerAdService == null || string.IsNullOrEmpty(VirtualView?.AdUnitId))
                return null;

            var view = _bannerAdService.CreateBannerAdView(VirtualView.AdUnitId, VirtualView.AdSize) as BannerView;
            if (view != null)
                view.RootViewController = GetRootViewController();
            return view;
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);

            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS ConnectHandler — AdUnitId='{VirtualView?.AdUnitId}' service={(_bannerAdService != null ? "OK" : "NULL")}");

            if (_bannerAdService != null)
            {
                _bannerAdService.AdLoaded += OnAdLoaded;
                _bannerAdService.AdFailedToLoad += OnAdFailedToLoad;
                _bannerAdService.AdClicked += OnAdClicked;
                _bannerAdService.AdImpression += OnAdImpression;
            }

            LoadAd();
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            if (_bannerAdService != null)
            {
                _bannerAdService.AdLoaded -= OnAdLoaded;
                _bannerAdService.AdFailedToLoad -= OnAdFailedToLoad;
                _bannerAdService.AdClicked -= OnAdClicked;
                _bannerAdService.AdImpression -= OnAdImpression;
            }

            if (_bannerView != null)
            {
                _bannerAdService?.DestroyBannerAd(_bannerView);
                _bannerView = null;
            }

            base.DisconnectHandler(platformView);
        }

        partial void UpdateAdUnitId() => RecreateAdView();

        partial void UpdateAdSize() => RecreateAdView();

        partial void UpdateBannerType() => LoadAd();

        private void RecreateAdView()
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS RecreateAdView — AdUnitId='{VirtualView?.AdUnitId}'");

            if (_bannerView != null)
            {
                _bannerAdService?.DestroyBannerAd(_bannerView);
                _bannerView.RemoveFromSuperview();
                _bannerView = null;
            }

            if (PlatformView != null)
            {
                foreach (var sub in PlatformView.Subviews)
                    sub.RemoveFromSuperview();
            }

            _bannerView = CreateNativeBannerView();
            if (_bannerView != null)
            {
                PlatformView?.AddSubview(_bannerView);
                LoadAd();
            }
        }

        private void LoadAd()
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS LoadAd — bannerView={(_bannerView != null ? "OK" : "NULL")} service={(_bannerAdService != null ? "OK" : "NULL")} unitId='{VirtualView?.AdUnitId}'");

            if (_bannerView == null || _bannerAdService == null || string.IsNullOrEmpty(VirtualView?.AdUnitId))
            {
                System.Diagnostics.Debug.WriteLine("[PjAds] iOS LoadAd — bailed out (null guard)");
                return;
            }

            var bannerView = _bannerView;
            var unitId = VirtualView.AdUnitId;
            _ = _bannerAdService.LoadBannerAdAsync(bannerView, unitId)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        System.Diagnostics.Debug.WriteLine($"[PjAds] iOS LoadBannerAdAsync faulted: {t.Exception?.Flatten().InnerException?.Message}");
                    else
                        System.Diagnostics.Debug.WriteLine("[PjAds] iOS LoadBannerAdAsync completed OK");
                });
        }

        private static UIViewController? GetRootViewController()
        {
            var windowScene = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .FirstOrDefault();
            var window = windowScene?.Windows?.FirstOrDefault(w => w.IsKeyWindow)
                ?? UIApplication.SharedApplication.KeyWindow;
            var root = window?.RootViewController;
            while (root?.PresentedViewController != null)
                root = root.PresentedViewController;
            return root;
        }

        private void OnAdLoaded(object? sender, AdLoadedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS OnAdLoaded — {e.AdUnitId}");
            VirtualView?.OnAdLoaded(e);
        }

        private void OnAdFailedToLoad(object? sender, AdFailedToLoadEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS OnAdFailedToLoad — {e.AdUnitId} code={e.ErrorCode} msg={e.ErrorMessage}");
            VirtualView?.OnAdFailedToLoad(e);
        }

        private void OnAdClicked(object? sender, AdClickedEventArgs e) => VirtualView?.OnAdClicked(e);
        private void OnAdImpression(object? sender, AdImpressionEventArgs e) => VirtualView?.OnAdImpression(e);
    }
}
#endif
