using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Pj.Library;
using System;
using Xamarin.Forms;

namespace GeneralInformation.Exts
{
    public static class ThemeHelper
    {

        public static IStyleModel GetDefaultStyle()
        {
            return GetDefaultStyle(SettingsHelper.SelectedTheme);
        }
        public static IStyleModel GetDefaultStyle(AppThemes theme)
        {
            return StylePropertyHelper.LoadStyle(theme);
        }

        public static void UpdateStatusBarBasedOnTheme(Color statusBarColor, bool isDarkTheme)
        {
            try
            {
                DependencyService.Get<IAppEnvironment>().SetStatusBarColor(statusBarColor, isDarkTheme);
            }
            catch (Exception ex)
            {
            }
        }

        public static void UpdateAppThemes(IStyleModel styleModel)
        {
            if (styleModel == null) return;
            UpdateStatusBarBasedOnTheme(styleModel.PageBgColorConverted, styleModel.AppTheme == AppThemes.Dark);

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
            if (appResources.ContainsKey("AppShellFooterGradientStart") && styleModel.SubPageDetailsContentGradientStartColor.HasValue())
            {
                appResources["AppShellFooterGradientStart"] = styleModel.SubPageDetailsContentGradientStartColor;
            }
            if (appResources.ContainsKey("AppShellFooterGradientEnd") && styleModel.SubPageDetailsContentGradientEndColor.HasValue())
            {
                appResources["AppShellFooterGradientEnd"] = styleModel.SubPageDetailsContentGradientEndColor;
            }
        }


    }
}
