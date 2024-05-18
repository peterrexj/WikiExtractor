using GeneralInformation.Exts;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralInformation
{
    public class ExceptionHandler
    {
        public static void RunOnAppDispatcher(Action action)
        {
            try
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    action();
                });
            }
            catch (Exception)
            {

            }
        }

        public static void CaptureException(Exception exception, params string[] specificDetails)
        {
            try
            {
#if DEBUG
                throw exception;
#else
                Dictionary<string, string> errorContext = new();
                if (specificDetails != null)
                {
                    int counter = 0;
                    foreach (var detail in specificDetails.Where(f => f.HasValue()))
                    {
                        errorContext.Add($"Context{counter++}", detail);
                    }
                }
                RunOnAppDispatcher(() => Crashes.TrackError(exception, DeviceDetails.GenerateMetaInformation(errorContext)));
#endif
            }
            catch (Exception)
            {
            }
        }

    }
}
