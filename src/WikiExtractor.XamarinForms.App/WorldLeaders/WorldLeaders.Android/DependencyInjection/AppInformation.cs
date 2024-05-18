using GeneralInformation.Services;
using Xamarin.Forms;
using Wiki.Droid;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.Droid
{
    public class AppInformation : IAppInformation
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

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreWorldLeaders.db";
        public string DbUserStore => "WorldLeadersUserStore.db";
    }
}