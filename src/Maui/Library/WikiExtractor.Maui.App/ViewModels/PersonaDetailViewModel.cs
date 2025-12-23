using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Services;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WikiExtractor.ViewModels;
using WikiExtractor.Maui.App.ViewModels;
using Microsoft.Maui.Controls;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class PersonaDetailViewModel : BaseViewModel
    {
        public PersonaDetailViewModel()
        {
            ItemDetailItems = new ObservableCollection<ItemDetailListViewModel>();
            var appInfo = CustomServices.AppInformation;

            PlayAudio = new Command<int>(async (id) => await SpeakNowDefaultSettings(id));
            StopAudio = new Command(() => CancelSpeech());

            // Ads removed as per migration plan
            TextOnFirstTabInformationOnDetailPage = appInfo?.TextOnFirstTabInformationOnDetailPage ?? "Information";
        }

        private bool _isDataLoading;
        public bool IsDataLoading
        {
            get => _isDataLoading;
            set
            {
                _isDataLoading = value;
                OnPropertyChanged("IsDataLoading");
                OnPropertyChanged("IsPageEnabled");
            }
        }

        public bool IsPageEnabled => !IsDataLoading;

        private string _loadingMessage = "Loading details...";
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                OnPropertyChanged("LoadingMessage");
            }
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

        private ObservableCollection<ItemDetailListViewModel> _itemDetailItems;
        public ObservableCollection<ItemDetailListViewModel> ItemDetailItems
        {
            get => _itemDetailItems;
            set
            {
                _itemDetailItems = value;
                OnPropertyChanged("ItemDetailItems");
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
            // Commented out to prevent ListView scroll reset
            // OnPropertyChanged("Persona");
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
        public ICommand StopAudio { get; set; }
        private CancellationTokenSource cts;
        private Stack<int> _playItems = new();
        private int _currentPlayingId = -1;
        
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
                    _currentPlayingId = textToSpeechId;
                    
                    // Set the playing state for the current item
                    SetPlayingState(textToSpeechId, true);
                }
                
                await TextToSpeech.SpeakAsync(contentForSpeech, await SettingsHelper.SpeechSettings(), cancelToken: cts.Token).ContinueWith(t =>
                {
                    if (_playItems.Count > 0)
                    {
                        var playedId = _playItems.Pop();
                        // Reset the playing state when speech completes
                        Application.Current.Dispatcher.Dispatch(() =>
                        {
                            SetPlayingState(playedId, false);
                        });
                    }
                    _currentPlayingId = -1;
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
                        // Reset playing state for currently playing item
                        if (_currentPlayingId != -1)
                        {
                            SetPlayingState(_currentPlayingId, false);
                            _currentPlayingId = -1;
                        }
                    }
                }

                cts = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void SetPlayingState(int contentLinkId, bool isPlaying)
        {
            try
            {
                var item = ItemDetailItems?.FirstOrDefault(x => x.ContentLinkId == contentLinkId);
                if (item != null)
                {
                    item.IsPlaying = isPlaying;
                }
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

        public void CleanupResources()
        {
            CancelSpeech();
            cts?.Dispose();
        }
    }
}