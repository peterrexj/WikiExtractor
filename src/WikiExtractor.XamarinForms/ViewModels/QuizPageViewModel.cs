using GeneralInformation;
using GeneralInformation.Exts;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using Pj.Library;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WikiExtractor.Process.DbModels;
using WikiExtractor.ViewModels;
using WikiExtractor.XamarinForms.Exts;
using WikiExtractor.XamarinForms.ViewModels.Charts;
using Xamarin.Forms;

namespace WikiExtractor.XamarinForms.ViewModels
{
    public class QuizPageViewModel : BaseViewModel
    {
        private readonly SfPopupLayout _sfPopup;
        public ICommand ClosePopupCommand { get; set; }

        public QuizPageViewModel(SfPopupLayout sfPopup)
        {
            _sfPopup = sfPopup;
            PageCancellationTokenSource = new CancellationTokenSource();
            ClosePopupCommand = new Command(ClosePopupAction);
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(Answer1));
                OnPropertyChanged(nameof(Answer2));
                OnPropertyChanged(nameof(Answer3));
                OnPropertyChanged(nameof(Answer4));
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public int TotalQuestions => Questions?.Count ?? 0;

        public QuizPageQuestionViewModel CurrentQuestion => Questions?.First(f => f.Id == CurrentIndex);
        public string Answer1 => CurrentQuestion?.AnswerCollection[0] ?? "";
        public string Answer2 => CurrentQuestion?.AnswerCollection[1] ?? "";
        public string Answer4 => CurrentQuestion?.AnswerCollection[3] ?? "";
        public string Answer3 => CurrentQuestion?.AnswerCollection[2] ?? "";
        public int ProgressValue => CurrentIndex * 10;

        private int _questionSetId;
        public int QuestionSetId
        {
            get => _questionSetId;
            set
            {
                _questionSetId = value;
                OnPropertyChanged(nameof(QuestionSetId));
            }
        }

        private DateTime _createdDateTime;
        public DateTime CreatedDateTime
        {
            get => _createdDateTime;
            set
            {
                _createdDateTime = value;
                OnPropertyChanged(nameof(CreatedDateTime));
            }
        }

