using MagicGradients;
using System.Drawing;

namespace GeneralInformation.Models.Mix
{
    public interface IStyleModel
    {
        string ActivityIndicatorColor { get; set; }
        string AppShellBackgroundColor { get; set; }
        string AppShellFlyoutItemLabelFontColor { get; set; }
        string AppShellFlyoutItemSelectedBackgroundColor { get; set; }
        string AppShellForegroundIconColor { get; set; }
        string AppShellTitleFontColor { get; set; }
        AppThemes AppTheme { get; set; }
        string AutoCompleteBackgroundColor { get; set; }
        string AutoCompleteClearButtonColor { get; set; }
        string AutoCompleteDropdownBackgroundColor { get; set; }
        string AutoCompleteDropdownBorderColor { get; set; }
        string AutoCompleteDropdownTextColor { get; set; }
        string AutoCompleteHighlightedTextColor { get; set; }
        string AutoCompleteNoResultsFoundTextColor { get; set; }
        string AutoCompleteTextColor { get; set; }
        string AutoCompleteWaterColor { get; set; }
        string DefaultFontColor { get; set; }
        string DefaultFontFamily { get; set; }
        string DefaultFontFamilyBold { get; set; }
        IGradientSource ListItemBoxBgSource { get; set; }
        string ListItemBoxBorderColor { get; set; }
        IGradientSource MgPageBgSource { get; set; }
        Color PageBgColorConverted { get; }
        string SegBgColor { get; set; }
        string SegBorderColor { get; set; }
        string SegSelectedBgColor { get; set; }
        string SegSelectedTextFgColor { get; set; }
        string SegTextFgColor { get; set; }
        string SortButtonColor { get; set; }
        string AppShellFooterGradientEnd { get; set; }
        string AppShellFooterGradientStart { get; set; }
        IGradientSource SubPageFactsBoxBorderBgSource { get; set; }
        string SubPageFactsFontColor { get; set; }
        IGradientSource SubPagePictureTitleBoxBorderBgSource { get; set; }
        string SubPagePictureTitleBoxTextColor { get; set; }
        IGradientSource SubPageTopHeaderBoxBorderBgSource { get; set; }
        string SubPageTopHeaderBoxBorderColor { get; set; }
        string SubPageTopHeaderFontColor { get; set; }
        string SubPageTopHeaderWikiFontColor { get; set; }
        string SubPageHeader2GradientStartColor { get; set; }
        string SubPageHeader2GradientEndColor { get; set; }
        string SubPageHeader3GradientStartColor { get; set; }
        string SubPageHeader3GradientEndColor { get; set; }
        string SwitchThumbBorderColorOFF { get; set; }
        string SwitchThumbBorderColorON { get; set; }
        string SwitchThumbColorOFF { get; set; }
        string SwitchThumbColorON { get; set; }
        string SwitchTrackColorOFF { get; set; }
        string SwitchTrackColorON { get; set; }
        IGradientSource TabHeadBgSource { get; set; }
        string TabHeadTextColor { get; set; }
        string TabStrokeColor { get; set; }
        string ThemeButtonColor { get; set; }
        IGradientSource TopPanelBgSource { get; set; }
        string TopPanelBorderColor { get; set; }

        IGradientSource ChartBgSource { get; set; }
        string ChartBorderColor { get; set; }
        string ChartLegendColor { get; set; }
        string ChartDataMarkerColor { get; set; }
        string ChartCorrectAnswerColor { get; set; }
        string ChartWrongAnswerColor { get; set; }
        string ChartNotAnsweredColor { get; set; }
        string ChartDataMarkerFontColor { get; set; }
        string QuizQuestionBackColor { get; set; }
        string QuizAnswerDefaultFontBackColor { get; set; }
        string QuizAnswerDefaultBackColor { get; set; }
        string QuizAnswerSelectionBackColor { get; set; }
        string QuizProgressColor { get; set; }
        string QuizProgressTrackColor { get; set; }
        string ButtonQuizBackColor { get; set; }
        string ButtonQuizFontColor { get; set; }
        string QuizNextButtonBackColor01 { get; set; }
        string QuizNextButtonBackColor02 { get; set; }
        string PopupHeaderFooterBackColor { get; set; }
        string PopupContentBackColor { get; set; }
    }
}