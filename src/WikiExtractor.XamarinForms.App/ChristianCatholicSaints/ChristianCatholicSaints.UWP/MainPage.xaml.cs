using Syncfusion.ListView.XForms.UWP;
using Syncfusion.XForms.UWP.PopupLayout;
using TestAny.Essentials.Core;

namespace ChristianCatholicSaints.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            SfListViewRenderer.Init();
            SfPopupLayoutRenderer.Init();
            TestAnyAppConfig.InitializeFramework();
            LoadApplication(new GeneralInformation.App());
        }
    }
}
