using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using WikiExtractor.Maui.App.Services;

namespace Maui.Wiki.Platforms.iOS.DependencyInjection
{
    public class ImageService : IImageService
    {
        private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1, 1);
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

        public async Task<string> DownloadAndResizeImageAsync(string imageUrl, string outputFilePath, CancellationToken cancellationToken, int width = 100, int height = 100, double scalePercentage = 100)
        {
            UIImage original = null;
            UIImage resized = null;
            
            try
            {
                // Validate input parameters
                if (string.IsNullOrEmpty(imageUrl))
                {
                    System.Diagnostics.Debug.WriteLine("Error: Image URL is null or empty");
                    return null;
                }

                if (string.IsNullOrEmpty(outputFilePath))
                {
                    System.Diagnostics.Debug.WriteLine("Error: Output file path is null or empty");
                    return null;
                }

                // Ensure output directory exists
                string outputDirectory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error creating output directory: {ex.Message}");
                        return null;
                    }
                }

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true;

                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
                    // Add timeout to prevent hanging
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    
                    using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error downloading image: HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
                            return null;
                        }

                        using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await inputStream.CopyToAsync(memoryStream, cancellationToken);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            if (memoryStream.Length == 0)
                            {
                                System.Diagnostics.Debug.WriteLine("Error: Downloaded image data is empty");
                                return null;
                            }

                            var data = NSData.FromStream(memoryStream);
                            if (data == null || data.Length == 0)
                            {
                                System.Diagnostics.Debug.WriteLine("Error: NSData conversion failed");
                                return null;
                            }

                            original = UIImage.LoadFromData(data);
                            if (original == null)
                            {
                                System.Diagnostics.Debug.WriteLine("Error: Failed to load image from data");
                                return null;
                            }

                            int newWidth = width;
                            int newHeight = height;

                            try
                            {
                                // Set the value from the source without depending
                                if (original.Size.Width > 0 && original.Size.Height > 0)
                                {
                                    newWidth = (int)original.Size.Width;
                                    newHeight = (int)original.Size.Height;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error getting image dimensions: {ex.Message}");
                            }

                            if (scalePercentage > 0)
                            {
                                newWidth = (int)(newWidth * scalePercentage / 100);
                                newHeight = (int)(newHeight * scalePercentage / 100);
                            }

                            // Ensure dimensions are valid
                            newWidth = Math.Max(1, newWidth);
                            newHeight = Math.Max(1, newHeight);

                            try
                            {
                                // Resize the image
                                UIGraphics.BeginImageContextWithOptions(new CGSize(newWidth, newHeight), false, 0);
                                original.Draw(new CGRect(0, 0, newWidth, newHeight));
                                resized = UIGraphics.GetImageFromCurrentImageContext();
                                UIGraphics.EndImageContext();

                                if (resized == null)
                                {
                                    System.Diagnostics.Debug.WriteLine("Error: Failed to resize image");
                                    return null;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error resizing image: {ex.Message}");
                                return null;
                            }

                            bool lockAcquired = false;
                            try
                            {
                                // Use timeout to prevent deadlock
                                lockAcquired = await FileLock.WaitAsync(TimeSpan.FromSeconds(5));
                                if (!lockAcquired)
                                {
                                    System.Diagnostics.Debug.WriteLine("Error: Timeout waiting for file lock");
                                    return null;
                                }

                                // Save the image to file
                                string extension = Path.GetExtension(outputFilePath).ToLowerInvariant();
                                NSData imageData;
                                
                                if (extension == ".png")
                                {
                                    imageData = resized.AsPNG();
                                }
                                else
                                {
                                    imageData = resized.AsJPEG(0.8f);
                                }
                                
                                if (imageData == null || imageData.Length == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("Error: Failed to convert image to PNG/JPEG data");
                                    return null;
                                }
                                
                                imageData.Save(outputFilePath, true);
                                
                                // Verify file was created
                                if (!File.Exists(outputFilePath))
                                {
                                    System.Diagnostics.Debug.WriteLine("Error: File was not created after save operation");
                                    return null;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error saving image to file: {ex.Message}");
                                return null;
                            }
                            finally
                            {
                                if (lockAcquired)
                                {
                                    FileLock.Release();
                                }
                            }
                        }
                    }
                }
                return outputFilePath;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Image download was canceled");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error downloading or processing image from {imageUrl}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
            finally
            {
                original?.Dispose();
                resized?.Dispose();
            }
        }
    }
}