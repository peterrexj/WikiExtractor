using WikiExtractor.Maui.App.Constants;
using WikiExtractor.Maui.App.Services;

namespace Maui.WorldLeaders.Platforms.iOS.DependencyInjection
{
    public class AppInformation : IAppInformation
    {
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
        public string HeaderIcon => "appicon_1024.png";

        public string AdsAppId => "ca-app-pub-4219645367584712~1266796586";

        public string AdsBannerId => "ca-app-pub-4219645367584712/2856862813";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/1398942308";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4668637936";

        public string NoAdsProductId => "no_ads";
        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
