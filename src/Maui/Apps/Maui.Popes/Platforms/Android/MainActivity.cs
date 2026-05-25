using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using WikiExtractor.Maui.App.Services;

namespace Maui.Wiki
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Switch to the dark-windowed theme before MAUI renders its first frame,
            // preventing the bare white window from showing during the splash transition.
            SetTheme(Resource.Style.Maui_MainTheme_NoActionBar_Dark);

            // Override the window background with the user's saved theme color so that
            // switching themes persists across app restarts without a flash.
            var bgHex = AppSettingsService.GetThemeBackgroundColor();
            Window?.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor(bgHex)));

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            System.Diagnostics.Debug.WriteLine("[PjAds] MainActivity.OnCreate — calling MobileAds.Initialize");
            MobileAds.Initialize(this);
            System.Diagnostics.Debug.WriteLine("[PjAds] MainActivity.OnCreate — MobileAds.Initialize called (async init continues in background)");
        }
    }
}
