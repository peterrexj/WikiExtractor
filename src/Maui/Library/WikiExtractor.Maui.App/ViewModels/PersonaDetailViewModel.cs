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

            PlayAudio = new Command<int>(async (id) => await SpeakNowDefaultSettings(id));
            StopAudio = new Command(() => CancelSpeech());

            // Ads removed as per migration plan
            TextOnFirstTabInformationOnDetailPage = SharedServiceCore.AppInformation?.TextOnFirstTabInformationOnDetailPage ?? "Information";
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

        #region Text To Speech Service
        public ICommand PlayAudio { get; set; }
        public ICommand StopAudio { get; set; }
        private CancellationTokenSource cts;
        private int _currentPlayingId = -1;
        
        public async Task SpeakNowDefaultSettings(int textToSpeechId)
        {
            try
            {
                // 1. Stop any current speech and IMMEDIATELY reset all UI buttons
                CancelSpeech();

                await Task.Delay(100);

                // 2. Prepare new speech
                cts = new CancellationTokenSource();
                var contentForSpeech = GetContentsById(textToSpeechId);

                if (contentForSpeech.HasValue())
                {
                    _currentPlayingId = textToSpeechId;

                    // 3. Update UI to "Stop" icon for the NEW item
                    SetPlayingState(textToSpeechId, true);
                }

                try
                {
                    // 4. Await the speech directly instead of using ContinueWith
                    // This keeps the logic linear and predictable
                    await TextToSpeech.SpeakAsync(contentForSpeech,
                        await SettingsHelper.SpeechSettings(),
                        cancelToken: cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Speech was cancelled by the user or a new speech request
                    // We handle this silently as it's expected behavior
                }
                finally
                {
                    // 5. Always reset the UI when done, failed, or cancelled
                    // Only reset if this specific task is still the "current" one
                    if (_currentPlayingId == textToSpeechId)
                    {
                        SetPlayingState(textToSpeechId, false);
                        _currentPlayingId = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                SetPlayingState(textToSpeechId, false);
            }
        }

        public void CancelSpeech()
        {
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }

                // Force-reset the UI for the item that was playing
                if (_currentPlayingId != -1)
                {
                    SetPlayingState(_currentPlayingId, false);
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