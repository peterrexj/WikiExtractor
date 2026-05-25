#if IOS
using Foundation;
using Google.MobileAds;
using Microsoft.Extensions.Logging;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using UIKit;

namespace PjAds.Maui.Platforms.iOS
{
    public class InterstitialAdService : IInterstitialAdService
    {
        private readonly ILogger<InterstitialAdService>? _logger;
        private InterstitialAd? _interstitialAd;
        private string? _currentAdUnitId;

        public event EventHandler<AdLoadedEventArgs>? AdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;
        public event EventHandler<InterstitialAdOpenedEventArgs>? AdOpened;
        public event EventHandler<InterstitialAdClosedEventArgs>? AdClosed;
        public event EventHandler<AdClickedEventArgs>? AdClicked;
        public event EventHandler<AdImpressionEventArgs>? AdImpression;

        public bool IsInterstitialAdLoaded => _interstitialAd != null;
        public bool IsSupported => true;

        public InterstitialAdService(ILogger<InterstitialAdService>? logger = null) => _logger = logger;

        public async Task LoadInterstitialAdAsync(string adUnitId)
        {
            _currentAdUnitId = adUnitId;
            System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialAdService.LoadInterstitialAdAsync — adUnitId='{adUnitId}'");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                InterstitialAd.Load(adUnitId, Request.GetDefaultRequest(), (ad, error) =>
                {
                    if (error != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialAd load failed — code={error.Code} msg='{error.LocalizedDescription}'");
                        _interstitialAd = null;
                        AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(adUnitId, (int)error.Code, error.LocalizedDescription));
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialAd loaded — '{adUnitId}'");
                    _interstitialAd = ad;
                    _interstitialAd!.Delegate = new InterstitialDelegate(adUnitId, this);
                    AdLoaded?.Invoke(this, new AdLoadedEventArgs(adUnitId));
                });
            });
        }

        public async Task<bool> ShowInterstitialAdAsync()
        {
            if (_interstitialAd == null)
            {
                System.Diagnostics.Debug.WriteLine("[PjAds] iOS ShowInterstitialAdAsync — no ad loaded");
                return false;
            }

            return await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var root = GetRootViewController();
                if (root == null)
                {
                    System.Diagnostics.Debug.WriteLine("[PjAds] iOS ShowInterstitialAdAsync — no root view controller");
                    return false;
                }

                _interstitialAd.Present(root);
                return true;
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

        private class InterstitialDelegate : FullScreenContentDelegate
        {
            private readonly string _adUnitId;
            private readonly InterstitialAdService _service;

            public InterstitialDelegate(string adUnitId, InterstitialAdService service)
            {
                _adUnitId = adUnitId;
                _service = service;
            }

            public override void DidPresentFullScreenContent(FullScreenPresentingAd ad)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialDelegate.DidPresent — '{_adUnitId}'");
                _service.AdOpened?.Invoke(_service, new InterstitialAdOpenedEventArgs(_adUnitId));
            }

            public override void DidDismissFullScreenContent(FullScreenPresentingAd ad)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialDelegate.DidDismiss — '{_adUnitId}'");
                _service._interstitialAd = null;
                _service.AdClosed?.Invoke(_service, new InterstitialAdClosedEventArgs(_adUnitId));
            }

            public override void DidFailToPresentFullScreenContent(FullScreenPresentingAd ad, NSError error)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialDelegate.DidFailToPresent — code={error.Code} msg='{error.LocalizedDescription}'");
                _service._interstitialAd = null;
                _service.AdFailedToLoad?.Invoke(_service, new AdFailedToLoadEventArgs(_adUnitId, (int)error.Code, error.LocalizedDescription));
            }

            public override void DidRecordClick(FullScreenPresentingAd ad)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialDelegate.DidRecordClick — '{_adUnitId}'");
                _service.AdClicked?.Invoke(_service, new AdClickedEventArgs(_adUnitId));
            }

            public override void DidRecordImpression(FullScreenPresentingAd ad)
            {
                System.Diagnostics.Debug.WriteLine($"[PjAds] iOS InterstitialDelegate.DidRecordImpression — '{_adUnitId}'");
                _service.AdImpression?.Invoke(_service, new AdImpressionEventArgs(_adUnitId));
            }
        }
    }
}
#endif
