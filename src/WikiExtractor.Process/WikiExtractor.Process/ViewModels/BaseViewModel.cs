namespace WikiExtractor.ViewModels
{
    public class BaseViewModel : BasePropertyChangeModel
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
                OnPropertyChanged("IsFree");
            }
        }
        public bool IsFree => !IsBusy;

        private bool _isPageBusy { get; set; }
        public bool IsPageBusy
        {
            get
            {
                return _isPageBusy;
            }
            set
            {
                _isPageBusy = value;
                OnPropertyChanged("IsPageBusy");
            }
        }

        private bool _isActive { get; set; }
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public string _bannerAdsUnitId;
        public string BannerAdsUnitId
        {
            get
            {
                return _bannerAdsUnitId;
            }
            set
            {
                _bannerAdsUnitId = value;
                OnPropertyChanged(nameof(BannerAdsUnitId));
            }
        }

        public bool AdsEnabled => WikiExtractor.Maui.App.Services.SharedServiceCore.AdsConfig.AdsEnabled;
    }
}
