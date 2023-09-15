using System;
using System.Collections.Generic;
using System.Linq;
using ChristianCatholicSaints.iOS.DependencyImp;
using Foundation;
using Pj.Library;
using UIKit;

namespace ChristianCatholicSaints.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(CryptoHelper.Decrypt(ConfigHelperiOS.SyncFusionLicense));

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
