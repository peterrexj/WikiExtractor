using GeneralInformation.Exts;
using GeneralInformation.Services;
using GeneralInformation.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace GeneralInformation
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ThemeHelper.UpdateAppThemes(ThemeHelper.GetDefaultStyle());

            MainPage = new AppShell();
            if (!AppCenter.Configured)
            {
                if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                {
                    AppCenter.Start($"android={DependencyService.Get<IAppInformation>().AppCentreAppKey};", typeof(Analytics), typeof(Crashes));
                }
                else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                {
                    AppCenter.Start($"ios={DependencyService.Get<IAppInformation>().AppCentreAppKey};", typeof(Analytics), typeof(Crashes));
                }
                else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.UWP)
                {
                    AppCenter.Start($"uwp={DependencyService.Get<IAppInformation>().AppCentreAppKey};", typeof(Analytics), typeof(Crashes));
                }
                else
                {
                    AppCenter.Start($"android={DependencyService.Get<IAppInformation>().AppCentreAppKey};"
                          //  +
                          //"uwp={Your UWP App secret here};" +
                          //"ios={Your iOS App secret here};"
                          //"macos={Your macOS App secret here};"
                          ,
                          typeof(Analytics), typeof(Crashes));
                }
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
