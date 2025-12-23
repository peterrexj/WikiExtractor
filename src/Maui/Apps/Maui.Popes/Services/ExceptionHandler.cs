using System.Diagnostics;

namespace Maui.Wiki.Services
{
    public static class ExceptionHandler
    {
        public static void CaptureException(Exception exception, string source, string additionalInfo = null)
        {
            try
            {
                Debug.WriteLine($"Exception in {source}: {exception.Message}");
                Debug.WriteLine(exception.StackTrace);
                
                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    Debug.WriteLine($"Additional info: {additionalInfo}");
                }
                
                // In a real app, you might want to log to a file or a remote service
                // For now, we'll just log to the debug console
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ExceptionHandler: {ex.Message}");
            }
        }
    }
}