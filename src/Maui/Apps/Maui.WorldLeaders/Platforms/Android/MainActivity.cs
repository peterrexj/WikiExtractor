using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
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
            SetTheme(Maui.WorldLeaders.Resource.Style.Maui_MainTheme_NoActionBar_Dark);

            var bgHex = AppSettingsService.GetThemeBackgroundColor();
            var bgColor = Android.Graphics.Color.ParseColor(bgHex);
            Window?.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(bgColor));
            Window?.SetNavigationBarColor(bgColor);

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            // Apply status bar color + icon tint after window is fully initialized
            var isLight = AppSettingsService.IsThemeBackgroundLight();
            Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window?.SetStatusBarColor(bgColor);
            Window?.SetNavigationBarColor(bgColor);
            ApplyStatusBarIconTint(isLight);

            CrossFirebase.Initialize(this);
            MobileAds.Initialize(this);
        }

        private void ApplyStatusBarIconTint(bool lightIcons)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    var lightFlags = (int)WindowInsetsControllerAppearance.LightStatusBars
                                   | (int)WindowInsetsControllerAppearance.LightNavigationBars;
                    Window?.InsetsController?.SetSystemBarsAppearance(lightIcons ? lightFlags : 0, lightFlags);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.M && Window?.DecorView != null)
                {
#pragma warning disable CA1422
                    var flags = Window.DecorView.SystemUiVisibility;
                    if (lightIcons) flags |= (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                    else flags &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                    Window.DecorView.SystemUiVisibility = flags;
#pragma warning restore CA1422
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StatusBar] ApplyStatusBarIconTint error: {ex.Message}");
            }
        }
    }
}
