using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Pj.Library;
using System.Linq;
using System.Windows.Input;
using WikiExtractor.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation.ViewModels
{
    public class PersonaDetailViewModel : BaseViewModel
    {
        public PersonaDetailViewModel()
        {
            selectedTabIndex = -1;
            var appInfo = DependencyService.Get<IAppInformation>();

            AdsInterstitialId = appInfo.AdsInterstitialId;
            AdsBannerId = appInfo.AdsBannerId;
            TextOnFirstTabInformationOnDetailPage = appInfo.TextOnFirstTabInformationOnDetailPage;
            CarouselImageLoadMoreItemsCount = appInfo.CarouselImageLoadMoreItemsCount;
            CarouselImageLoadComplete = false;
        }

        private int selectedTabIndex;
        public int SelectedTabIndex { get => selectedTabIndex; set => SetProperty(ref selectedTabIndex, value); }
        public ICommand TapHyperLinkToWikiPage => new Command<string>(async (url) => await Launcher.OpenAsync($"https://en.wikipedia.org/{url}"));

        private PersonaViewModel _persona;
        public PersonaViewModel Persona
        {
            get { return _persona; }
            set
            {
                _persona = value;
                OnPropertyChanged("Persona");
            }
        }

        public bool IsPicturesAvailable => Persona != null && Persona.Pictures != null && Persona.Pictures.Any(f => f.PicturePath != "NoImageAvailable.png");
        public bool IsPrimaryPictureAvailable => Persona != null && Persona.PicturePrimaryPath.HasValue();
        public bool IsMetaDataAvailable => Persona != null && Persona.Metadatas != null && Persona.Metadatas.Any();
        public bool IsDetailsAvailable => Persona != null && Persona.Paragraphs != null && Persona.Paragraphs.Any();

        private int? _availableCount;
        public int? AvailableTabCount
        {
            get
            {
                if (_availableCount == null)
                {
                    _availableCount = 0;
                    if (IsMetaDataAvailable) _availableCount++;
                    if (IsPicturesAvailable) _availableCount++;
                    if (IsDetailsAvailable) _availableCount++;
                }
                return _availableCount;
            }
        }

        private string _pictureTitle;
        public string PictureTitle
        {
            get
            {
                if (_pictureTitle == null)
                {
                    _pictureTitle = $"Pictures [{Persona?.Pictures.Count}]";
                }
                return _pictureTitle;
            }
            //set
            //{
            //    _pictureTitle =value;
            //    OnPropertyChanged("PictureTitle");
            //}
        }

        private string _currentSelectedPictureCaption;
        public string CurrentSelectedPictureCaption
        {
            get
            {
                return _currentSelectedPictureCaption;
            }
            set
            {
                _currentSelectedPictureCaption = value;
                OnPropertyChanged("CurrentSelectedPictureCaption");
            }
        }

        public bool CarouselImageLoadComplete { get; set; }
        public int CarouselImageTotalClicksToLoadComplete { get; set; }
        public int CarouselImageCurrentClickIndex { get; set; }
        public int CarouselImageLoadMoreItemsCount { get; set; }

        #region Ads
        private string _adsBannerId;
        public string AdsBannerId
        {
            get
            {
                return _adsBannerId;
            }
            set
            {
                _adsBannerId = value;
                OnPropertyChanged("AdsBannerId");
            }
        }

        private string _adsInterstitialId;
        public string AdsInterstitialId
        {
            get
            {
                return _adsInterstitialId;
            }
            set
            {
                _adsInterstitialId = value;
                OnPropertyChanged("AdsInterstitialId");
            }
        }
        #endregion

        public string TextOnFirstTabInformationOnDetailPage { get; set; }

        #region Style 
        private IStyleModel styleModelDefault;
        public IStyleModel DefaultStyle
        {
            get => styleModelDefault;
            set
            {
                styleModelDefault = value;
                OnPropertyChanged("DefaultStyle");
            }
        }

        #endregion
    }
}
