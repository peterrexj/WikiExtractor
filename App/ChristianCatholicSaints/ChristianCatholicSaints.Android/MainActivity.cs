using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Pj.Library;
using Android.Views;
using GeneralInformation;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using Android.Gms.Ads;
using Microsoft.AppCenter.Crashes;

namespace ChristianCatholicSaints.Droid
{
    [Activity(Label = "Saints", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(CryptoHelper.Decrypt("+QcTizXqAlBO5w3z7WWab6WlQqM96TUQ8OC8rPLrTVeTC1plyteIfJwEGTzq1TCWGELzS3AF87Z2LeXwOcc0Y6WV67I9nEFo61Uz71rzy0evCgpa221FX0GMc1/eQIc6"));
            base.OnCreate(savedInstanceState);

            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            MobileAds.Initialize(ApplicationContext);

            LoadApplication(new App());
        }

        #region Error 
        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;
            Crashes.TrackError(e.Exception);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            Crashes.TrackError(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
            Crashes.TrackError(newExc);
        }

        #endregion
    }
}