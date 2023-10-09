using Syncfusion.ListView.XForms.UWP;
using Syncfusion.XForms.UWP.PopupLayout;
using TestAny.Essentials.Core;

namespace PopesOfChurch.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            SfPopupLayoutRenderer.Init();
            SfListViewRenderer.Init();
            TestAnyAppConfig.InitializeFramework();

            LoadApplication(new GeneralInformation.App());
        }
    }
}
