using WikiExtractor.Maui.App.Constants;
using WikiExtractor.Maui.App.Services;

namespace Maui.Saints.Platforms.Android.DependencyInjection
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

        public string AppShareLink => "https://play.google.com/store/apps/details?id=com.pj.saints.wiki";
        public string RateAppLink => "market://details?id=com.pj.saints.wiki";
        public string FeedbackEmail => "support@yoursimpleapps.com";

        public string AdsAppId => "ca-app-pub-4219645367584712~8231475165";

        public string AdsBannerId => "ca-app-pub-4219645367584712/9833451000";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/9085860639";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/3235891676";

        public string NoAdsProductId => "no_ads";
        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
