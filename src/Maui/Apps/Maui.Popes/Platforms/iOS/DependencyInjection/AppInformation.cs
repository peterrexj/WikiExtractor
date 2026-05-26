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

        public string AdsAppId => "ca-app-pub-4219645367584712~8734202306";

        public string AdsBannerId => "ca-app-pub-4219645367584712/8224449302";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/2322572355";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/1495389423";

        public string NoAdsProductId => "no_ads";
        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}