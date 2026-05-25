using Foundation;
using System;
using System.Diagnostics;
using UIKit;
using WikiExtractor.Maui.App.Exts;
using ObjCRuntime;
using Google.MobileAds;

namespace Maui.WorldLeaders
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            MobileAds.SharedInstance.Start(status =>
            {
                Debug.WriteLine("[App] MobileAds init complete");
            });

            try
            {
                SetupNativeExceptionHandling();
                return MauiProgram.CreateMauiApp();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[iOS] EXCEPTION in CreateMauiApp: {ex.Message}");
                throw;
            }
        }

        private void SetupNativeExceptionHandling()
        {
            try
            {
                Runtime.MarshalManagedException += (object sender, MarshalManagedExceptionEventArgs args) =>
                {
                    try
                    {
                        ExceptionHandler.CaptureException(args.Exception, "iOS MarshalManagedException");
                        args.ExceptionMode = MarshalManagedExceptionMode.UnwindNativeCode;
                    }
                    catch { }
                };

                ObjCRuntime.Runtime.MarshalObjectiveCException += (sender, args) =>
                {
                    try
                    {
                        if (args.Exception is NSException nsException)
                        {
                            var managedException = new Exception($"Uncaught iOS Exception: {nsException.Name} - {nsException.Reason}");
                            ExceptionHandler.CaptureException(managedException, "MarshalObjectiveCException");
                        }
                    }
                    catch { }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to set up native exception handling: {ex.Message}");
            }
        }
    }
}
