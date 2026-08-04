namespace WikiExtractor.Maui.App.Services
{
    public record OtherAppInfo(string Name, string Description, string IconImage, string StoreUrl);

    public interface IAppInformation
    {
        public string AdsAppId { get; }
        public string AdsBannerId { get; }
        public string AdsQuizBannerId { get; }
        public string AdsInterstitialId { get; }
        public string NoAdsProductId { get; }


        string ImageCacheFolder { get; }
        int ImageCacheTotalDaysToInvalidate { get; }

        int StyleOnImageHeightRequestOnListPage { get; }
        int StyleOnListItemHeightRequestOnListPagePhone { get; }
        int StyleOnListItemHeightRequestOnListPageTablet { get; }
        int StyleOnListItemHeightRequestOnListPageDesktop { get; }

        string TextOnFirstTabInformationOnDetailPage { get; }

        string DbWikiStore { get; }
        string DbUserStore { get; }

        string HeaderIcon { get; }

        string AppShareLink { get; }
        string RateAppLink { get; }
        string FeedbackEmail { get; }

        IReadOnlyList<OtherAppInfo> OtherApps { get; }

        /// <summary>
        /// Gets the list of registered font families available in the application
        /// </summary>
        /// <returns>List of font family names</returns>
        List<string> GetRegisteredFontFamilies();
    }
}