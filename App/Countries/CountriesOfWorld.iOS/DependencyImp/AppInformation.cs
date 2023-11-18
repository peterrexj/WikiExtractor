using GeneralInformation.Services;
using Wiki.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.iOS
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "7a906b29-071c-4855-a1fa-9891e2eb9f14";

        public string AdsBannerId => "ca-app-pub-4219645367584712/2073536552";
       
        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4887402158";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;
            
        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion

        public string TextOnFirstTabInformationOnDetailPage => "Country facts";

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreCountries.db";
        public string DbUserStore => "CountryUserStore.db";
    }
}