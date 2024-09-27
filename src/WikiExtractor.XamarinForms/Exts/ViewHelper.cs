using GeneralInformation;
using System;
using System.Threading.Tasks;

namespace WikiExtractor.XamarinForms.Exts
{
    public class ViewHelper
    {
        public static void RunOnAppDispatcher(Action action)
        {
            try
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(action);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        public static Task RunOnAppDispatcherAsync(Action action)
        {
            try
            {
                return Task.Run(() => App.Current.Dispatcher.BeginInvokeOnMainThread(action));
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                return null;
            }
        }
    }
}
