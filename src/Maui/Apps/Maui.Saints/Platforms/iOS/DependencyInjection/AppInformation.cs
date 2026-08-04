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

        public string AppShareLink => "https://apps.apple.com/app/id6470120151";
        public string RateAppLink => "itms-apps://itunes.apple.com/app/id6470120151?action=write-review";
        public string FeedbackEmail => "yoursimpleapps@gmail.com";

        public string AdsAppId => "ca-app-pub-4219645367584712~6335237852";

        public string AdsBannerId => "ca-app-pub-4219645367584712/4824498021";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/9258719634";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/6454657353";

        public string NoAdsProductId => "no_ads";

        public IReadOnlyList<OtherAppInfo> OtherApps => new[]
        {
            new OtherAppInfo("All Popes",        "Every pope in Catholic history",    "app_popes.png",       "https://apps.apple.com/app/id6471321897"),
            new OtherAppInfo("Countries",        "Explore every country on Earth",    "app_countries.png",   "https://apps.apple.com/app/id6472267884"),
            new OtherAppInfo("World Leaders",    "Heads of state past & present",     "app_worldleaders.png","https://apps.apple.com/app/id6505108906"),
        };

        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
