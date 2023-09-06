using Syncfusion.ListView.XForms.UWP;

namespace PopesOfChurch.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            SfListViewRenderer.Init();
            LoadApplication(new GeneralInformation.App());
        }
    }
}
