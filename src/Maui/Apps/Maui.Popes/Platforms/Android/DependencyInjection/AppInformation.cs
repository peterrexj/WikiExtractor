using WikiExtractor.Maui.App.Constants;using WikiExtractor.Maui.App.Services;

namespace Maui.Wiki.Platforms.Android.DependencyInjection
{
    public class AppInformation : IAppInformation
    {
        // public int ShowFirstInterstitialAdOnClickLimit => 0;
        // public int ShowLaterInterstitialAdOnClickLimit => 0;

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

        public string AdsAppId => "ca-app-pub-4219645367584712~1706236868";

        public string AdsBannerId => "ca-app-pub-3940256099942544/6300978111"; //"ca-app-pub-4219645367584712/5749138243";

        public string AdsQuizBannerId => "ca-app-pub-3940256099942544/6300978111"; //"ca-app-pub-4219645367584712/1177011646";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/3071004011";

        public string NoAdsProductId => "no_ads";
        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
