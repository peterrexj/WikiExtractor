using ChristianCatholicSaints.iOS.DependencyImp;
using UIKit;

namespace ChristianCatholicSaints.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(ConfigHelperiOS.SyncFusionLicense);

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
