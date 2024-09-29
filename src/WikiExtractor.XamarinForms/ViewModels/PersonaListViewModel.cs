using System;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using Syncfusion.XForms.Buttons;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Syncfusion.XForms.EffectsView;
using WikiExtractor.ViewModels;
using WikiExtractor.XamarinForms.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation.ViewModels
{
    public class PersonaListViewModel : BaseViewModel
    {
        public ICommand TakeQuizCommand { get; set; }
        private bool _shouldProcessQuizRequest = true;

        public PersonaListViewModel()
        {
            StyleDrive = new StyleDrive
            {
                StyleOnImageHeightRequestOnListPage = DependencyService.Get<IAppInformation>().StyleOnImageHeightRequestOnListPage,
                //StyleOnListItemHeightRequestOnListPage = StylePropertyHelper.GetStyleOnListItemHeightRequestOnListPage()
            };

            SortByCollection = new System.Collections.ObjectModel.ObservableCollection<Syncfusion.XForms.Buttons.SfSegmentItem>
            {
                 new SfSegmentItem { Text = "Default" },
                 new SfSegmentItem { Text = "A-Z" },
                 new SfSegmentItem { Text = "Z-A" },
                 new SfSegmentItem { Text = "Read" },
                 new SfSegmentItem { Text = "UnRead" },
                 new SfSegmentItem { Text = "Random" }
            };

            PageCancellationTokenSource = new CancellationTokenSource();
            TakeQuizCommand = new Command<SfEffectsView>(TakeQuiz);
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
        public StyleDrive StyleDrive { get; set; }

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

        private async void TakeQuiz(SfEffectsView button)
        {
            IsBusy = true;

            await Task.Delay(200);

            if (_shouldProcessQuizRequest)
            {
                _shouldProcessQuizRequest = false;

                try
                {
                    await Shell.Current.GoToAsync($"{nameof(QuizPage)}");
                }
                catch (Exception e)
                {
                    ExceptionHandler.CaptureException(e);
                }

                _shouldProcessQuizRequest = true;
            }

            IsBusy = false;
        }
    }

    public class StyleDrive
    {
        public int StyleOnImageHeightRequestOnListPage { get; set; }
        public int StyleOnListItemHeightRequestOnListPage { get; set; }
    }
}
