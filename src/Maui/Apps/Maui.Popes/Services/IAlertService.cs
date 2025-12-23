namespace WikiExtractor.Maui.App.Services
{
    public interface IAlertService
    {
        Task ShowAlert(string title, string message, string cancel);
        Task<bool> ShowConfirmation(string title, string message, string accept, string cancel);
        void ShowToast(string message, int durationMilliseconds = 2000);
    }
}