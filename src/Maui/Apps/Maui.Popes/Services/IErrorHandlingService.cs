namespace WikiExtractor.Maui.App.Services
{
    public interface IErrorHandlingService
    {
        void HandleException(Exception exception, string context = null);
    }
}