namespace GeneralInformation.Services
{
    public interface IAppInformation
    {
        string AppCentreAppKey { get; }
        string AdsBannerId { get; }
        string AdsInterstitialId { get; }
        int ShowFirstInterstitialAdOnClickLimit { get; }
        int ShowLaterInterstitialAdOnClickLimit { get; }

        string ImageCacheFolder { get; }
        int ImageCacheTotalDaysToInvalidate { get; }

        int StyleOnImageHeightRequestOnListPage { get; }
        int StyleOnListItemHeightRequestOnListPagePhone { get; }
        int StyleOnListItemHeightRequestOnListPageTablet { get; }
        int StyleOnListItemHeightRequestOnListPageDesktop { get; }

        string TextOnFirstTabInformationOnDetailPage { get; }
        int CarouselImageLoadMoreItemsCount { get; }
    }
}
