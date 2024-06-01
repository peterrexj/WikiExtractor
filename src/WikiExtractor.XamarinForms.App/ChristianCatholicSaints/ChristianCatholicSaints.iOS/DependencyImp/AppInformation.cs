using GeneralInformation.Services;
using Wiki.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.iOS
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "c335a6cd-9eaa-4382-97f1-0028cbd5c2c2";

        public string AdsBannerId => "ca-app-pub-4219645367584712/4824498021";
       
        public string AdsInterstitialId => "ca-app-pub-4219645367584712/6454657353";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        public bool DisplayAds => true;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Saint facts";

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreSaints.db";
        public string DbUserStore => "SaintsUserStore.db";
    }
}