using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Plugin.Firebase.Core.Platforms.Android;
using WikiExtractor.Maui.App.Services;

namespace Maui.WorldLeaders
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Maui_MainTheme_NoActionBar_Dark);

            var bgHex = AppSettingsService.GetThemeBackgroundColor();
            Window?.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor(bgHex)));

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            CrossFirebase.Initialize(this);
            MobileAds.Initialize(this);
        }
    }
}
