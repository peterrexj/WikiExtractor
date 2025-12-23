using Pj.Library;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using WikiExtractor.Maui.App.Exts;

namespace WikiExtractor.Maui.App.Exts
{
    public class ExtendedImage : Image
    {
        public ExtendedImage()
        {

        }

        void RunOnAppDispatcher(Action action)
        {
            try
            {
                Application.Current.Dispatcher.Dispatch(() =>
                {
                    action();
                });
            }
            catch (Exception ex)
            {
                // TODO: Implement proper exception handling
                System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
            }
        }

        #region Local Image Path
        public static readonly BindableProperty LocalFileNameProperty =
            BindableProperty.Create(
                propertyName: "LocalFileName",
                returnType: typeof(string),
                declaringType: typeof(ExtendedImage),
                defaultValue: default(string),
                propertyChanged: OnLocalFileNamePropertyChanged);

        private string _localFileName;
        public string LocalFileName
        {
            get { return (string)GetValue(LocalFileNameProperty); }
            set { SetValue(LocalFileNameProperty, value); }
        }

        private static void OnLocalFileNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ExtendedImage)bindable;
            if (newValue != null && !string.IsNullOrEmpty((string)newValue))
            {
                control._localFileName = Path.Combine(FileSystem.CacheDirectory, (string)newValue);
            }
            else
            {
                control._localFileName = null;
            }
        }

        #endregion

        #region CustomSource
        public static readonly BindableProperty CustomSourceProperty =
            BindableProperty.Create(
                propertyName: "CustomSource",
                returnType: typeof(ImageSource),
                declaringType: typeof(ExtendedImage),
                defaultValue: default(ImageSource),
                propertyChanged: OnCustomSourcePropertyChanged);

        private static void OnCustomSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (ExtendedImage)bindable;
                if (newValue is UriImageSource uriImageSource && uriImageSource.Uri != null)
                {
                    control.CustomSource = ImageSource.FromUri(((Microsoft.Maui.Controls.UriImageSource)newValue).Uri);
                }
                else if (newValue is FileImageSource)
                {
                    control.CustomSource = (Microsoft.Maui.Controls.FileImageSource)newValue;
                }
            }
            catch (Exception ex)
            {
                // TODO: Implement proper exception handling
                System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
            }
        }

        public ImageSource CustomSource
        {
            get => base.Source;
            set
            {
                Task.Run(() =>
                    RunOnAppDispatcher(() =>
                    {
                        try
                        {
                            if (value is UriImageSource uriImageSource && uriImageSource.Uri != null)
                            {
                                base.Source = null;
                                LoadImageAsync(uriImageSource.Uri.ToString());
                            }
                            else
                            {
                                base.Source = null;
                                base.Source = ImageSource.FromFile(((Microsoft.Maui.Controls.FileImageSource)value).File);
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: Implement proper exception handling
                            System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
                        }
                    })
                );
            }
        }
        #endregion

        #region Picture Source
        public static readonly BindableProperty PictureSourceProperty =
            BindableProperty.Create(
                propertyName: "PictureSource",
                returnType: typeof(object),
                declaringType: typeof(ExtendedImage),
                defaultValue: default(object),
                propertyChanged: OnPictureSourcePropertyChanged);

        private static void OnPictureSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (ExtendedImage)bindable;
                control.PictureSource = newValue;
            }
            catch (Exception ex)
            {
                // TODO: Implement proper exception handling
                System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
            }
        }

        public object PictureSource
        {
            set
            {
                Task.Run(() =>
                    RunOnAppDispatcher(() =>
                    {
                        try
                        {
                            if (value == null) { return; }
                            
                            // TODO: Implement PictureViewModel handling when available
                            // For now, handle basic image source
                            if (value is string imagePath)
                            {
                                if (File.Exists(imagePath))
                                {
                                    base.Source = null;
                                    base.Source = ImageSource.FromFile(imagePath);
                                }
                                else if (!string.IsNullOrEmpty(imagePath) && imagePath.StartsWith("http"))
                                {
                                    if (string.IsNullOrEmpty(_localFileName))
                                    {
                                        var fileName = Path.GetFileName(imagePath);
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            _localFileName = Path.Combine(FileSystem.CacheDirectory, fileName);
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(_localFileName))
                                    {
                                        base.Source = null;
                                        LoadImageAsync(imagePath);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: Implement proper exception handling
                            System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
                        }
                    })
                );
            }
        }

        #endregion

        #region Image Width

        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(ExtendedImage), default(int));

        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        #endregion

        #region Image Height

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(ExtendedImage), default(int));

        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        #endregion

        #region Cancellation Token from the page

        public static readonly BindableProperty PageCancellationTokenSourceProperty =
            BindableProperty.Create(nameof(PageCancellationTokenSource), typeof(CancellationTokenSource), typeof(ExtendedImage), default(CancellationTokenSource));

        public CancellationTokenSource PageCancellationTokenSource
        {
            get { return (CancellationTokenSource)GetValue(PageCancellationTokenSourceProperty); }
            set { SetValue(PageCancellationTokenSourceProperty, value); }
        }

        #endregion

        private async Task GetImageStreamFromUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(_localFileName))
                {
                    return;
                }
                
                if (PageCancellationTokenSource == null)
                {
                    PageCancellationTokenSource = new CancellationTokenSource();
                }
                // TODO: Implement CacheImageDownloadHelper when available
                // For now, use basic HTTP client to download image
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(_localFileName, imageBytes);
            }
            catch (Exception ex)
            {
                // TODO: Implement proper exception handling
                System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
            }
        }
        public async void LoadImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(_localFileName))
                {
                    return;
                }
                
                await GetImageStreamFromUrl(imageUrl);
                base.Source = null;
                base.Source = ImageSource.FromFile(_localFileName);
            }
            catch (Exception ex)
            {
                // TODO: Implement proper exception handling
                System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
            }
        }
        public new ImageSource Source
        {
            get => base.Source;
            set
            {
                Task.Run(() =>
                {
                    RunOnAppDispatcher(() =>
                    {
                        try
                        {
                            if (value is UriImageSource uriImageSource && uriImageSource.Uri != null)
                            {
                                LoadImageAsync(uriImageSource.Uri.ToString());
                            }
                            else
                            {
                                base.Source = value;
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: Implement proper exception handling
                            System.Diagnostics.Debug.WriteLine($"ExtendedImage Exception: {ex.Message}");
                        }
                    });
                });
            }
        }
    }
}