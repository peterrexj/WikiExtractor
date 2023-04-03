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

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Pope facts";
        public int CarouselImageLoadMoreItemsCount => 5;
    }
}