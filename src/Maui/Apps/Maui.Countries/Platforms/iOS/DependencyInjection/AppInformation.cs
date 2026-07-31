using WikiExtractor.Maui.App.Constants;
using WikiExtractor.Maui.App.Services;

namespace Maui.Countries.Platforms.iOS.DependencyInjection
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

        public string AppShareLink => "https://apps.apple.com/app/id6472267884";
        public string RateAppLink => "itms-apps://itunes.apple.com/app/id6472267884?action=write-review";
        public string FeedbackEmail => "yoursimpleapps@gmail.com";

        public string AdsAppId => "ca-app-pub-4219645367584712~1323561667";

        public string AdsBannerId => "ca-app-pub-4219645367584712/2073536552";

        public string AdsQuizBannerId => "ca-app-pub-4219645367584712/5146615629";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/4887402158";

        public string NoAdsProductId => "no_ads";
        public List<string> GetRegisteredFontFamilies()
        {
            return RegisteredFonts.GetFontFamilies();
        }
    }
}
