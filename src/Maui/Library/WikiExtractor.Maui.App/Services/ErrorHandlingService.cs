using System.Diagnostics;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Process;

namespace WikiExtractor.Maui.App.Services
{
    public class ErrorHandlingService : IErrorHandlingService
    {
        private readonly IAlertService _alertService;

        public ErrorHandlingService(IAlertService alertService)
        {
            _alertService = alertService;
        }

        public void HandleException(Exception exception, string context = null)
        {
            if (exception == null)
                return;

            // Log the exception
            Debug.WriteLine($"ERROR [{context}]: {exception.Message}");
            Debug.WriteLine(exception.StackTrace);

            // Log to app's exception handler
            ExceptionHandler.CaptureException(exception, context);

            // Show a user-friendly message for critical errors
            if (IsCriticalException(exception))
            {
                _alertService.ShowAlert("Error", "An unexpected error occurred. Please try again later.", "OK");
            }
        }

        private bool IsCriticalException(Exception ex)
        {
            // Determine which exceptions should be shown to the user
            return ex is InvalidOperationException || 
                   ex is NullReferenceException ||
                   ex is ArgumentException ||
                   ex.Message.Contains("database") ||
                   ex.Message.Contains("storage");
        }
    }
}