#if IOS
using Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using PjAds.Maui.Controls;
using PjAds.Maui.Models;
using PjAds.Maui.Services;
using UIKit;

namespace PjAds.Maui.Controls
{
    /// <summary>
    /// iOS-specific handler for BannerAdView using Google Mobile Ads SDK
    /// </summary>
    public partial class BannerAdViewHandler
    {
        private IBannerAdService? _bannerAdService;
        private ILogger<BannerAdViewHandler>? _logger;
        private UIView? _adView;

        protected override UIView CreatePlatformView()
        {
            try
            {
                // Get services
                _bannerAdService = MauiContext?.Services?.GetService<IBannerAdService>();
                _logger = MauiContext?.Services?.GetService<ILogger<BannerAdViewHandler>>();

                // Create a container view that will stretch to fill width
                var containerView = new UIView();
                containerView.BackgroundColor = UIColor.Clear;
                
                // Set up auto-resizing to fill width
                containerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                
                // Get the height for the ad size, but allow width to stretch
                var adHeight = GetHeightForAdSize(VirtualView.AdSize);
                containerView.Frame = new CoreGraphics.CGRect(0, 0, 320, adHeight); // Default width, will stretch

                // Create the actual banner ad view using the service
                if (_bannerAdService != null && !string.IsNullOrEmpty(VirtualView.AdUnitId))
                {
                    var adView = _bannerAdService.CreateBannerAdView(VirtualView.AdUnitId, VirtualView.AdSize);
                    if (adView is UIView view)
                    {
                        _adView = view;
                        
                        // Make the ad view fill the container width
                        view.Frame = containerView.Bounds;
                        view.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                        
                        containerView.AddSubview(view);
                        
                        // Subscribe to events
                        _bannerAdService.AdLoaded += OnAdLoaded;
                        _bannerAdService.AdFailedToLoad += OnAdFailedToLoad;
                        _bannerAdService.AdClicked += OnAdClicked;
                        _bannerAdService.AdImpression += OnAdImpression;
                    }
                }
                else
                {
                    // Add placeholder content to container that fills width
                    var placeholderView = new UIView();
                    placeholderView.BackgroundColor = UIColor.LightGray;
                    placeholderView.Frame = containerView.Bounds;
                    placeholderView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    
                    var label = new UILabel
                    {
                        Text = "Ad Placeholder",
                        TextAlignment = UITextAlignment.Center,
                        TextColor = UIColor.DarkGray,
                        Font = UIFont.SystemFontOfSize(12),
                        Frame = placeholderView.Bounds,
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
                    };
                    
                    placeholderView.AddSubview(label);
                    containerView.AddSubview(placeholderView);
                    
                    _logger?.LogWarning("Banner ad service not available, using placeholder view");
                }

                return containerView;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create iOS banner ad view");
                // Return a simple container view to prevent crashes
                var errorView = new UIView();
                var adHeight = GetHeightForAdSize(VirtualView.AdSize);
                errorView.Frame = new CoreGraphics.CGRect(0, 0, 320, adHeight);
                errorView.BackgroundColor = UIColor.Red;
                errorView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                return errorView;
            }
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);
            
            // Load the ad when the view is connected
            LoadAd();
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            try
            {
                // Unsubscribe from events
                if (_bannerAdService != null)
                {
                    try
                    {
                        _bannerAdService.AdLoaded -= OnAdLoaded;
                        _bannerAdService.AdFailedToLoad -= OnAdFailedToLoad;
                        _bannerAdService.AdClicked -= OnAdClicked;
                        _bannerAdService.AdImpression -= OnAdImpression;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error unsubscribing from banner ad events");
                    }
                }

                // Clean up the banner view safely
                if (_adView != null)
                {
                    try
                    {
                        // Ensure UI operations happen on main thread
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                if (_adView.Superview != null)
                                {
                                    _adView.RemoveFromSuperview();
                                }
                            }
                            catch (Exception uiEx)
                            {
                                _logger?.LogWarning(uiEx, "Error removing ad view from superview");
                            }
                        });
                        
