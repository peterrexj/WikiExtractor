using Android.OS;
using Android.Views;
using GeneralInformation;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Wiki.Droid;

[assembly: Dependency(typeof(AppEnvironment))]
namespace Wiki.Droid
{
    public class AppEnvironment : IAppEnvironment
    {
        public IStyleModel GetStyle(AppThemes theme)
        {
            IStyleModel styles = StyleProviderGenericHelper.LoadStyle(theme);

            return styles;
        }

        public void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                return;

            var activity = Platform.CurrentActivity;
            var window = activity.Window;
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.SetStatusBarColor(color.ToPlatformColor());

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var flag = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                window.DecorView.SystemUiVisibility = darkStatusBarTint ? flag : 0;
            }
        }

        public bool DisplayAds => true;
    }
}