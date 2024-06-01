using GeneralInformation;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Wiki.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppEnvironment))]
namespace Wiki.iOS
{
    public class AppEnvironment : IAppEnvironment
    {
        public IStyleModel GetStyle(AppThemes theme)
        {
            IStyleModel styles = StyleProviderGenericHelper.LoadStyle(theme);

            return styles;
        }

        public void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint) { }

        public bool DisplayAds => true;
    }
}