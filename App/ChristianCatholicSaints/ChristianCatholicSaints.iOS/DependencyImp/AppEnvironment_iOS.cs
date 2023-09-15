using ChristianCatholicSaints.iOS.DependencyImp;
using GeneralInformation;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppEnvironment_iOS))]
namespace ChristianCatholicSaints.iOS.DependencyImp
{
    public class AppEnvironment_iOS : IAppEnvironment
    {
        public IStyleModel GetStyle(AppThemes theme)
        {
            IStyleModel styles = StyleProviderGenericHelper.LoadStyle(theme);

            return styles;
        }

        public void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint) { }
    }
}