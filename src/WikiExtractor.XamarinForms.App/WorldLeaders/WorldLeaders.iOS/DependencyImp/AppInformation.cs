using GeneralInformation.Services;
using Wiki.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.iOS
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "38dc98c2-b3a9-46d8-8f93-574b1d65131b";

        public string AdsBannerId => "ca-app-pub-4219645367584712/2856862813";
        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/1398942308";
        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4668637936";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion

        public string TextOnFirstTabInformationOnDetailPage => "Leaders facts";

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreWorldLeaders.db";
        public string DbUserStore => "WorldLeadersUserStore.db";
    }
}