using Syncfusion.ListView.XForms.UWP;
using Syncfusion.XForms.UWP.PopupLayout;

namespace ChristianCatholicSaints.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            SfListViewRenderer.Init();
            SfPopupLayoutRenderer.Init();
            LoadApplication(new GeneralInformation.App());
        }
    }
}
