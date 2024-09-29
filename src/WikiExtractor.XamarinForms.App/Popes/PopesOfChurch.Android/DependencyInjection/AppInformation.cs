using GeneralInformation.Services;
using Wiki.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.Droid
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "0bfcd53d-06a1-4bc3-86ab-03ca67d866e8";

        public string AdsBannerId => "ca-app-pub-4219645367584712/5749138243";
        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/1177011646";
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

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStorePopes.db";
        public string DbUserStore => "PopesUserStore.db";
    }
}