using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using MagicGradients;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Exts;
using Xamarin.Forms;

namespace GeneralInformation.Exts
{
    public static class StylePropertyHelper
    {

        public static IStyleModel LoadStyle(AppThemes appTheme)
        {
            return DependencyService.Get<IAppEnvironment>().GetStyle(appTheme);
        }

        public static int GetStyleOnListItemHeightRequestOnListPage()
        {
            var appInfo = DependencyService.Get<IAppInformation>();
            if (Device.Idiom == TargetIdiom.Phone)
            {
                return appInfo.StyleOnListItemHeightRequestOnListPagePhone;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                return appInfo.StyleOnListItemHeightRequestOnListPageTablet;
            }
            else if (Device.Idiom == TargetIdiom.Desktop)
            {
                return appInfo.StyleOnListItemHeightRequestOnListPageDesktop;
            }
            return ConfigData.MinHeightOfListItemInListPage;
        }
    }
}
