using GeneralInformation.Exts;
using GeneralInformation.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using System;
using WikiExtractor.Exts;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var appCentreKey = DependencyService.Get<IAppInformation>().AppCentreAppKey;

            if (!AppCenter.Configured)
            {
                if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                {
                    AppCenter.Start($"android={appCentreKey};", typeof(Analytics), typeof(Crashes));
                }
                else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                {
                    AppCenter.Start($"ios={appCentreKey};", typeof(Analytics), typeof(Crashes));
                }
                else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.UWP)
                {
                    AppCenter.Start($"uwp={appCentreKey};", typeof(Analytics), typeof(Crashes));
                }
                else
                {
                    AppCenter.Start($"android={appCentreKey};"
                          //  +
                          //"uwp={Your UWP App secret here};" +
                          //"ios={Your iOS App secret here};"
                          //"macos={Your macOS App secret here};"
                          ,
                          typeof(Analytics), typeof(Crashes));
                }
            }

            ConfigData.LocalStorageCacheFolderPath = DependencyService.Get<IAppInformation>().ImageCacheFolder;
            ConfigData.DisplayAds = DependencyService.Get<IAppEnvironment>().DisplayAds;

            if (ConfigData.LocalStorageCacheFolderPath.IsEmpty())
            {
                Crashes.TrackError(new Exception("LocalStorageCacheFolderPath is empty!"));
            }
            try
            {
                MainPage = new AppShell();
                ThemeHelper.UpdateAppThemes(ThemeHelper.GetDefaultStyle());
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex, $"AppCentre: {(appCentreKey ?? "not found!")}");
            }
        }

        protected override void OnStart()
        {
            OnResume();
        }

        protected override void OnSleep()
        {
            RequestedThemeChanged -= App_RequestedThemeChanged;
        }

        protected override void OnResume()
        {
            RequestedThemeChanged += App_RequestedThemeChanged;
        }

        private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ThemeHelper.UpdateAppThemes(ThemeHelper.GetDefaultStyle());
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
