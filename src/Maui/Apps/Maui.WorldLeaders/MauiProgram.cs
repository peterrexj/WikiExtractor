using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;
using Syncfusion.Maui.Core.Hosting;
using System.Diagnostics;
using WikiExtractor.Process;
using WikiExtractor.Exts;
using WikiExtractor.Maui.App.Exts;
using PjAds.Maui.Extensions;
using PjAds.Maui.Models;
using WikiExtractor.Maui.App.Views;
using WikiExtractor.Maui.App.Models;
using Maui.WorldLeaders.Views;

namespace Maui.WorldLeaders
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            IAppInformation appInfo;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Font Awesome 5 Free-Solid-900.otf", "FontAwesome");
                    fonts.AddFont("CALIBRI.TTF", "Calibri");
                    fonts.AddFont("Lato-Regular.ttf", "Lato");
                    fonts.AddFont("Nunito-Regular.ttf", "Nunito");
                    fonts.AddFont("Pacifico-Regular.ttf", "Pacifico");
                    fonts.AddFont("Raleway-Regular.ttf", "Raleway");
                });

            builder.Services.AddSingleton<WikiAppController>();

            builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
            builder.Services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<INoAdsService, NoAdsService>();

            builder.Services.AddSingleton<WikiExtractor.Maui.App.Services.IThemeHandler, WikiExtractor.Maui.App.Services.ThemeHandler>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddSingleton<SplashPage>();

            builder.Services.AddSingleton<App>();

            builder.Services.AddSingleton<WikiExtractor.Maui.App.ViewModels.PersonaListViewModel>();
            builder.Services.AddSingleton<WikiExtractor.Maui.App.ViewModels.PersonaDetailViewModel>();

            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.PersonaDetailPage>();

            builder.Services.AddTransient<WikiExtractor.Maui.App.ViewModels.QuizPageViewModel>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.ViewModels.QuizResultsPageViewModel>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.QuizPage>();
            builder.Services.AddTransient<WikiExtractor.Maui.App.Views.QuizResultsPage>();

#if ANDROID
            appInfo = new Maui.WorldLeaders.Platforms.Android.DependencyInjection.AppInformation();
            builder.Services.AddSingleton<IAppInformation>(appInfo);
            builder.Services.AddSingleton<IAppEnvironment, WikiExtractor.Maui.App.Platforms.Android.DependencyInjection.AppEnvironment>();
            builder.Services.AddSingleton<IImageService, WikiExtractor.Maui.App.Platforms.Android.DependencyInjection.ImageService>();
            builder.Services.AddSingleton<ILocalStorage, WikiExtractor.Maui.App.Platforms.Android.DependencyInjection.LocalStorage>();
#elif IOS
            appInfo = new Maui.WorldLeaders.Platforms.iOS.DependencyInjection.AppInformation();
            builder.Services.AddSingleton<IAppInformation>(appInfo);
            builder.Services.AddSingleton<IAppEnvironment, WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection.AppEnvironment>();
            builder.Services.AddSingleton<IImageService, WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection.ImageService>();
            builder.Services.AddSingleton<ILocalStorage, WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection.LocalStorage>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var adsConfig = new AdsConfig
            {
                ApplicationId = appInfo.AdsAppId,
                BannerAdUnitId = appInfo.AdsBannerId,
                QuizBannerAdUnitId = appInfo.AdsQuizBannerId,
                InterstitialAdUnitId = appInfo.AdsInterstitialId,
                AdsEnabled = true
            };
            builder.Services.AddSingleton(adsConfig);

            var adConfig = new AdConfiguration
            {
                ApplicationId = adsConfig.ApplicationId,
                BannerAdUnitId = appInfo.AdsBannerId,
                QuizBannerAdUnitId = appInfo.AdsQuizBannerId,
                InterstitialAdUnitId = appInfo.AdsInterstitialId,
                AdsEnabled = true,
                TestMode =
#if DEBUG
            true,
#else
            false,
#endif
                FirstInterstitialAdThreshold = 1,
                SubsequentInterstitialAdThreshold = 3
            };
            builder.UsePjAds(adConfig).ConfigurePjAdsHandlers();

            return builder.Build();
        }
    }
}
