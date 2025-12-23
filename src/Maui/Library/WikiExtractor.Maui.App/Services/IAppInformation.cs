namespace WikiExtractor.Maui.App.Services
{
    public interface IAppInformation
    {
        string AppCentreAppKey { get; }
        
        // Ads properties removed as per migration plan
        
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