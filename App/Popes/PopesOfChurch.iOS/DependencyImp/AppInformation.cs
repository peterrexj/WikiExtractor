using GeneralInformation.Services;
using Wiki.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation))]
namespace Wiki.iOS
{
    public class AppInformation : IAppInformation
    {
        public string AppCentreAppKey => "2814afe4-9e78-4962-896c-ba5e70506ac2";

        public string AdsBannerId => "ca-app-pub-4219645367584712/8224449302";
       
        public string AdsInterstitialId => "ca-app-pub-4219645367584712/1495389423";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion

        public string TextOnFirstTabInformationOnDetailPage => "Popes facts";
        public int CarouselImageLoadMoreItemsCount => 5;

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;
    }
}