                        _bannerAdService?.DestroyBannerAd(_adView);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error cleaning up ad view");
                    }
                    finally
                    {
                        _adView = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error during banner ad handler disconnect");
            }
            finally
            {
                base.DisconnectHandler(platformView);
            }
        }

        partial void UpdateAdUnitId()
        {
            // Recreate the banner view with new ad unit ID
            RecreateAdView();
        }

        partial void UpdateAdSize()
        {
            // Recreate the banner view with new ad size
            RecreateAdView();
        }

        partial void UpdateBannerType()
        {
            // Banner type is handled by the service, just reload the ad
            LoadAd();
        }

        private void RecreateAdView()
        {
            if (PlatformView != null && _bannerAdService != null && !string.IsNullOrEmpty(VirtualView.AdUnitId))
            {
                try
                {
                    // Clean up existing view safely
                    if (_adView != null)
                    {
                        _adView.RemoveFromSuperview();
                        _bannerAdService.DestroyBannerAd(_adView);
                        _adView = null;
                    }

                    // Clear the container
                    foreach (var subview in PlatformView.Subviews)
                    {
                        subview.RemoveFromSuperview();
                    }

                    // Create new banner view
                    var adView = _bannerAdService.CreateBannerAdView(VirtualView.AdUnitId, VirtualView.AdSize);
                    if (adView is UIView view)
                    {
                        _adView = view;
                        view.Frame = PlatformView.Bounds;
                        PlatformView.AddSubview(view);
                        
                        // Load the ad
                        LoadAd();
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to recreate ad view");
                }
            }
        }

        private void LoadAd()
        {
            if (_adView == null || _bannerAdService == null || string.IsNullOrEmpty(VirtualView.AdUnitId))
                return;

            try
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _bannerAdService.LoadBannerAdAsync(_adView, VirtualView.AdUnitId);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error loading banner ad, but continuing to prevent app crashes");
                        
                        // Don't rethrow - just log and continue to prevent app crashes
                        // The banner ad service should handle errors gracefully
                    }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to start banner ad loading task, but continuing");
                // Don't rethrow - just log and continue
            }
        }

        private static CoreGraphics.CGSize GetSizeForAdSize(Models.AdSize adSize)
        {
            return adSize switch
            {
                Models.AdSize.Banner => new CoreGraphics.CGSize(320, 50),
                Models.AdSize.LargeBanner => new CoreGraphics.CGSize(320, 100),
                Models.AdSize.MediumRectangle => new CoreGraphics.CGSize(300, 250),
                Models.AdSize.FullBanner => new CoreGraphics.CGSize(468, 60),
                Models.AdSize.Leaderboard => new CoreGraphics.CGSize(728, 90),
                Models.AdSize.SmartBanner => new CoreGraphics.CGSize(320, 50),
                _ => new CoreGraphics.CGSize(320, 50)
            };
        }

        private UIViewController? GetRootViewController()
        {
            try
            {
                var windowScene = UIApplication.SharedApplication.ConnectedScenes
                    .OfType<UIWindowScene>()
                    .FirstOrDefault();

                var window = windowScene?.Windows?.FirstOrDefault(w => w.IsKeyWindow) 
                    ?? UIApplication.SharedApplication.KeyWindow;

                return window?.RootViewController ?? GetTopViewController();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to get root view controller");
                return null;
            }
        }

        private UIViewController? GetTopViewController()
        {
            try
            {
                var rootController = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                
                while (rootController?.PresentedViewController != null)
                {
                    rootController = rootController.PresentedViewController;
                }

                return rootController;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to get top view controller");
                return null;
            }
        }

        private static nfloat GetHeightForAdSize(PjAds.Maui.Models.AdSize adSize)
        {
            return adSize switch
            {
                PjAds.Maui.Models.AdSize.Banner => 50,
                PjAds.Maui.Models.AdSize.LargeBanner => 100,
                PjAds.Maui.Models.AdSize.MediumRectangle => 250,
                PjAds.Maui.Models.AdSize.FullBanner => 60,
                PjAds.Maui.Models.AdSize.Leaderboard => 90,
                PjAds.Maui.Models.AdSize.SmartBanner => 50, // Default height for smart banner
                _ => 50
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