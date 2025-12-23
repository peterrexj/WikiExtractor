using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Exts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;

namespace WikiExtractor.Maui.App.Exts
{
    public static class StylePropertyHelper
    {

        public static IStyleModel LoadStyle(WikiExtractor.Maui.App.Services.AppThemes appTheme)
        {
            return CustomServices.AppEnvironment.GetStyle(appTheme);
        }

        public static int GetStyleOnListItemHeightRequestOnListPage()
        {
            var appInfo = CustomServices.AppInformation;
            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                return appInfo.StyleOnListItemHeightRequestOnListPagePhone;
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                return appInfo.StyleOnListItemHeightRequestOnListPageTablet;
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                return appInfo.StyleOnListItemHeightRequestOnListPageDesktop;
            }
            return ConfigData.MinHeightOfListItemInListPage;
        }
    }
}