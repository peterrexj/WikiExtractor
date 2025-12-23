#if ANDROID
using Android.Gms.Ads;
using Android.Gms.Ads.Interstitial;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using Microsoft.Extensions.Logging;

namespace PjAds.Maui.Platforms.Android
{
    /// <summary>
    /// Android implementation of interstitial ad service using Google Mobile Ads SDK
    /// </summary>
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

        public InterstitialAdService(ILogger<InterstitialAdService>? logger = null)
        {
            _logger = logger;
        }

        public async Task LoadInterstitialAdAsync(string adUnitId)
        {
            try
            {
                _currentAdUnitId = adUnitId;
                var adRequest = new AdRequest.Builder().Build();

                await Task.Run(() =>
                {
                    var callback = new InterstitialAdLoadCallbackImpl(this);
                    InterstitialAd.Load(
                        Platform.CurrentActivity ?? global::Android.App.Application.Context,
                        adUnitId,
                        adRequest,
                        callback
                    );
                });

                _logger?.LogDebug("Loading interstitial ad for unit ID: {AdUnitId}", adUnitId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load interstitial ad for unit ID: {AdUnitId}", adUnitId);
                AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(adUnitId, -1, ex.Message));
            }
        }

        public async Task<bool> ShowInterstitialAdAsync()
        {
            try
            {
                if (_interstitialAd == null)
                {
                    _logger?.LogWarning("No interstitial ad loaded to show");
                    return false;
                }

                var activity = Platform.CurrentActivity as AndroidX.Fragment.App.FragmentActivity;
                if (activity == null)
                {
                    _logger?.LogError("No valid activity found to show interstitial ad");
                    return false;
                }

                await Task.Run(() =>
                {
                    _interstitialAd.Show(activity);
                });

                _logger?.LogDebug("Showing interstitial ad for unit ID: {AdUnitId}", _currentAdUnitId);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to show interstitial ad for unit ID: {AdUnitId}", _currentAdUnitId);
                return false;
            }
        }

        private void OnInterstitialAdLoaded(InterstitialAd interstitialAd)
        {
            _interstitialAd = interstitialAd;
            _interstitialAd.FullScreenContentCallback = new InterstitialAdCallback(this);
            
            _logger?.LogDebug("Interstitial ad loaded for unit ID: {AdUnitId}", _currentAdUnitId);
            AdLoaded?.Invoke(this, new AdLoadedEventArgs(_currentAdUnitId ?? string.Empty));
        }

        private void OnInterstitialAdFailedToLoad(LoadAdError error)
        {
            _interstitialAd = null;
            _logger?.LogWarning("Interstitial ad failed to load for unit ID: {AdUnitId}, Error: {Error}", _currentAdUnitId, error.Message);
            AdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(_currentAdUnitId ?? string.Empty, error.Code, error.Message));
        }

        private class InterstitialAdLoadCallbackImpl : global::Android.Gms.Ads.Interstitial.InterstitialAdLoadCallback
        {
            private readonly InterstitialAdService _service;

            public InterstitialAdLoadCallbackImpl(InterstitialAdService service)
            {
                _service = service;
            }

            public void OnAdLoaded(Java.Lang.Object ad)
            {
                _service.OnInterstitialAdLoaded((InterstitialAd)ad);
            }

            public void OnAdFailedToLoad(LoadAdError error)
            {
                _service.OnInterstitialAdFailedToLoad(error);
            }
        }

        private class InterstitialAdCallback : FullScreenContentCallback
        {
            private readonly InterstitialAdService _service;

            public InterstitialAdCallback(InterstitialAdService service)
            {
                _service = service;
            }

            public override void OnAdShowedFullScreenContent()
            {
                base.OnAdShowedFullScreenContent();
                _service._logger?.LogDebug("Interstitial ad opened for unit ID: {AdUnitId}", _service._currentAdUnitId);
                _service.AdOpened?.Invoke(_service, new InterstitialAdOpenedEventArgs(_service._currentAdUnitId ?? string.Empty));
            }

            public override void OnAdDismissedFullScreenContent()
            {
                base.OnAdDismissedFullScreenContent();
                _service._logger?.LogDebug("Interstitial ad closed for unit ID: {AdUnitId}", _service._currentAdUnitId);
                _service.AdClosed?.Invoke(_service, new InterstitialAdClosedEventArgs(_service._currentAdUnitId ?? string.Empty));
                
                // Clear the ad reference as it can only be shown once
                _service._interstitialAd = null;
            }

            public override void OnAdFailedToShowFullScreenContent(AdError error)
            {
                base.OnAdFailedToShowFullScreenContent(error);
                _service._logger?.LogError("Interstitial ad failed to show for unit ID: {AdUnitId}, Error: {Error}", _service._currentAdUnitId, error.Message);
                _service._interstitialAd = null;
            }

            public override void OnAdClicked()
            {
                base.OnAdClicked();
                _service._logger?.LogDebug("Interstitial ad clicked for unit ID: {AdUnitId}", _service._currentAdUnitId);
                _service.AdClicked?.Invoke(_service, new AdClickedEventArgs(_service._currentAdUnitId ?? string.Empty));
            }

            public override void OnAdImpression()
            {
                base.OnAdImpression();
                _service._logger?.LogDebug("Interstitial ad impression recorded for unit ID: {AdUnitId}", _service._currentAdUnitId);
                _service.AdImpression?.Invoke(_service, new AdImpressionEventArgs(_service._currentAdUnitId ?? string.Empty));
            }
        }
    }
}
#endif