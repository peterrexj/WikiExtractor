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
        public static int GetStyleOnListItemHeightRequestOnListPage()
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                return SharedServiceCore.AppInformation.StyleOnListItemHeightRequestOnListPagePhone;
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                return SharedServiceCore.AppInformation.StyleOnListItemHeightRequestOnListPageTablet;
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                return SharedServiceCore.AppInformation.StyleOnListItemHeightRequestOnListPageDesktop;
            }
            return ConfigData.MinHeightOfListItemInListPage;
        }
    }
}