using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Wiki.Uwp;
using WikiExtractor.XamarinForms.Services;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageService))]
namespace Wiki.Uwp
{
    public class ImageService : IImageService
    {
        private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1, 1);
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

        public async Task<string> DownloadAndResizeImageAsync(string imageUrl, string outputFilePath, CancellationToken cancellationToken, int width = 100, int height = 100, double scalePercentage = 100)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
                    using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode) return null;

                        response.EnsureSuccessStatusCode();
                        using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await inputStream.CopyToAsync(memoryStream, 81920, cancellationToken);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            var randomAccessStream = memoryStream.AsRandomAccessStream();
                            var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

                            uint newWidth = 0;
                            uint newHeight = 0;

                            try
                            {
                                //Set the value from the source without depending
                                newWidth = decoder.PixelWidth;
                                newHeight = decoder.PixelHeight;
                            }
                            catch (Exception) { }

                            if (newWidth == 0) newWidth = (uint)width;
                            if (newHeight == 0) newHeight = (uint)height;

                            if (scalePercentage > 0)
                            {
                                newWidth = (uint)(newWidth * scalePercentage / 100);
                                newHeight = (uint)(newHeight * scalePercentage / 100);
                            }

                            // Transform to resize
                            BitmapTransform transform = new BitmapTransform()
                            {
                                ScaledWidth = newWidth,
                                ScaledHeight = newHeight
                            };

                            PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Premultiplied,
                                transform,
                                ExifOrientationMode.IgnoreExifOrientation,
                                ColorManagementMode.DoNotColorManage
                            );

                            // Determine the encoder based on the file extension
                            Guid encoderId = GetEncoderId(outputFilePath);

                            // Encode to the output file
                            InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream();
                            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, resizedStream);
                            encoder.SetPixelData(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Premultiplied,
                                newWidth,
                                newHeight,
                                decoder.DpiX,
                                decoder.DpiY,
                                pixelData.DetachPixelData());

                            await encoder.FlushAsync();

                            // Check if the file exists, create if not
                            StorageFile outputFile;
                            try
                            {
                                outputFile = await StorageFile.GetFileFromPathAsync(outputFilePath);
                            }
                            catch (FileNotFoundException)
                            {
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(outputFilePath));
                                outputFile = await folder.CreateFileAsync(Path.GetFileName(outputFilePath), CreationCollisionOption.ReplaceExisting);
                            }

                            // Write to output file
                            await FileLock.WaitAsync();
                            try
                            {
                                using (IRandomAccessStream fileStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
                                {
                                    resizedStream.Seek(0);
                                    await RandomAccessStream.CopyAndCloseAsync(resizedStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                                }
                            }
                            catch (FileLoadException ex)
                            {
                                if (ex.Message.Contains("The process cannot access the file because it is being used by another process") == false)
                                {
                                    throw;
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                FileLock.Release();
                            }
                        }
                    }
                }
                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or processing image from {imageUrl}: {ex.Message}");
                return null;
            }
        }

        private Guid GetEncoderId(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (extension)
            {
                case ".png":
                    return BitmapEncoder.PngEncoderId;
                case ".jpg":
                case ".jpeg":
                default:
                    return BitmapEncoder.JpegEncoderId;
            }
        }
    }
}
