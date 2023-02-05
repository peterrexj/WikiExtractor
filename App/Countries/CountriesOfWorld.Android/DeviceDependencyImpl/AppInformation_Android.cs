using CountriesOfWorld.Droid.DeviceDependencyImpl;
using GeneralInformation.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation_Android))]
namespace CountriesOfWorld.Droid.DeviceDependencyImpl
{
    public class AppInformation_Android : IAppInformation
    {
        public string AppCentreAppKeyDroid => "eda9cb2c-4d70-49ba-a98f-6e54d37873d2";

        public string AdsBannerId => "ca-app-pub-4219645367584712/3041169107";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4901045689";

        public int ShowFirstInterstitialAdOnClickLimit => 3;

        public int ShowLaterInterstitialAdOnClickLimit => 8;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 100;

        public int StyleOnListItemHeightRequestOnListPagePhone => 250;
        public int StyleOnListItemHeightRequestOnListPageTablet => 250;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 250;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Country facts";
        public int CarouselImageLoadMoreItemsCount => 5;
    }
}