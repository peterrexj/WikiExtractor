namespace WikiExtractor.Maui.App.Services
{
    public interface IAppInformation
    {
        public string AdsAppId { get; }
        public string AdsBannerId { get; }
        public string AdsQuizBannerId { get; }
        public string AdsInterstitialId { get; }


        string ImageCacheFolder { get; }
        int ImageCacheTotalDaysToInvalidate { get; }

        int StyleOnImageHeightRequestOnListPage { get; }
        int StyleOnListItemHeightRequestOnListPagePhone { get; }
        int StyleOnListItemHeightRequestOnListPageTablet { get; }
        int StyleOnListItemHeightRequestOnListPageDesktop { get; }

        string TextOnFirstTabInformationOnDetailPage { get; }

        string DbWikiStore { get; }
        string DbUserStore { get; }
    }
}