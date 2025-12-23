using System.Threading;
using System.Threading.Tasks;

namespace WikiExtractor.Maui.App.Services
{
    public interface IImageService
    {
        Task<string> DownloadAndResizeImageAsync(string imageUrl, string outputFilePath, CancellationToken cancellationToken, int width, int height, double scalePercentage);
    }
}