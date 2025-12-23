using WikiExtractor.Maui.App.Services;

namespace Maui.Wiki.Platforms.Android.DependencyInjection
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "0bfcd53d-06a1-4bc3-86ab-03ca67d866e8";

        // Ads properties removed as per migration plan
        // public string AdsBannerId => "";
        // public string AdsQuizBannerId => "";
        // public string AdsInterstitialId => "";
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

        public string DbWikiStore => "WikiStore.db";
        public string DbUserStore => "WikiUserStore.db";
    }
}
