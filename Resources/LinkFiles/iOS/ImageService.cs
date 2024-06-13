using Foundation;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Wiki.iOS;
using WikiExtractor.XamarinForms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageService))]
namespace Wiki.iOS
{
    public class ImageService : IImageService
    {
        private static readonly SemaphoreSlim FileLock = new(1, 1);

        public async Task<string> DownloadAndResizeImageAsync(string imageUrl, string outputFilePath, CancellationToken cancellationToken, int width = 100, int height = 100, double scalePercentage = 100)
        {
            UIImage originalImage = null;
            UIImage resizedImage = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode) return null;

                    response.EnsureSuccessStatusCode();
                    using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                    using (var memoryStream = new MemoryStream())
                    {
                        await inputStream.CopyToAsync(memoryStream, cancellationToken);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        originalImage = UIImage.LoadFromData(NSData.FromStream(memoryStream));

                        nfloat newWidth = 0;
                        nfloat newHeight = 0;

                        try
                        {
                            //Set the value from the source without depending
                            newWidth = originalImage.Size.Width;
                            newHeight = originalImage.Size.Height;
                        }
                        catch (Exception) { }

                        if (newWidth == 0) newWidth = (nfloat)width;
                        if (newHeight == 0) newHeight = (nfloat)height;

                        if (scalePercentage > 0)
                        {
                            newWidth = (nfloat)(newWidth * scalePercentage / 100);
                            newHeight = (nfloat)(newHeight * scalePercentage / 100);
                        }

                        // Create a new scaled image
                        UIGraphics.BeginImageContext(new SizeF((float)newWidth, (float)newHeight));
                        originalImage.Draw(new RectangleF(0, 0, (float)newWidth, (float)newHeight));

                        resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                        UIGraphics.EndImageContext();

                        NSData imgData = null;
                        var imageType = GetImageType(outputFilePath);

                        imgData = imageType switch
                        {
                            ImageType.Png => resizedImage.AsPNG(),
                            _ => resizedImage.AsJPEG(0.8f),
                        };
                        
                        await FileLock.WaitAsync();
                        try
                        {
                            File.WriteAllBytes(outputFilePath, imgData.ToArray());
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
                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or processing image from {imageUrl}: {ex.Message}");
                return null;
            }
            finally
            {
                originalImage?.Dispose();
                resizedImage?.Dispose();
            }
        }
        private enum ImageType
        {
            Jpg,
            Png
        }
        private ImageType GetImageType(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".png" => ImageType.Png,
                _ => ImageType.Jpg,
            };
        }
    }
}