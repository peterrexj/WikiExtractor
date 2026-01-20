using System.Collections.ObjectModel;
using System.Windows.Input;
// using Syncfusion.Maui.Buttons; // Temporarily disabled
// using Syncfusion.Maui.Core; // Temporarily disabled
using Syncfusion.Maui.Buttons;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class PersonaListViewModel : BaseViewModel
    {
        public ICommand TakeQuizCommand { get; set; }
        
        private bool _shouldProcessQuizRequest = true;

        public PersonaListViewModel()
        {
            BannerAdsUnitId = SharedServiceCore.AdsConfig.BannerAdUnitId;

            SortByCollection = new System.Collections.ObjectModel.ObservableCollection<SfSegmentItem>
            {
                 new SfSegmentItem { Text = "Default" },
                 new SfSegmentItem { Text = "A-Z" },
                 new SfSegmentItem { Text = "Z-A" },
                 new SfSegmentItem { Text = "Read" },
                 new SfSegmentItem { Text = "UnRead" },
                 new SfSegmentItem { Text = "Random" }
            };

            PageCancellationTokenSource = new CancellationTokenSource();
            TakeQuizCommand = new Command<SfButton>(TakeQuiz);
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

        public bool IsPageEnabled => !IsDataLoading && !IsNavigating;

        private string _loadingMessage = "Loading list...";
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                OnPropertyChanged("LoadingMessage");
            }
        }

        private bool _isNavigating;
        public bool IsNavigating
        {
            get => _isNavigating;
            set
            {
                _isNavigating = value;
                OnPropertyChanged("IsNavigating");
                OnPropertyChanged("IsPageEnabled");
            }
        }

        private string _navigationMessage = "Opening details...";
        public string NavigationMessage
        {
            get => _navigationMessage;
            set
            {
                _navigationMessage = value;
                OnPropertyChanged("NavigationMessage");
            }
        }

        public string Title { get; set; }
        
        private IList<PersonaViewModel> _personas;
        public IList<PersonaViewModel> Personas { get => _personas; set => SetProperty(ref _personas, value); }

        private IEnumerable<PersonaAutoCompleteModel> _autoCompleteList;
        public IEnumerable<PersonaAutoCompleteModel> AutocompleteList
        {
            get => _autoCompleteList;
            set => SetProperty(ref _autoCompleteList, value);
        }
        public string SearchItemName { get; set; }

        public ICommand TapHyperLinkToWikiPage => new Command<string>(async (url) => await Launcher.OpenAsync($"https://en.wikipedia.org/{url}"));

        public ObservableCollection<SfSegmentItem> SortByCollection { get; set; }
        private int _sortBySelectedIndex;
        public int SortBySelectedIndex
        {
            get => _sortBySelectedIndex;
            set
            {
                _sortBySelectedIndex = value;
                OnPropertyChanged("SortBySelectedIndex");
            }
        }


        private bool hideItemRead;
        public bool HideItemRead
        {
            get => hideItemRead;
            set
            {
                hideItemRead = value;
                OnPropertyChanged("HideItemRead");
            }
        }

        #region PageCancellationTokenSource 
        private CancellationTokenSource pageCancellationTokenSource;
        public CancellationTokenSource PageCancellationTokenSource
        {
            get => pageCancellationTokenSource;
            set
            {
                pageCancellationTokenSource = value;
                OnPropertyChanged("PageCancellationTokenSource");
            }
        }
        #endregion

        private async void TakeQuiz(SfButton button)
        {
            IsPageBusy = true;

            await Task.Delay(200);

            if (_shouldProcessQuizRequest)
            {
                _shouldProcessQuizRequest = false;

                try
                {
                    await Shell.Current.GoToAsync($"QuizPage");
                }
                catch (Exception e)
                {
                    ExceptionHandler.CaptureException(e);
                }

                _shouldProcessQuizRequest = true;
            }

            IsPageBusy = false;
        }
    }
}