using System.Diagnostics;

namespace Maui.Wiki.Services
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Central exception handling method that can be used across the entire application
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="source">The source/location where the exception occurred</param>
        /// <param name="additionalInfo">Any additional context information</param>
        public static void CatchException(Exception exception, string source, string additionalInfo = null)
        {
            try
            {
                Debug.WriteLine($"[ERROR] Exception in {source}");
                Debug.WriteLine($"Message: {exception.Message}");
                Debug.WriteLine($"Type: {exception.GetType().Name}");
                Debug.WriteLine($"StackTrace: {exception.StackTrace}");
                
                if (exception.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {exception.InnerException.Message}");
                    Debug.WriteLine($"Inner StackTrace: {exception.InnerException.StackTrace}");
                }
                
                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    Debug.WriteLine($"Additional info: {additionalInfo}");
                }
                
                // In production, you might want to:
                // - Log to a file
                // - Send to a remote logging service (e.g., Application Insights, Sentry)
                // - Show user-friendly error message
                // - Track analytics
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Critical Error in ExceptionHandler: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Legacy method for backward compatibility
        /// </summary>
        public static void CaptureException(Exception exception, string source, string additionalInfo = null)
        {
            CatchException(exception, source, additionalInfo);
        }
    }
}