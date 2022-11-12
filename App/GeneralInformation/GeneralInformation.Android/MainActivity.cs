using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Pj.Library;

namespace GeneralInformation.Droid
{
    [Activity(Label = "GeneralInformation", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(CryptoHelper.Decrypt("+QcTizXqAlBO5w3z7WWab6WlQqM96TUQ8OC8rPLrTVeTC1plyteIfJwEGTzq1TCWGELzS3AF87Z2LeXwOcc0Y6WV67I9nEFo61Uz71rzy0evCgpa221FX0GMc1/eQIc6"));
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}