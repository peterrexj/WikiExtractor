using System;
using System.Threading.Tasks;

namespace WikiExtractor.Maui.App.Exts
{
    public class ViewHelper
    {
        public static void RunOnAppDispatcher(Action action)
        {
            try
            {
                Application.Current.Dispatcher.Dispatch(action);
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
                return Task.Run(() => Application.Current.Dispatcher.Dispatch(action));
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                return null;
            }
        }
    }
}