        private ObservableCollection<QuizPageQuestionViewModel> _questions;
        public ObservableCollection<QuizPageQuestionViewModel> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                OnPropertyChanged(nameof(Questions));
                OnPropertyChanged(nameof(TotalQuestions));
            }
        }

        private IStyleModel _styleModelDefault;
        public IStyleModel DefaultStyle
        {
            get => _styleModelDefault;
            set
            {

                _styleModelDefault = value;
                OnPropertyChanged(nameof(DefaultStyle));

                CustomPaletteColors = new ObservableCollection<Color>
                {
                    Color.FromHex(DefaultStyle.ChartCorrectAnswerColor),
                    Color.FromHex(DefaultStyle.ChartWrongAnswerColor),
                    Color.FromHex(DefaultStyle.ChartNotAnsweredColor)
                };

                QuizCorrectAnswerColor = Color.FromHex(DefaultStyle.ChartCorrectAnswerColor);
                QuizWrongAnswerColor = Color.FromHex(DefaultStyle.ChartWrongAnswerColor);
                QuizAnswerDefaultBackColor = Color.FromHex(DefaultStyle.QuizAnswerDefaultBackColor);
                QuizAnswerSelectionBackColor = Color.FromHex(DefaultStyle.QuizAnswerSelectionBackColor);
            }
        }

        public Color QuizCorrectAnswerColor { get; set; }
        public Color QuizWrongAnswerColor { get; set; }
        public Color QuizAnswerDefaultBackColor { get; set; }
        public Color QuizAnswerSelectionBackColor { get; set; }

        public StyleDrive StyleDrive { get; set; } = new()
        {
            StyleOnImageHeightRequestOnListPage = DependencyService.Get<IAppInformation>().StyleOnImageHeightRequestOnListPage,
        };

        private CancellationTokenSource _pageCancellationTokenSource;
        public CancellationTokenSource PageCancellationTokenSource
        {
            get => _pageCancellationTokenSource;
            set
            {
                _pageCancellationTokenSource = value;
                OnPropertyChanged("PageCancellationTokenSource");
            }
        }

        public void Initialize()
        {
            IsBusy = true;
            try
            {
                DefaultStyle = ThemeHelper.GetDefaultStyle();

                QuestionSetId = SharedServices.QuizController.GetQuestionSetId();
                CreatedDateTime = System.DateTime.Now;

                Questions = new ObservableCollection<QuizPageQuestionViewModel>(
                    SharedServices.QuizController.GenerateQuizQuestionsForNewSession()
                        .Select(q => new QuizPageQuestionViewModel(q))
                        .ToList());
                Questions.Iter(q => q.DefaultStyle = DefaultStyle);
                CurrentIndex = 1;
            }
            catch (Exception e)
            {
                ExceptionHandler.CaptureException(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void SaveCurrentResponse()
        {
            try
            {
                var currentResponse = new QuizResponse
                {
                    MasterId = CurrentQuestion.MasterId,
                    CreatedDateTime = CreatedDateTime,
                    QuestionSetId = QuestionSetId,
                    UserResponse = CurrentQuestion.HasUserAnswered ? CurrentQuestion.IsCorrect ? 1 : 0 : -1,
                    MetadataKey = CurrentQuestion.MetadataKey
                };
                SharedServices.QuizController.SaveResponse(currentResponse);
            }
            catch (Exception e)
            {
                ExceptionHandler.CaptureException(e);
            }
        }

        #region  Popup

        public async void ShowSummaryPopup()
        {
            await Task.Run(() =>
            {
                ViewHelper.RunOnAppDispatcher(() =>
                {
                    if (_sfPopup == null) return;

                    _sfPopup.PopupView.IsFullScreen = true;
                    _sfPopup.ClosePopupOnBackButtonPressed = false;

                    _sfPopup.Show(true);
                });
            });
        }

        private async void ClosePopupAction()
        {
            _sfPopup?.Dismiss();
            await Task.Delay(200);
            await Shell.Current.GoToAsync("..");
        }

        #endregion

        private ObservableCollection<DataModel> _chartPassFailData;
        public ObservableCollection<DataModel> ChartPassFailData
        {
            get => _chartPassFailData;
            set
            {
                _chartPassFailData = value;
                OnPropertyChanged(nameof(ChartPassFailData));
            }
        }

        private ObservableCollection<Color> _customPaletteColors;
        public ObservableCollection<Color> CustomPaletteColors
        {
            get => _customPaletteColors;
            set
            {
                _customPaletteColors = value;
                OnPropertyChanged(nameof(CustomPaletteColors));
            }
        }

        public void CalculateSummary()
        {
            if (Questions?.Count == 0)
            {
                ChartPassFailData = new ObservableCollection<DataModel>
                {
                    new DataModel { Category = $"No quiz questions", Value = 0 }
                };
            }
            var correct = Questions.Count(q => q.HasUserAnswered && q.IsCorrect);
            var wrong = Questions.Count(q => q.HasUserAnswered && !q.IsCorrect);
            var notAnswered = Questions.Count(q => !q.HasUserAnswered);

            // Calculate percentages and round them
            var correctPercentage = Math.Round((double)correct / Questions.Count * 100, 0);
            var wrongPercentage = Math.Round((double)wrong / Questions.Count * 100, 0);
            var notAnsweredPercentage = Math.Round((double)notAnswered / Questions.Count * 100, 0);

            ChartPassFailData = new ObservableCollection<DataModel>
            {
                new DataModel { Category = $"Correct ({correctPercentage}%)", Value = correct },
                new DataModel { Category = $"Wrong ({wrongPercentage}%)", Value = wrong },
                new DataModel { Category = $"Not Answered ({notAnsweredPercentage}%)", Value = notAnswered }
            };
        }
    }
}
