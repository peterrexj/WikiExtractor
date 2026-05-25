using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace WikiExtractor.Maui.App.Services
{
    public class AlertService : IAlertService
    {
        public async Task ShowAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> ShowConfirmation(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public void ShowToast(string message, int durationMilliseconds = 2000)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var toast = Toast.Make(message, ToastDuration.Short);
                await toast.Show();
            });
        }
    }
}
