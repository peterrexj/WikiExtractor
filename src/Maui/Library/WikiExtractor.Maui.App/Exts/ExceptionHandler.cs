// using Microsoft.AppCenter.Crashes;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;

namespace WikiExtractor.Maui.App.Exts
{
    public class ExceptionHandler
    {
        private static readonly object _logLock = new object();
        private static bool _isLoggingSetup = false;
        private static string _logFilePath = string.Empty;

        static ExceptionHandler()
        {
            try
            {
                SetupLogging();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to initialize ExceptionHandler: {ex.Message}");
            }
        }

        private static void SetupLogging()
        {
            if (_isLoggingSetup) return;

            try
            {
                var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var logDir = Path.Combine(docsPath, "Logs");
                
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                _logFilePath = Path.Combine(logDir, $"app_log_{DateTime.Now:yyyyMMdd}.txt");
                _isLoggingSetup = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to setup logging: {ex.Message}");
            }
        }

        public static void RunOnAppDispatcher(Action action)
        {
            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Dispatch(() =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            LogToFile($"Exception in dispatcher action: {ex.Message}\n{ex.StackTrace}");
                        }
                    });
                }
                else
                {
                    LogToFile("Cannot dispatch action - Application.Current is null");
                }
            }
            catch (Exception ex)
            {
                LogToFile($"Failed to dispatch action: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void CaptureException(Exception exception, params string[] specificDetails)
        {
            if (exception == null) return;

            try
            {
                // Build detailed exception information
                var sb = new StringBuilder();
                sb.AppendLine($"Exception: {exception.GetType().FullName}");
                sb.AppendLine($"Message: {exception.Message}");
                sb.AppendLine($"Stack Trace: {exception.StackTrace}");
                
                if (exception.InnerException != null)
                {
                    sb.AppendLine($"Inner Exception: {exception.InnerException.GetType().FullName}");
                    sb.AppendLine($"Inner Message: {exception.InnerException.Message}");
                    sb.AppendLine($"Inner Stack Trace: {exception.InnerException.StackTrace}");
                }

                if (specificDetails != null && specificDetails.Length > 0)
                {
                    sb.AppendLine("Context Details:");
                    foreach (var detail in specificDetails.Where(f => f.HasValue()))
                    {
                        sb.AppendLine($"  - {detail}");
                    }
                }

                // Log to debug output
                Debug.WriteLine(sb.ToString());
                
                // Log to file
                LogToFile(sb.ToString());

#if DEBUG
                // In debug mode, we can rethrow for the debugger
                // But wrap in a check to prevent infinite recursion
                if (!exception.StackTrace?.Contains("ExceptionHandler.CaptureException") ?? true)
                {
                    Debug.WriteLine("DEBUG MODE: Exception captured but not rethrown to prevent app crash");
                }
#else
                // In release mode, log but don't crash
                Dictionary<string, string> errorContext = new();
                if (specificDetails != null)
                {
                    int counter = 0;
                    foreach (var detail in specificDetails.Where(f => f.HasValue()))
                    {
                        errorContext.Add($"Context{counter++}", detail);
                    }
                }
                // If AppCenter is configured in the future:
                // RunOnAppDispatcher(() => Crashes.TrackError(exception, DeviceDetails.GenerateMetaInformation(errorContext)));
#endif
            }
            catch (Exception ex)
            {
                // Last resort logging if exception handling itself fails
                Debug.WriteLine($"Error in exception handler: {ex.Message}");
                LogToFile($"Error in exception handler: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void LogToFile(string message)
        {
            try
            {
                if (!_isLoggingSetup)
                {
                    SetupLogging();
                }

                if (string.IsNullOrEmpty(_logFilePath)) return;

                lock (_logLock)
                {
                    File.AppendAllText(_logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\n\n");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to log to file: {ex.Message}");
            }
        }
    }
}