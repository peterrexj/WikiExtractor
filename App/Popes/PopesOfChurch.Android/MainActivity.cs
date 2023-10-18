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
using TestAny.Essentials.Core;
using Syncfusion.XForms.Android.PopupLayout;

namespace PopesOfChurch.Droid
{
    [Activity(Label = "Popes", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            GeneralInformation.ConfigHelperDroid.LoadConfig();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(GeneralInformation.ConfigHelperDroid.SyncFusionLicense);
            base.OnCreate(savedInstanceState);

            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            SfPopupLayoutRenderer.Init();
            MobileAds.Initialize(ApplicationContext);
            TestAnyAppConfig.InitializeFramework();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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