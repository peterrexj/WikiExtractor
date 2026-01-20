using Android.OS;
using Android.Views;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Exts;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;

namespace Maui.Wiki.Platforms.Android.DependencyInjection
{
    public class AppEnvironment : IAppEnvironment
    {

        public void SetStatusBarColor(Color color, bool darkStatusBarTint)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                return;

            // Get current activity from Android.App.Application.Context
            var context = global::Android.App.Application.Context;
            var activity = context as global::Android.App.Activity;
            var window = activity?.Window;
            if (window == null) return;

            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            // Convert Microsoft.Maui.Graphics.Color to Android.Graphics.Color
            var androidColor = new global::Android.Graphics.Color(
                (byte)(color.Red * 255),
                (byte)(color.Green * 255),
                (byte)(color.Blue * 255),
                (byte)(color.Alpha * 255));
            window.SetStatusBarColor(androidColor);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var flag = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                window.DecorView.SystemUiVisibility = darkStatusBarTint ? flag : 0;
            }
        }

        // Ads removed as per migration plan
        public bool DisplayAds => false;
    }
}
