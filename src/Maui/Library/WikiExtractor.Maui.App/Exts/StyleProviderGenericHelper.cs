using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Maui.App.Models.Mix;

namespace WikiExtractor.Maui.App.Exts
{
    public static class StyleProviderGenericHelper
    {
        public static IStyleModel LoadStyle(AppThemes appTheme)
        {
            switch (appTheme)
            {
                case AppThemes.Dark:
                    return new StyleModel
                    {
                        AppTheme = appTheme,

                        //Background
                        //Font
                        DefaultFontFamily = "Calibri",
                        DefaultFontFamilyBold = "CalibriB",
                        DefaultFontColor = "#D5D8DC",

                        ActivityIndicatorColor = "#EAF2F8",

                        //Top Panel
                        TopPanelBorderColor = "#1B4F72",

                        //List Item Box
                        ListItemBoxBorderColor = "#283747",

                        //Theme Button
                        ThemeButtonColor = "#E5E7E9",
                        SortButtonColor = "#CACFD2",

                        //Segments
                        SegBgColor = "#D5D8DC",
                        SegBorderColor = "#CACFD2",
                        SegTextFgColor = "#212F3D",
                        SegSelectedTextFgColor = "#154360",
                        SegSelectedBgColor = "#89b5af",

                        //SfSwitch
                        SwitchThumbBorderColorON = "#283433",
                        SwitchThumbColorON = "#4f6865", //  -- 884EA0
                        SwitchTrackColorON = "#a5dbd3",  //  -- EBDEF0

                        SwitchThumbBorderColorOFF = "#283433",
                        SwitchThumbColorOFF = "#4f6865",
                        SwitchTrackColorOFF = "#ae373c",

                        //AutoComplete
                        AutoCompleteBackgroundColor = "#424949",
                        AutoCompleteClearButtonColor = "#BFC9CA",
                        AutoCompleteDropdownBackgroundColor = "#424949",
                        AutoCompleteDropdownBorderColor = "#424949",
                        AutoCompleteDropdownTextColor = "#BFC9CA",
                        AutoCompleteHighlightedTextColor = "#148F77",
                        AutoCompleteNoResultsFoundTextColor = "#BFC9CA",
                        AutoCompleteTextColor = "#BFC9CA",
                        AutoCompleteWaterColor = "#707B7C",

                        //Details Page
                        SubPageTopHeaderBoxBorderColor = "#424949",
                        SubPageTopHeaderFontColor = "#D5D8DC",
                        SubPageTopHeaderWikiFontColor = "#7FB3D5",

                        SubPageFactsFontColor = "#ECF0F1",

                        SubPagePictureTitleBoxTextColor = "#1B2631",
                        SubPageHeader2GradientStartColor = "#1A5276",
                        SubPageHeader2GradientEndColor = "#424949",
                        SubPageHeader3GradientStartColor = "#161616",
                        SubPageHeader3GradientEndColor = "#1F618D",

                        //Play Button
                        PlayButtonBackgroundColor = "#D4E6F1",

                        //Tab
                        TabStrokeColor = "#85929E",
                        TabHeadTextColor = "#EAEDED",

                        //Themes
                        AppShellTitleFontColor = "#B2BABB",
                        AppShellFlyoutItemLabelFontColor = "#B2BABB",
                        AppShellFlyoutItemSelectedBackgroundColor = "#7F8C8D",
                        AppShellBackgroundColor = "#424949",
                        AppShellForegroundIconColor = "#D5D8DC",
                        AppShellFooterGradientStart = "#303030",
                        AppShellFooterGradientEnd = "#181818",

                        //Popup

                        //charts
                        ChartBorderColor = "#616A6B",
                        ChartLegendColor = "Green",
                        ChartDataMarkerColor = "#f4f6f6",
                        ChartCorrectAnswerColor = "#99BC85",
                        ChartWrongAnswerColor = "#F39189",
                        ChartNotAnsweredColor = "#B5C0D0",
                        ChartDataMarkerFontColor = "#C9BBCF",

                        QuizQuestionBackColor = "#551d2d",
                        QuizAnswerDefaultBackColor = "#F9F9F9",
                        QuizAnswerSelectionBackColor = "#AFB9C8",
                        QuizAnswerDefaultFontBackColor = "#535353",
                        QuizProgressColor = "#ca4060",
                        QuizProgressTrackColor = "#240a10",
                        QuizNextButtonBackColor = "#7b2a41",

                        PopupHeaderFooterBackColor = "#2b2b2b",
                        PopupContentBackColor = "#404040",

                        ButtonQuizBackColor = "#89B5AF",
                        ButtonQuizFontColor = "#222831",
                    };
                case AppThemes.Light:
                    return new StyleModel
                    {
                        AppTheme = appTheme,

                        //Background

                        //Font
                        DefaultFontFamily = "Calibri",
                        DefaultFontFamilyBold = "CalibriB",
                        DefaultFontColor = "#212F3C",

                        ActivityIndicatorColor = "#154360",

                        //Top Panel
                        TopPanelBorderColor = "#CCD1D1",

                        //List Item Box
                        ListItemBoxBorderColor = "#F5EEF8",

                        //Theme Button
                        ThemeButtonColor = "#202020",
                        SortButtonColor = "#566573",

                        //Segments
                        SegBgColor = "#F7F9F9",
                        SegBorderColor = "#34495E",
                        SegTextFgColor = "#808B96",
                        SegSelectedTextFgColor = "#212F3C",
                        SegSelectedBgColor = "#89a4c7",

                        //SfSwitch
                        SwitchThumbBorderColorON = "#283433",
                        SwitchThumbColorON = "#D6DBDF", //  -- 884EA0
                        SwitchTrackColorON = "#6e84a0",  //  -- EBDEF0

                        SwitchThumbBorderColorOFF = "#283433",
                        SwitchThumbColorOFF = "#D6DBDF",
                        SwitchTrackColorOFF = "#b9b9b9",

                        //AutoComplete
                        AutoCompleteBackgroundColor = "#BFC9CA",
                        AutoCompleteClearButtonColor = "#5D6D7E",
                        AutoCompleteDropdownBackgroundColor = "#BFC9CA",
                        AutoCompleteDropdownBorderColor = "#BFC9CA",
                        AutoCompleteDropdownTextColor = "#212F3C",
                        AutoCompleteHighlightedTextColor = "#117864",
                        AutoCompleteNoResultsFoundTextColor = "#212F3C",
                        AutoCompleteTextColor = "#212F3C",
                        AutoCompleteWaterColor = "#505050",

                        //Details Page
                        SubPageTopHeaderBoxBorderColor = "#CACFD2",
                        SubPageTopHeaderFontColor = "#34495E",
                        SubPageTopHeaderWikiFontColor = "#1A5276",

                        SubPagePictureTitleBoxTextColor = "#34495E",

                        SubPageFactsFontColor = "#212F3C",
                        SubPageHeader2GradientStartColor = "#579BB1",
                        SubPageHeader2GradientEndColor = "#bfbfbf",
                        SubPageHeader3GradientStartColor = "#bfbfbf",
                        SubPageHeader3GradientEndColor = "#579BB1",

                        //Play Button
                        PlayButtonBackgroundColor = "#CACFD2",

                        //Tab
                        TabStrokeColor = "#5D6D7E",
                        TabHeadTextColor = "#5D6D7E",

                        //Themes
                        AppShellTitleFontColor = "#34495E",
                        AppShellFlyoutItemLabelFontColor = "#85929E",
                        AppShellFlyoutItemSelectedBackgroundColor = "#34495E",
                        AppShellBackgroundColor = "#D5D8DC",
                        AppShellForegroundIconColor = "#212F3C",
                        AppShellFooterGradientStart = "#F8F8F8",
                        AppShellFooterGradientEnd = "#C8C8C8",

                        //Popup

                        //charts
                        ChartBorderColor = "#616A6B",
                        ChartLegendColor = "Green",
                        ChartDataMarkerColor = "#f4f6f6",
                        ChartCorrectAnswerColor = "#82e0aa",
                        ChartWrongAnswerColor = "#e59866",
                        ChartNotAnsweredColor = "#85c1e9",
                        ChartDataMarkerFontColor = "#C9BBCF",

                        QuizQuestionBackColor = "#54BAB9",
                        QuizAnswerDefaultBackColor = "#ededed",
                        QuizAnswerSelectionBackColor = "#8d8d8d",
                        QuizProgressColor = "#58c3c3",
                        QuizProgressTrackColor = "#1a3939",
                        QuizNextButtonBackColor = "#419191",
                        QuizAnswerDefaultFontBackColor = "#34495e",

                        PopupHeaderFooterBackColor = "#e5e7e9",
                        PopupContentBackColor = "#e5e7e9",

                        ButtonQuizBackColor = "#657993",
                        ButtonQuizFontColor = "#DEECFC",
                    };
                default: return null;
            }
        }
    }
}
