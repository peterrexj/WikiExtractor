using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Pj.Library;
using UIKit;

namespace GeneralInformation.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(CryptoHelper.Decrypt("+QcTizXqAlBO5w3z7WWab6WlQqM96TUQ8OC8rPLrTVeTC1plyteIfJwEGTzq1TCWGELzS3AF87Z2LeXwOcc0Y6WV67I9nEFo61Uz71rzy0evCgpa221FX0GMc1/eQIc6"));
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
