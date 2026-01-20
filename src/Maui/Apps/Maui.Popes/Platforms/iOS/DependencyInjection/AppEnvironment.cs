using UIKit;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Exts;
using Microsoft.Maui.Graphics;

namespace Maui.Wiki.Platforms.iOS.DependencyInjection
{
    public class AppEnvironment : IAppEnvironment
    {

        public void SetStatusBarColor(Color color, bool darkStatusBarTint)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    // Get the current window safely
                    UIWindow window = GetCurrentWindow();
                    if (window?.WindowScene?.StatusBarManager != null)
                    {
                        var statusBar = new UIView(window.WindowScene.StatusBarManager.StatusBarFrame);
                        statusBar.BackgroundColor = new UIColor(
                            (float)color.Red,
                            (float)color.Green,
                            (float)color.Blue,
                            (float)color.Alpha);

                        window.AddSubview(statusBar);
                    }
                }
                else if (UIApplication.SharedApplication.ValueForKey(new Foundation.NSString("statusBar")) is UIView statusBar)
                {
                    statusBar.BackgroundColor = new UIColor(
                        (float)color.Red,
                        (float)color.Green,
                        (float)color.Blue,
                        (float)color.Alpha);
                }

                // Set status bar content to light or dark
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    UIApplication.SharedApplication.SetStatusBarStyle(
                        darkStatusBarTint ? UIStatusBarStyle.DarkContent : UIStatusBarStyle.LightContent,
                        false);
                }
                else
                {
                    UIApplication.SharedApplication.SetStatusBarStyle(
                        darkStatusBarTint ? UIStatusBarStyle.Default : UIStatusBarStyle.LightContent,
                        false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting status bar color: {ex.Message}");
                // Continue execution even if setting status bar color fails
            }
        }

        private UIWindow GetCurrentWindow()
        {
            // Get the current window using the modern API for iOS 13+
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
                {
                    if (scene is UIWindowScene windowScene)
                    {
                        foreach (var window in windowScene.Windows)
                        {
                            if (window.IsKeyWindow)
                            {
                                return window;
                            }
                        }
                    }
                }
            }

            // Fallback for older iOS versions
            return UIApplication.SharedApplication.Windows.Length > 0 ?
                   UIApplication.SharedApplication.Windows[0] : null;
        }

        // Ads removed as per migration plan
        public bool DisplayAds => false;
    }
}