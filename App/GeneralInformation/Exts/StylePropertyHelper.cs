using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Exts;
using Xamarin.Forms;

namespace GeneralInformation.Exts
{
    internal static class StylePropertyHelper
    {
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
