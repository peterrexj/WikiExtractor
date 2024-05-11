using GeneralInformation.Exts;
using Pj.Library;
using System;
using System.IO;
using System.Threading.Tasks;
using WikiExtractor.Exts;
using WikiExtractor.ViewModels;
using Xamarin.Forms;

namespace GeneralInformation
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
                App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    action();
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
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
            control._localFileName = Path.Combine(ConfigData.LocalStorageCacheFolderPath, (string)newValue);
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
                    control.CustomSource = ImageSource.FromUri(((Xamarin.Forms.UriImageSource)newValue).Uri);
                }
                else if (newValue is FileImageSource)
                {
                    control.CustomSource = (Xamarin.Forms.FileImageSource)newValue;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
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
                                base.Source = ImageSource.FromFile(((Xamarin.Forms.FileImageSource)value).File);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.CaptureException(ex);
                        }
                    })
                );
            }
        }
        #endregion

        public static readonly BindableProperty PictureSourceProperty =
            BindableProperty.Create(
                propertyName: "PictureSource",
                returnType: typeof(PictureViewModel),
                declaringType: typeof(ExtendedImage),
                defaultValue: default(PictureViewModel),
                propertyChanged: OnPictureSourcePropertyChanged);

        private static void OnPictureSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (ExtendedImage)bindable;
                control.PictureSource = newValue as PictureViewModel;
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        public PictureViewModel PictureSource
        {
            set
            {
                Task.Run(() =>
                    RunOnAppDispatcher(() =>
                    {
                        try
                        {
                            if (value == null) { return; }
                            if (value is not PictureViewModel obj) { return; }

                            if (obj.PictureLocalPath.HasValue() && File.Exists(obj.PictureLocalPath))
                            {
                                base.Source = null;
                                base.Source = ImageSource.FromFile(obj.PictureLocalPath);
                            }
                            else if (obj.PicturePath.HasValue() && obj.PicturePath.StartsWith("http"))
                            {
                                if (_localFileName.IsEmpty())
                                {
                                    _localFileName = obj.PictureLocalPath;
                                }

                                base.Source = null;
                                LoadImageAsync(obj.PicturePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.CaptureException(ex);
                        }
                    })
                );
            }
        }

        private async Task GetImageStreamFromUrl(string url)
        {
            try
            {
                await CacheImageDownloadHelper.DownloadImage(_localFileName, url);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
        public async void LoadImageAsync(string imageUrl)
        {
            try
            {
                await GetImageStreamFromUrl(imageUrl);
                base.Source = null;
                base.Source = ImageSource.FromFile(_localFileName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
        public new ImageSource Source
        {
            get => base.Source;
            set
            {
                Task.Run(() =>
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
                        ExceptionHandler.CaptureException(ex);
                    }
                });
            }
        }
    }
}
