using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TestAny.Essentials.Api;
using Xamarin.Forms;

namespace GeneralInformation.Exts
{
    public static class CacheImageDownloadHelper
    {
        public static int DaysToHoldCacheImage = DependencyService.Get<IAppInformation>().ImageCacheTotalDaysToInvalidate;

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

        public static async Task DownloadImage(string filePathLocal, string imageUrl)
        {
            try
            {
                var shouldDownload = ValidateCachedLocalFile(filePathLocal, imageUrl);
                if (shouldDownload)
                {
                    var response = await new TestApiHttp()
                                        .OpenFullUrl(imageUrl)
                                        .AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36")
                                        .DownloadAsync(filePathLocal);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex, $"LocalDownloadImageFilePath:{filePathLocal}", $"ImageUrl:{imageUrl}");
            }
        }
    }
}
