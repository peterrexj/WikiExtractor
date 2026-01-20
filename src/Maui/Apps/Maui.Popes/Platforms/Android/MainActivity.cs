using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Syncfusion.Maui.Core.Hosting;
using System;
using System.Threading.Tasks;

namespace Maui.Wiki
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            //var window = Platform.CurrentActivity.Window;

            //// Use WindowInsetsControllerCompat for backward compatibility
            //var windowInsetsController = WindowCompat.GetInsetsController(window, window.DecorView);

            //if (windowInsetsController != null)
            //{
            //    // 1. Set the behavior (Equivalent to ImmersiveSticky)
            //    // This makes the bars reappear briefly when the user swipes from the edge
            //    windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;

            //    // 2. Hide the bars (Equivalent to Fullscreen | HideNavigation)
            //    // You can hide .StatusBars(), .NavigationBars(), or both using .SystemBars()
            //    windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
            //}
        }
    }
    //[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    //public class MainActivity : Activity
    //{
    //    protected override void OnCreate(Bundle? savedInstanceState)
    //    {
    //        base.OnCreate(savedInstanceState);

    //        // Load Syncfusion license
    //        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF1cWWhAYVF/WmFZfVpgdVdMZVVbRX9PIiBoS35RckVhWXxecnVVRmNeVkN0WA==");

    //        // Error handling
    //        AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
    //        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    //        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

    //        // Initialize MAUI app
    //        MauiProgram.CreateMauiApp();
    //    }

    //    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    //    {
    //        // Handle permissions result directly since Platform.OnRequestPermissionsResult is not available
    //        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    //    }

    //    #region Error Handling
    //    private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
    //    {
    //        e.Handled = true;
    //        Crashes.TrackError(e.Exception);
    //    }

    //    private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
    //    {
    //        var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
    //        Crashes.TrackError(newExc);
    //    }

    //    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    //    {
    //        var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
    //        Crashes.TrackError(newExc);
    //    }
    //    #endregion
    //}
}
