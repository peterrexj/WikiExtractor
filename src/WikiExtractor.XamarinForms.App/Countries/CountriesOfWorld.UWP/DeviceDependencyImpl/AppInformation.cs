using GeneralInformation.Services;
using Wiki.Uwp;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.Uwp
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "92149c55-3f37-4263-9b9d-26f7eedbd8fc";

        public string AdsBannerId => "ca-app-pub-4219645367584712/3041169107";
        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/2354361942";
        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4901045689";

        public int ShowFirstInterstitialAdOnClickLimit => 3;

        public int ShowLaterInterstitialAdOnClickLimit => 8;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 148;
        public int StyleOnListItemHeightRequestOnListPageTablet => 128;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 128;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Country facts";

        public string ImageCacheFolder => ApplicationData.Current.LocalFolder.Path;
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreCountries.db";
        public string DbUserStore => "CountryUserStore.db";
    }
}