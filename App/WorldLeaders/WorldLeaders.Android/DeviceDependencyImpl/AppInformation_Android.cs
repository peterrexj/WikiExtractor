using WorldLeaders.Droid.DeviceDependencyImpl;
using GeneralInformation.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation_Android))]
namespace WorldLeaders.Droid.DeviceDependencyImpl
{
    public class AppInformation_Android : IAppInformation
    {
        public string AppCentreAppKey => "4a88ff7e-2001-4194-8ed2-3913845c6fe0";

        public string AdsBannerId => "ca-app-pub-4219645367584712/5897223389";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4584141715";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Leader facts";
        public int CarouselImageLoadMoreItemsCount => 5;
    }
}