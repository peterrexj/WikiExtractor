using CountriesOfWorld.UWP.DeviceDependencyImpl;
using GeneralInformation.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppEnvironment_Uwp))]
namespace CountriesOfWorld.UWP.DeviceDependencyImpl
{
    public class AppEnvironment_Uwp : IAppEnvironment
    {
        public void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint)
        {
           
        }
    }
}