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
            MainPage = new AppShell();
            if (!AppCenter.Configured)
            {
                AppCenter.Start($"android={DependencyService.Get<IAppInformation>().AppCentreAppKeyDroid};"
                      //  +
                      //"uwp={Your UWP App secret here};" +
                      //"ios={Your iOS App secret here};" +
                      //"macos={Your macOS App secret here};"
                      ,
                      typeof(Analytics), typeof(Crashes));
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
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ThemeHelper.SetTheme(e.RequestedTheme);
            });
        }
    }
}
