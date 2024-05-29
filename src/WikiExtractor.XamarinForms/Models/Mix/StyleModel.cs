using GeneralInformation.Services;
using MagicGradients;
using System.Drawing;
using WikiExtractor.ViewModels;

namespace GeneralInformation.Models.Mix
{
    public class StyleModel : BasePropertyChangeModel, IStyleModel
    {
        public virtual AppThemes AppTheme { get; set; }
        //Page level
        public virtual IGradientSource MgPageBgSource { get; set; }
        public virtual Color PageBgColorConverted => SharedServices.ToColorConverter.ToColorFromHex(AppShellBackgroundColor);

        //Fonts
        public virtual string DefaultFontFamily { get; set; }
        public virtual string DefaultFontFamilyBold { get; set; }
        public virtual string DefaultFontColor { get; set; }

        public virtual string ActivityIndicatorColor { get; set; }

        //Tab
        public virtual string TabStrokeColor { get; set; }
        public virtual string TabHeadTextColor { get; set; }
        public virtual IGradientSource TabHeadBgSource { get; set; }

        //TopPanel Box
        public virtual IGradientSource TopPanelBgSource { get; set; }
        public virtual string TopPanelBorderColor { get; set; }

        //List Item Box
        public virtual IGradientSource ListItemBoxBgSource { get; set; }
        public virtual string ListItemBoxBorderColor { get; set; }

        //Theme button
        public virtual string ThemeButtonColor { get; set; }
        public virtual string SortButtonColor { get; set; }

        //SegmentedControl
        public virtual string SegTextFgColor { get; set; }
        public virtual string SegSelectedTextFgColor { get; set; }
        public virtual string SegBorderColor { get; set; }
        public virtual string SegBgColor { get; set; }
        public virtual string SegSelectedBgColor { get; set; }

        //SfSwitch
        public virtual string SwitchBusyIndicatorColorON { get; set; }
        public virtual string SwitchThumbBorderColorON { get; set; }
        public virtual string SwitchThumbColorON { get; set; }
        public virtual string SwitchTrackBorderColorON { get; set; }
        public virtual string SwitchTrackColorON { get; set; }

        public virtual string SwitchBusyIndicatorColorOFF { get; set; }
        public virtual string SwitchThumbBorderColorOFF { get; set; }
        public virtual string SwitchThumbColorOFF { get; set; }
        public virtual string SwitchTrackBorderColorOFF { get; set; }
        public virtual string SwitchTrackColorOFF { get; set; }

        //Auto Complete
        public virtual string AutoCompleteBackgroundColor { get; set; }
        public virtual string AutoCompleteClearButtonColor { get; set; }
        public virtual string AutoCompleteDropdownBackgroundColor { get; set; }
        public virtual string AutoCompleteDropdownBorderColor { get; set; }
        public virtual string AutoCompleteDropdownTextColor { get; set; }
        public virtual string AutoCompleteHighlightedTextColor { get; set; }
        public virtual string AutoCompleteNoResultsFoundTextColor { get; set; }
        public virtual string AutoCompleteTextColor { get; set; }
        public virtual string AutoCompleteWaterColor { get; set; }

        //Details Page
        public virtual string SubPageTopHeaderBoxBorderColor { get; set; }
        public virtual IGradientSource SubPageTopHeaderBoxBorderBgSource { get; set; }
        public virtual string SubPageTopHeaderFontColor { get; set; }
        public virtual string SubPageTopHeaderWikiFontColor { get; set; }
        public virtual string SubPageHeader2GradientStartColor { get; set; }
        public virtual string SubPageHeader2GradientEndColor { get; set; }
        public virtual string SubPageHeader3GradientStartColor { get; set; }
        public virtual string SubPageHeader3GradientEndColor { get; set; }

        //List Items facts
        public virtual IGradientSource SubPageFactsBoxBorderBgSource { get; set; }
        public virtual string SubPageFactsFontColor { get; set; }

        //Picture title bottom box
        public virtual IGradientSource SubPagePictureTitleBoxBorderBgSource { get; set; }
        public virtual string SubPagePictureTitleBoxTextColor { get; set; }

        //Themes
        public virtual string AppShellTitleFontColor { get; set; }
        public virtual string AppShellFlyoutItemLabelFontColor { get; set; }
        public virtual string AppShellFlyoutItemSelectedBackgroundColor { get; set; }

        public virtual string AppShellBackgroundColor { get; set; }
        public virtual string AppShellForegroundIconColor { get; set; }

        //App Shell gradient
        public virtual string AppShellFooterGradientStart { get; set; }
        public virtual string AppShellFooterGradientEnd { get; set; }

        //Popup
        public virtual IGradientSource PopupHeaderBgSource { get; set; }
        public virtual IGradientSource PopupFooterBgSource { get; set; }
        public virtual IGradientSource PopupContentBgSource { get; set; }
    }
}
