using GeneralInformation.Exts;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var appInfo = DependencyService.Get<IAppInformation>();

            PlayAudio = new Command<int>(async (id) => await SpeakNowDefaultSettings(id));

            AdsInterstitialId = appInfo.AdsInterstitialId;
            AdsBannerId = appInfo.AdsBannerId;
            TextOnFirstTabInformationOnDetailPage = appInfo.TextOnFirstTabInformationOnDetailPage;
        }

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

        #region Tab details
        public string TextOnFirstTabInformationOnDetailPage { get; set; }
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

        #endregion

        public void TriggerEvents()
        {
            OnPropertyChanged("Persona");
            OnPropertyChanged("PictureTitle");
            OnPropertyChanged("IsPrimaryPictureAvailable");
            OnPropertyChanged("IsMetaDataAvailable");
            OnPropertyChanged("IsDetailsAvailable");
            OnPropertyChanged("CurrentSelectedPictureCaption");
            OnPropertyChanged("CarouselImageLoadComplete");
            OnPropertyChanged("CarouselImageTotalClicksToLoadComplete");
            OnPropertyChanged("CarouselImageCurrentClickIndex");
            OnPropertyChanged("CarouselImageLoadMoreItemsCount");
            OnPropertyChanged("TextOnFirstTabInformationOnDetailPage");
            OnPropertyChanged("SelectedTabIndex");
            OnPropertyChanged("AvailableTabCount");
            OnPropertyChanged("IsPicturesAvailable");
        }

        public string PictureTitle => $"Pictures [{(IsPicturesAvailable ? Persona?.Pictures.Count : 0)}]";

        #region Popup Image
        private PictureViewModel _popupImage;
        public PictureViewModel PopupImage
        {
            get
            {
                return _popupImage;
            }
            set
            {
                _popupImage = value;
                OnPropertyChanged("PopupImage");
            }
        }
        #endregion

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

        #region Text To Speech Service
        public ICommand PlayAudio { get; set; }
        private CancellationTokenSource cts;
        private Stack<int> _playItems = new();
        public async Task SpeakNowDefaultSettings(int textToSpeechId)
        {
            try
            {
                // This method will block until utterance finishes.
                CancelSpeech();
                cts = new CancellationTokenSource();

                var contentForSpeech = GetContentsById(textToSpeechId);
                if (contentForSpeech.HasValue())
                {
                    _playItems.Push(textToSpeechId);
                }
                await TextToSpeech.SpeakAsync(contentForSpeech, await SettingsHelper.SpeechSettings(), cancelToken: cts.Token).ContinueWith(t =>
                {
                    _playItems.Pop();
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        public void CancelSpeech()
        {
            try
            {
                if (cts == null) return;

                if (!cts.IsCancellationRequested)
                {
                    if (_playItems.Count > 0)
                    {
                        cts.Cancel();

                    }
                }

                cts = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        #endregion

        private string GetContentsById(int id)
        {
            if (Persona.Paragraphs.Any(f => f.Id == id))
            {
                return string.Join(Environment.NewLine, Persona.Paragraphs.Where(f => f.Id == id).Select(f => f.Content));
            }
            else
            {
                return string.Join(Environment.NewLine, Persona.Paragraphs.SelectMany(f => f.Para3Containers).SelectMany(f => f.Para3s).Where(f => f.Id == id).Select(f => f.Content));
            }
        }
    }
}
