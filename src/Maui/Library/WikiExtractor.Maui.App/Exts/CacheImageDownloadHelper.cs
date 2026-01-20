using Pj.Library;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Exts
{
    public static class CacheImageDownloadHelper
    {
        private static int? _daysToHoldCacheImage;
        private static int DaysToHoldCacheImage
        {
            get
            {
                if (_daysToHoldCacheImage == null)
                {
                    try
                    {
                        _daysToHoldCacheImage = SharedServiceCore.AppInformation?.ImageCacheTotalDaysToInvalidate ?? 7; // Default to 7 days
                    }
                    catch
                    {
                        _daysToHoldCacheImage = 7; // Fallback value
                    }
                }
                return _daysToHoldCacheImage.Value;
            }
        }

        public static bool ValidateCachedLocalFile(string filePathLocal, string imageUrl)
        {
            if (imageUrl.IsEmpty() || imageUrl.EqualsIgnoreCase("NoImageAvailable.png")) return false;
            bool fileExists = File.Exists(filePathLocal);

            if (!fileExists || (fileExists && File.GetCreationTime(filePathLocal).AddDays(DaysToHoldCacheImage) < DateTime.Now))
            {
                if (fileExists)
                {
                    IoHelper.DeleteFile(filePathLocal);
                }
                return true;
            }
            return false;
        }

        public static async Task DownloadImage(string filePathLocal, string imageUrl, CancellationToken cancellationToken, int width, int height, double scalePercentage)
        {
            try
            {
                var shouldDownload = ValidateCachedLocalFile(filePathLocal, imageUrl);
                if (shouldDownload)
                {
                    await SharedServiceCore.ImageService.DownloadAndResizeImageAsync(imageUrl, filePathLocal, cancellationToken, width, height, scalePercentage);
                }
                //var shouldDownload = ValidateCachedLocalFile(filePathLocal, imageUrl);
                //if (shouldDownload)
                //{
                //    var response = await new TestApiHttp()
                //                        .OpenFullUrl(imageUrl)
                //                        .AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36")
                //                        .DownloadAsync(filePathLocal);
                //}
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex, $"LocalDownloadImageFilePath:{filePathLocal}", $"ImageUrl:{imageUrl}");
            }
        }
    }
}