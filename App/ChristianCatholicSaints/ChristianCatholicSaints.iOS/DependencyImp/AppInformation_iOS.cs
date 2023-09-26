using ChristianCatholicSaints.iOS.DependencyImp;
using GeneralInformation.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppInformation_iOS))]
namespace ChristianCatholicSaints.iOS.DependencyImp
{
    public class AppInformation_iOS : IAppInformation
    {
        public string AppCentreAppKey => "c335a6cd-9eaa-4382-97f1-0028cbd5c2c2";

        public string AdsBannerId => "ca-app-pub-4219645367584712/9833451000";

        public string AdsInterstitialId => "ca-app-pub-4219645367584712/3235891676";

        public int ShowFirstInterstitialAdOnClickLimit => 1;

        public int ShowLaterInterstitialAdOnClickLimit => 5;

        #region Style Implementation

        public int StyleOnImageHeightRequestOnListPage => 130;

        public int StyleOnListItemHeightRequestOnListPagePhone => 200;
        public int StyleOnListItemHeightRequestOnListPageTablet => 200;
        public int StyleOnListItemHeightRequestOnListPageDesktop => 200;
        #endregion
        public string TextOnFirstTabInformationOnDetailPage => "Saint facts";
        public int CarouselImageLoadMoreItemsCount => 5;

        public string ImageCacheFolder => System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        public int ImageCacheTotalDaysToInvalidate => 30;
    }
}