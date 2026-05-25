using Android.Graphics;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Platforms.Android.DependencyInjection
{
    public class ImageService : IImageService
    {
        private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1, 1);
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

        public async Task<string> DownloadAndResizeImageAsync(string imageUrl, string outputFilePath, CancellationToken cancellationToken, int width = 100, int height = 100, double scalePercentage = 100)
        {
            Bitmap original = null;
            Bitmap resized = null;
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true;

                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
                    using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode) return null;

                        response.EnsureSuccessStatusCode();
                        using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await inputStream.CopyToAsync(memoryStream, cancellationToken);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            original = BitmapFactory.DecodeStream(memoryStream);

                            int newWidth = 0;
                            int newHeight = 0;

                            try
                            {
                                newWidth = original.Width;
                                newHeight = original.Height;
                            }
                            catch (Exception) { }

                            if (newWidth == 0) newWidth = width;
                            if (newHeight == 0) newHeight = height;

                            if (scalePercentage > 0)
                            {
                                newWidth = (int)(newWidth * scalePercentage / 100);
                                newHeight = (int)(newHeight * scalePercentage / 100);
                            }

                            resized = Bitmap.CreateScaledBitmap(original, newWidth, newHeight, true);

                            await FileLock.WaitAsync();
                            try
                            {
                                using (var outputStream = File.OpenWrite(outputFilePath))
                                {
                                    resized.Compress(GetCompressFormat(outputFilePath), 80, outputStream);
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
            finally
            {
                original?.Dispose();
                resized?.Dispose();
            }
        }

        private Bitmap.CompressFormat GetCompressFormat(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".png" => Bitmap.CompressFormat.Png,
                _ => Bitmap.CompressFormat.Jpeg,
            };
        }
    }
}
