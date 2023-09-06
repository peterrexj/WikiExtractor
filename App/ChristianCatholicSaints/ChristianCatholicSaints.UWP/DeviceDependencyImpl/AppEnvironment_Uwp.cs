using ChristianCatholicSaints.UWP.DeviceDependencyImpl;
using GeneralInformation.Models.Mix;
using GeneralInformation;
using GeneralInformation.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppEnvironment_Uwp))]
namespace ChristianCatholicSaints.UWP.DeviceDependencyImpl
{
    public class AppEnvironment_Uwp : IAppEnvironment
    {
        public IStyleModel GetStyle(AppThemes theme)
        {
            IStyleModel styles = StyleProviderGenericHelper.LoadStyle(theme);

            return styles;
        }

        public void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint)
        {
           
        }
    }
}