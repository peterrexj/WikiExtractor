using WikiExtractor.Maui.App.Constants;
using WikiExtractor.Maui.App.Services;

namespace Maui.Countries.Platforms.Android.DependencyInjection
{
    public class AppInformation : IAppInformation
    {
        #region Style Implementation
        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion

        public string TextOnFirstTabInformationOnDetailPage => "Country facts";

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;

        public string DbWikiStore => "WikiStoreCountries.db";
        public string DbUserStore => "CountryUserStore.db";
        public string HeaderIcon => "appicon_1024.png";

        public string AdsAppId => "ca-app-pub-4219645367584712~3489544050";

        public string AdsBannerId => "ca-app-pub-4219645367584712/3041169107";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/2354361942";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4901045689";

        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
