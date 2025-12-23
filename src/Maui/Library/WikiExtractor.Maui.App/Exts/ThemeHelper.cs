using Pj.Library;
using System;
using Microsoft.Maui.Controls;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;

namespace WikiExtractor.Maui.App.Exts
{
    public static class ThemeHelper
    {
        public static IStyleModel GetDefaultStyle()
        {
            return GetDefaultStyle(SettingsHelper.SelectedTheme);
        }
        public static IStyleModel GetDefaultStyle(WikiExtractor.Maui.App.Services.AppThemes theme)
        {
            // Convert Services.AppThemes to Models.Mix.AppThemes
            var mixAppTheme = theme switch
            {
                WikiExtractor.Maui.App.Services.AppThemes.Dark => WikiExtractor.Maui.App.Models.Mix.AppThemes.Dark,
                WikiExtractor.Maui.App.Services.AppThemes.Light => WikiExtractor.Maui.App.Models.Mix.AppThemes.Light,
                _ => WikiExtractor.Maui.App.Models.Mix.AppThemes.Light // Default fallback
            };
            
            return StyleProviderGenericHelper.LoadStyle(mixAppTheme);
        }

        public static void UpdateStatusBarBasedOnTheme(Color statusBarColor, bool isDarkTheme)
        {
            try
            {
                CustomServices.AppEnvironment.SetStatusBarColor(statusBarColor, isDarkTheme);
            }
            catch (Exception ex)
            {
            }
        }

        public static void UpdateAppThemes(IStyleModel styleModel)
        {
            if (styleModel == null) return;
            UpdateStatusBarBasedOnTheme(styleModel.PageBgColorConverted, styleModel.AppTheme == WikiExtractor.Maui.App.Models.Mix.AppThemes.Dark);

            ResourceDictionary appResources = Application.Current.Resources;
            if (appResources.ContainsKey("DefaultFontFamily") && styleModel.DefaultFontFamily.HasValue())
            {
                appResources["DefaultFontFamily"] = styleModel.DefaultFontFamily;
            }
            if (appResources.ContainsKey("DefaultFontFamilyBold") && styleModel.DefaultFontFamilyBold.HasValue())
            {
                appResources["DefaultFontFamilyBold"] = styleModel.DefaultFontFamilyBold;
            }
            if (appResources.ContainsKey("AppShellTitleFontColor") && styleModel.AppShellTitleFontColor.HasValue())
            {
                appResources["AppShellTitleFontColor"] = styleModel.AppShellTitleFontColor;
            }
            if (appResources.ContainsKey("AppShellFlyoutItemLabelFontColor") && styleModel.AppShellFlyoutItemLabelFontColor.HasValue())
            {
                appResources["AppShellFlyoutItemLabelFontColor"] = styleModel.AppShellFlyoutItemLabelFontColor;
            }
            if (appResources.ContainsKey("AppShellFlyoutItemSelectedBackgroundColor") && styleModel.AppShellFlyoutItemSelectedBackgroundColor.HasValue())
            {
                appResources["AppShellFlyoutItemSelectedBackgroundColor"] = styleModel.AppShellFlyoutItemSelectedBackgroundColor;
            }
            if (appResources.ContainsKey("AppShellBackgroundColor") && styleModel.AppShellBackgroundColor.HasValue())
            {
                appResources["AppShellBackgroundColor"] = styleModel.AppShellBackgroundColor;
            }
            if (appResources.ContainsKey("AppShellForegroundIconColor") && styleModel.AppShellForegroundIconColor.HasValue())
            {
                appResources["AppShellForegroundIconColor"] = styleModel.AppShellForegroundIconColor;
            }
            if (appResources.ContainsKey("AppShellFooterGradientStart") && styleModel.AppShellFooterGradientStart.HasValue())
            {
                appResources["AppShellFooterGradientStart"] = styleModel.AppShellFooterGradientStart;
            }
            if (appResources.ContainsKey("AppShellFooterGradientEnd") && styleModel.AppShellFooterGradientEnd.HasValue())
            {
                appResources["AppShellFooterGradientEnd"] = styleModel.AppShellFooterGradientEnd;
            }
        }


    }
}