using Foundation;
using GeneralInformation;
using Google.MobileAds;
using Microsoft.AppCenter.Crashes;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfAutoComplete.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.Graphics;
using Syncfusion.XForms.iOS.PopupLayout;
using Syncfusion.XForms.iOS.ProgressBar;
using Syncfusion.XForms.iOS.TabView;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Wiki.iOS
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
            ObjCRuntime.Class.ThrowOnInitFailure = false;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            SQLitePCL.Batteries_V2.Init();

            global::Xamarin.Forms.Forms.Init();

            SfPopupLayoutRenderer.Init();
            LoadApplication(new App());
            try
            {
                MobileAds.SharedInstance.Start(CompletionHandler);
            }
            catch (Exception)
            {
                //Ignore the exception if the ads cannot be initialized
            }

            Xamarin.Essentials.Platform.Init(() => GetCurrentUIViewController());

            new SfBusyIndicatorRenderer();
            new SfAutoCompleteRenderer();
            SfListViewRenderer.Init();
            SfChipRenderer.Init();
            SfChipGroupRenderer.Init();
            SfSegmentedControlRenderer.Init();
            SfTabViewRenderer.Init();
            SfGradientViewRenderer.Init();
            SfPopupLayoutRenderer.Init();
            SfCircularProgressBarRenderer.Init();
            SfGradientViewRenderer.Init();
            SfChartRenderer.Init();


#if DEBUG
            DisplayCrashReport();
#endif

            return base.FinishedLaunching(app, options);
        }

        private void CompletionHandler(InitializationStatus status)
        {

        }

        UIViewController GetCurrentUIViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            return vc;
        }

        #region Error Handling
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);
#if DEBUG
            LogUnhandledException(newExc);
#else
            Crashes.TrackError(newExc);
#endif
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
#if DEBUG
            LogUnhandledException(newExc);
#else
            Crashes.TrackError(newExc);
#endif
        }

        [Conditional("DEBUG")]
        private static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
                DateTime.Now, exception.ToString());
                File.WriteAllText(errorFilePath, errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }
        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        [Conditional("DEBUG")]
        [Obsolete]
        private static void DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
            var errorFilePath = Path.Combine(libraryPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
            var alertView = new UIAlertView("Crash Report", errorText, null, "Close", "Clear") { UserInteractionEnabled = true };
            alertView.Clicked += (sender, args) =>
            {
                if (args.ButtonIndex != 0)
                {
                    File.Delete(errorFilePath);
                }
            };
            alertView.Show();
        }

        #endregion
    }
}
