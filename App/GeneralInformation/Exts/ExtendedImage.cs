using GeneralInformation.Exts;
using GeneralInformation.Services;
using Pj.Library;
using System;
using System.IO;
using System.Threading.Tasks;
using TestAny.Essentials.Api;
using Xamarin.Forms;

namespace GeneralInformation
{
    public class ExtendedImage : Image
    {
        public static string CacheFolder = DependencyService.Get<IAppInformation>().ImageCacheFolder;

        public ExtendedImage()
        {

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
            control._localFileName = Path.Combine(CacheFolder, (string)newValue);
        }

        #endregion


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

        public ImageSource CustomSource
        {
            get => base.Source;
            set
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
                        base.Source = ImageSource.FromFile(((Xamarin.Forms.FileImageSource)value).File);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            }
        }
        public new ImageSource Source
        {
            get => base.Source;
            set
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
            }
        }

        public async void LoadImageAsync(string imageUrl)
        {
            try
            {
                await GetImageStreamFromUrl(imageUrl);
                base.Source = ImageSource.FromFile(_localFileName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
    }
}
