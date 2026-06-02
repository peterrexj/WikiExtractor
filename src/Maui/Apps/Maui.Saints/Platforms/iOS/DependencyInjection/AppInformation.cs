using WikiExtractor.Maui.App.Constants;
using WikiExtractor.Maui.App.Services;

namespace Maui.Saints.Platforms.iOS.DependencyInjection
{
    public class AppInformation : IAppInformation
    {
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
        public string DbUserStore => "WikiUserStore.db";
        public string HeaderIcon => "appicon_1024.png";

        public string AppShareLink => "https://apps.apple.com/app/idYOUR_APP_ID"; // TODO: replace with App Store ID
        public string RateAppLink => "itms-apps://itunes.apple.com/app/idYOUR_APP_ID?action=write-review"; // TODO: replace with App Store ID
        public string FeedbackEmail => "support@yoursimpleapps.com";

        public string AdsAppId => "ca-app-pub-3940256099942544~1458002511";

        public string AdsBannerId => "ca-app-pub-3940256099942544/2934735716";

        public string AdsQuizBannerId => "ca-app-pub-3940256099942544/2934735716";

        public string AdsInterstitialId => "ca-app-pub-3940256099942544/4411468910";

        public string NoAdsProductId => "no_ads";
        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
