using PopesOfChurch.Droid.DeviceDependencyImpl;
using GeneralInformation.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation_Android))]
namespace PopesOfChurch.Droid.DeviceDependencyImpl
{
    public class AppInformation_Android : IAppInformation
    {
        public string AppCentreAppKeyDroid => "0bfcd53d-06a1-4bc3-86ab-03ca67d866e8";

        public string AdsBannerId => "ca-app-pub-4219645367584712/5749138243";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/3071004011";

        public int ShowFirstInterstitialAdOnClickLimit => 2;

        public int ShowLaterInterstitialAdOnClickLimit => 7;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 180;
        public int StyleOnListItemHeightRequestOnListPageTablet => 180;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 180;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Pope facts";
        public int CarouselImageLoadMoreItemsCount => 5;
    }
}