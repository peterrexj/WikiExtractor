using Syncfusion.ListView.XForms.UWP;
using Syncfusion.XForms.UWP.PopupLayout;

namespace PopesOfChurch.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            SfPopupLayoutRenderer.Init();
            SfListViewRenderer.Init();

            LoadApplication(new GeneralInformation.App());
        }
    }
}
