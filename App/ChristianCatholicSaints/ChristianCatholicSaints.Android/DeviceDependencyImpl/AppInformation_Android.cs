using ChristianCatholicSaints.Droid.DeviceDependencyImpl;
using GeneralInformation.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation_Android))]
namespace ChristianCatholicSaints.Droid.DeviceDependencyImpl
{
    public class AppInformation_Android : IAppInformation
    {
        public string AppCentreAppKeyDroid => "4a88ff7e-2001-4194-8ed2-3913845c6fe0";

        public string AdsBannerId => "ca-app-pub-4219645367584712/9833451000";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/3235891676";

        public int ShowFirstInterstitialAdOnClickLimit => 3;

        public int ShowLaterInterstitialAdOnClickLimit => 8;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        #endregion
    }
}