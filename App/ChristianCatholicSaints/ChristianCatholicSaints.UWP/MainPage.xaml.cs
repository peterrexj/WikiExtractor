using Syncfusion.ListView.XForms.UWP;
using TestAny.Essentials.Core;

namespace ChristianCatholicSaints.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            SfListViewRenderer.Init();
            TestAnyAppConfig.InitializeFramework();
            LoadApplication(new GeneralInformation.App());
        }
    }
}
