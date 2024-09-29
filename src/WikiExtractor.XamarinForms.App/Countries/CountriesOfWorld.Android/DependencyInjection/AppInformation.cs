using GeneralInformation.Services;
using Wiki.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.Droid
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "eda9cb2c-4d70-49ba-a98f-6e54d37873d2";

        public string AdsBannerId => "ca-app-pub-4219645367584712/3041169107";
        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/2354361942";
        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4901045689";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 100;

        public int StyleOnListItemHeightRequestOnListPagePhone => 250;
        public int StyleOnListItemHeightRequestOnListPageTablet => 250;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 250;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Country facts";

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreCountries.db";
        public string DbUserStore => "CountryUserStore.db";
    }
}