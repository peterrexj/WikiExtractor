using Android.OS;
using Android.Views;
using WikiExtractor.Maui.App.Services;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;

namespace WikiExtractor.Maui.App.Platforms.Android.DependencyInjection
{
    public class AppEnvironment : IAppEnvironment
    {
        public void SetStatusBarColor(Color color, bool darkStatusBarTint)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                return;

            var window = Platform.CurrentActivity?.Window;
            if (window == null) return;

            var androidColor = new global::Android.Graphics.Color(
                (byte)(color.Red * 255),
                (byte)(color.Green * 255),
                (byte)(color.Blue * 255),
                (byte)(color.Alpha * 255));

            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            // On API 35+ (Android 15), SetStatusBarColor/SetNavigationBarColor are silently ignored
            // due to edge-to-edge enforcement. The window background drawable is what paints the
            // inset areas (status bar, nav bar), so we must update it here too.
            window.SetBackgroundDrawable(new global::Android.Graphics.Drawables.ColorDrawable(androidColor));
            window.SetStatusBarColor(androidColor);
            window.SetNavigationBarColor(androidColor);

            // API 30+ — use native InsetsController directly (no AndroidX needed)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                var controller = window.InsetsController;
                if (controller != null)
                {
                    var lightFlags = (int)WindowInsetsControllerAppearance.LightStatusBars
                                   | (int)WindowInsetsControllerAppearance.LightNavigationBars;
                    // darkStatusBarTint=true means light bg → dark icons; false = dark bg → light icons
                    controller.SetSystemBarsAppearance(
                        darkStatusBarTint ? lightFlags : 0,
                        lightFlags);
                }
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
#pragma warning disable CA1422
                var flags = window.DecorView.SystemUiVisibility;
                if (darkStatusBarTint)
                    flags |= (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                else
                    flags &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                window.DecorView.SystemUiVisibility = flags;
#pragma warning restore CA1422
            }
        }

        public bool DisplayAds => false;
    }
}
