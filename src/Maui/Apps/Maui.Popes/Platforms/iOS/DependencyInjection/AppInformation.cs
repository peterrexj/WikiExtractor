using WikiExtractor.Maui.App.Constants;
using WikiExtractor.Maui.App.Services;

namespace Maui.Wiki.Platforms.iOS.DependencyInjection
{
    public class AppInformation : IAppInformation
    {
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
        public string DbUserStore => "WikiUserStore.db";
        public string HeaderIcon => "appicon_1024.png";

        public string AppShareLink => "https://apps.apple.com/app/id6471321897";
        public string RateAppLink => "itms-apps://itunes.apple.com/app/id6471321897?action=write-review";
        public string FeedbackEmail => "yoursimpleapps@gmail.com";

        public string AdsAppId => "ca-app-pub-4219645367584712~8734202306";

        public string AdsBannerId => "ca-app-pub-4219645367584712/8224449302";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/2322572355";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/1495389423";

        public string NoAdsProductId => "no_ads";

        public IReadOnlyList<OtherAppInfo> OtherApps => new[]
        {
            new OtherAppInfo("All Saints",       "Catholic saints & their stories",  "app_saints.png",      "https://apps.apple.com/app/id6470120151"),
            new OtherAppInfo("Countries",        "Explore every country on Earth",    "app_countries.png",   "https://apps.apple.com/app/id6472267884"),
            new OtherAppInfo("World Leaders",    "Heads of state past & present",     "app_worldleaders.png","https://apps.apple.com/app/id6505108906"),
        };

        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}