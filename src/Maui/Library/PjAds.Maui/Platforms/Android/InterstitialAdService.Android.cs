#if ANDROID
using Android.Gms.Ads;
using Android.Gms.Ads.Interstitial;
using Android.Runtime;
using Microsoft.Extensions.Logging;
using PjAds.Maui.Models;
using PjAds.Maui.Services;

namespace PjAds.Maui.Platforms.Android
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

            // Ensure loading happens on UI thread to access CurrentActivity safely
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var adRequest = new AdRequest.Builder().Build();
                InterstitialAd.Load(
                    Platform.CurrentActivity ?? global::Android.App.Application.Context,
                    adUnitId,
                    adRequest,
                    new InterstitialAdLoadCallbackImpl(this));
            });
        }

        public async Task<bool> ShowInterstitialAdAsync()
        {
            if (_interstitialAd == null) return false;

            return await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var activity = Platform.CurrentActivity;
                if (activity == null) return false;

                _interstitialAd.Show(activity);
                return true;
            });
        }

        // Implementation of callbacks...
        private void OnInterstitialAdLoaded(InterstitialAd interstitialAd)
        {
            _interstitialAd = interstitialAd;
            _interstitialAd.FullScreenContentCallback = new InterstitialAdCallback(this);
            AdLoaded?.Invoke(this, new AdLoadedEventArgs(_currentAdUnitId ?? ""));
        }

        private void OnInterstitialAdFailedToLoad(LoadAdError error)
        {
            _interstitialAd = null;
            AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(_currentAdUnitId ?? "", error.Code, error.Message));
        }

        // Internal callback classes (same as yours but ensuring no Task.Run inside)
        private class InterstitialAdLoadCallbackImpl : global::Android.Gms.Ads.Interstitial.InterstitialAdLoadCallback
        {
            private readonly InterstitialAdService _service;

            public InterstitialAdLoadCallbackImpl(InterstitialAdService service)
            {
                _service = service;
            }

            // We use the Register attribute to map directly to the Java signature 
            // This prevents the "name clash" during Java code generation.
            [Register("onAdLoaded", "(Lcom/google/android/gms/ads/interstitial/InterstitialAd;)V", "GetOnAdLoaded_Lcom_google_android_gms_ads_interstitial_InterstitialAd_Handler")]
            public virtual void OnAdLoaded(global::Android.Gms.Ads.Interstitial.InterstitialAd ad)
            {
                _service.OnInterstitialAdLoaded(ad);
            }

            // This override usually works fine because LoadAdError isn't a generic type
            public override void OnAdFailedToLoad(global::Android.Gms.Ads.LoadAdError error)
            {
                _service.OnInterstitialAdFailedToLoad(error);
            }
        }

        private class InterstitialAdCallback : FullScreenContentCallback
        {
            private readonly InterstitialAdService _service;
            public InterstitialAdCallback(InterstitialAdService service) => _service = service;
            public override void OnAdDismissedFullScreenContent()
            {
                _service._interstitialAd = null; // Important: Clear after use
                _service.AdClosed?.Invoke(_service, new InterstitialAdClosedEventArgs(_service._currentAdUnitId ?? ""));
            }
            public override void OnAdShowedFullScreenContent() => _service.AdOpened?.Invoke(_service, new InterstitialAdOpenedEventArgs(_service._currentAdUnitId ?? ""));
            // ... Add remaining click/impression overrides here ...
        }
    }
}
#endif