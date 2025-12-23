using Foundation;
using System;
using System.Diagnostics;
using UIKit;
using WikiExtractor.Maui.App.Exts;
using ObjCRuntime;

namespace Maui.Wiki
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            // Set up iOS-specific exception handling
            SetupNativeExceptionHandling();
            
            return MauiProgram.CreateMauiApp();
        }

        private void SetupNativeExceptionHandling()
        {
            try
            {
                // Handle Objective-C exceptions
                Runtime.MarshalManagedException += (object sender, MarshalManagedExceptionEventArgs args) =>
                {
                    try
                    {
                        var exception = args.Exception;
                        ExceptionHandler.CaptureException(exception, "iOS MarshalManagedException",
                            $"Mode: {args.ExceptionMode}", "Source: Native iOS layer");
                        
                        // Don't throw the exception to the runtime
                        args.ExceptionMode = MarshalManagedExceptionMode.UnwindNativeCode;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error in MarshalManagedException handler: {ex.Message}");
                    }
                };

                // Handle unobserved exceptions in the iOS UI thread
                SetupUncaughtExceptionHandler();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to set up native exception handling: {ex.Message}");
            }
        }

        private void SetupUncaughtExceptionHandler()
        {
            // Set up a handler for uncaught Objective-C exceptions
            ObjCRuntime.Runtime.MarshalObjectiveCException += (sender, args) =>
            {
                try
                {
                    var exception = args.Exception;
                    if (exception is NSException nsException)
                    {
                        var name = nsException.Name;
                        var reason = nsException.Reason;
                        var callStack = nsException.CallStackSymbols;
                        var userInfo = nsException.UserInfo;

                        var callStackString = callStack != null ? string.Join("\n", callStack) : "No call stack available";
                        
                        var managedException = new Exception($"Uncaught iOS Exception: {name} - {reason}");
                        ExceptionHandler.CaptureException(managedException,
                            "MarshalObjectiveCException",
                            $"Call Stack: {callStackString}",
                            $"UserInfo: {userInfo}");
                    }
                    else
                    {
                        // Create a managed exception wrapper for the Objective-C exception
                        var managedException = new Exception($"Unknown Objective-C exception: {exception?.GetType().Name ?? "null"}");
                        ExceptionHandler.CaptureException(managedException,
                            "MarshalObjectiveCException",
                            "Unknown Objective-C exception");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in MarshalObjectiveCException handler: {ex.Message}");
                }
            };
        }
    }
}
