using Syncfusion.Maui.Core;
// using Syncfusion.Maui.Popup; // Temporarily disabled
using System.Collections.ObjectModel;
using System.Windows.Input;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Models;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels.Charts;
using WikiExtractor.Process.DbModels;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class QuizPageViewModel : BaseViewModel
    {
        // Temporary replacement for SfPopup - using simple boolean flag
        
        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged(nameof(IsPopupOpen));
            }
        }
        
        public ICommand ClosePopupCommand { get; set; }
        public ICommand NextQuestionCommand { get; set; }
        public ICommand ExitQuizCommand { get; set; }

        public ICommand AnswerSelectedCommand { get; set; }

        public Color QuizCorrectAnswerColor { get; set; }
        public Color QuizWrongAnswerColor { get; set; }
        public Color QuizAnswerDefaultBackColor { get; set; }
        public Color QuizAnswerSelectionBackColor { get; set; }
        public Color QuizProgressDefaultColor { get; set; }
        public Color QuizProgressCorrectColor { get; set; }
        public Color QuizProgressWrongColor { get; set; }
        public Color QuizProgressSkipColor { get; set; }

        private QuizThemeData? _theme = null;
        private bool _isProcessingNext = false;
        public bool IsNextEnabled => !_isProcessingNext;

        public QuizPageViewModel()
        {
            PageCancellationTokenSource = new CancellationTokenSource();
            ClosePopupCommand = new Command(ClosePopupAction);
            NextQuestionCommand = new Command<SfBusyIndicator>(async (loader) => await OnNextClick(loader));
            AnswerSelectedCommand = new Command<int>(async (index) => await OnAnswerClick(index));
            ExitQuizCommand = new Command(async () => await ExitQuiz());
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

        public QuizPageQuestionViewModel? CurrentQuestion => Questions?.FirstOrDefault(f => f.Id == CurrentIndex);
        public string Answer1 => CurrentQuestion?.AnswerCollection?[0] ?? "";
        public string Answer2 => CurrentQuestion?.AnswerCollection?[1] ?? "";
        public string Answer3 => CurrentQuestion?.AnswerCollection?[2] ?? "";
        public string Answer4 => CurrentQuestion?.AnswerCollection?[3] ?? "";
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

        private Color _answer1BackColor;
        public Color Answer1BackColor
        {
            get => _answer1BackColor;
            set
            {
                _answer1BackColor = value;
                OnPropertyChanged(nameof(Answer1BackColor));
            }
        }

        private Color _answer2BackColor;
        public Color Answer2BackColor
        {
            get => _answer2BackColor;
            set
            {
                _answer2BackColor = value;
                OnPropertyChanged(nameof(Answer2BackColor));
            }
        }

        private Color _answer3BackColor;
        public Color Answer3BackColor
        {
            get => _answer3BackColor;
            set
            {
                _answer3BackColor = value;
                OnPropertyChanged(nameof(Answer3BackColor));
            }
        }

        private Color _answer4BackColor;
        public Color Answer4BackColor
        {
            get => _answer4BackColor;
            set
            {
                _answer4BackColor = value;
                OnPropertyChanged(nameof(Answer4BackColor));
            }
        }

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

        public bool CanGoBack => true;

        public async Task InitializeAsync()
        {
            try
            {
                // Initialize quiz colors without DefaultStyle dependency
                InitializeQuizColors();

                QuestionSetId = SharedServices.QuizController.GetQuestionSetId();
                CreatedDateTime = System.DateTime.Now;

                Questions = new ObservableCollection<QuizPageQuestionViewModel>(
                    SharedServices.QuizController.GenerateQuizQuestionsForNewSession()
                        .Select(q => new QuizPageQuestionViewModel(q))
                        .ToList());

                // Seed segment colors after questions are created
                var defaultSegColor = Application.Current.Resources.TryGetValue("WikiAppListItemBoxBorderColor", out var dc) ? (Color)dc : Color.FromArgb("#888888");
                foreach (var q in Questions)
                    q.SegmentColor = defaultSegColor;

                CurrentIndex = 1;

                BannerAdsUnitId = SharedServiceCore.AdsConfig.QuizBannerAdUnitId ?? SharedServiceCore.AdsConfig.BannerAdUnitId;
            }
            catch (Exception e)
            {
                ExceptionHandler.CaptureException(e);
                throw;
            }
            finally
            {
                IsPageBusy = false;
            }
        }

        private async void InitializeQuizColors()
        {
            // This awaits the background task we started in App.xaml.cs
            _theme = await SharedServiceCore.ThemeHandler.GetThemeDataAsync();

            // Assign to properties that call OnPropertyChanged
            QuizCorrectAnswerColor = _theme.CorrectColor;
            QuizWrongAnswerColor = _theme.WrongColor;
            QuizAnswerDefaultBackColor = _theme.DefaultBackColor;
            QuizAnswerSelectionBackColor = _theme.SelectionBackColor;

            // Progress bar segment colors
            QuizProgressDefaultColor = Application.Current.Resources.TryGetValue("WikiAppListItemBoxBorderColor", out var defCol) ? (Color)defCol : Color.FromArgb("#888888");
            QuizProgressCorrectColor = Color.FromArgb("#7EC8A0");  // soft green
            QuizProgressWrongColor = Color.FromArgb("#E88080");    // soft red
            QuizProgressSkipColor = Color.FromArgb("#C8A06E");     // soft amber for skipped

            Answer1BackColor = QuizAnswerDefaultBackColor;
            Answer2BackColor = QuizAnswerDefaultBackColor;
            Answer3BackColor = QuizAnswerDefaultBackColor;
            Answer4BackColor = QuizAnswerDefaultBackColor;

            // Initialize all question segment colors to default
            if (Questions != null)
            {
                foreach (var q in Questions)
                    q.SegmentColor = QuizProgressDefaultColor;
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

        #region Navigation to Results

        public async Task ShowQuizResults()
        {
            try
            {
                // Ensure navigation happens on the main UI thread
                await ViewHelper.RunOnAppDispatcherAsync(async () =>
                {
                    // Navigate to QuizResultsPage with the quiz data using relative routing
                    await Shell.Current.GoToAsync("QuizResultsPage", new Dictionary<string, object>
                    {
                        ["Questions"] = Questions,
                        ["ChartData"] = ChartPassFailData,
                    });
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                // Fallback - just go back if navigation fails
                await ViewHelper.RunOnAppDispatcherAsync(async () =>
                {
                    await Shell.Current.GoToAsync("..");
                });
            }
        }

        private async void ClosePopupAction()
        {
            // This method is kept for compatibility but redirects to exit
            await ExitQuiz();
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

        public void CalculateSummary()
        {
            if (Questions?.Count == 0)
            {
                ChartPassFailData = new ObservableCollection<DataModel>
                {
                    new DataModel { Category = $"No quiz questions", Value = 0 }
                };
                return;
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

        #region Answer Interaction

        public void AnswersSetDefaultColor()
        {
            Answer1BackColor = QuizAnswerDefaultBackColor;
            Answer2BackColor = QuizAnswerDefaultBackColor;
            Answer3BackColor = QuizAnswerDefaultBackColor;
            Answer4BackColor = QuizAnswerDefaultBackColor;
        }

        public async Task OnAnswerClick(int answerIndex)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        ViewHelper.RunOnAppDispatcherAsync(() =>
                        {
                            // Reset all labels to default color
                            AnswersSetDefaultColor();

                            // Apply selection color to the selected answer
                            switch (answerIndex)
                            {
                                case 1:
                                    Answer1BackColor = QuizAnswerSelectionBackColor;
                                    CurrentQuestion.UserSelection = Answer1;
                                    break;
                                case 2:
                                    Answer2BackColor = QuizAnswerSelectionBackColor;
                                    CurrentQuestion.UserSelection = Answer2;
                                    break;
                                case 3:
                                    Answer3BackColor = QuizAnswerSelectionBackColor;
                                    CurrentQuestion.UserSelection = Answer3;
                                    break;
                                case 4:
                                    Answer4BackColor = QuizAnswerSelectionBackColor;
                                    CurrentQuestion.UserSelection = Answer4;
                                    break;
                            }
                        });
                    }
                    catch (Exception innerException)
                    {
                        ExceptionHandler.CaptureException(innerException);
                    }
                });
            }
            catch (Exception outerException)
            {
                ExceptionHandler.CaptureException(outerException);
            }
        }

        public async Task OnNextClick(SfBusyIndicator busyIndicator)
        {
            if (_isProcessingNext) return;
            _isProcessingNext = true;
            OnPropertyChanged(nameof(IsNextEnabled));
            // 1. Immediate UI update (Main Thread)
            IsPageBusy = true;

            try
            {
                var selectedAnswer = CurrentQuestion.UserSelection;
                if (selectedAnswer != null)
                {
                    // 2. Visual Feedback (Must be Main Thread)
                    var answerLabels = new Dictionary<object, int>
                    {
                        { Answer1, 1 }, { Answer2, 2 }, { Answer3, 3 }, { Answer4, 4 }
                    };

                    var isCorrect = CurrentQuestion.IsCorrect;
                    ApplyAnswerColor(answerLabels[selectedAnswer], isCorrect);

                    if (!isCorrect)
                    {
                        ApplyAnswerColor(answerLabels[CurrentQuestion.CorrectAnswer], true);
                    }

                    // Pause to let user see the answer
                    await Task.Delay(1000);

                    if (CurrentIndex != Questions.Count)
                    {
                        await Task.Run(() => SaveCurrentResponse());
                    }
                    AnswersSetDefaultColor();
                }

                // 4. Navigation Logic
                if (CurrentIndex == Questions.Count)
                {
                    // Color the last question segment before results
                    UpdateCurrentSegmentColor(selectedAnswer != null);
                    await Task.Delay(500);
                    await Task.WhenAll(
                        Task.Run(() => SaveCurrentResponse()),
                        Task.Run(() => CalculateSummary())
                    );

                    await ShowQuizResults();
                    return;
                }
                else if (Questions.Count > CurrentIndex)
                {
                    UpdateCurrentSegmentColor(selectedAnswer != null);
                    CurrentIndex += 1;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                // 5. Turn off loader (Main Thread)
                IsPageBusy = false;
                _isProcessingNext = false;
                OnPropertyChanged(nameof(IsNextEnabled));
            }
        }

        private void UpdateCurrentSegmentColor(bool wasAnswered)
        {
            var question = CurrentQuestion;
            if (question == null) return;
            Application.Current.Dispatcher.Dispatch(() =>
            {
                if (!wasAnswered)
                    question.SegmentColor = QuizProgressSkipColor;
                else if (question.IsCorrect)
                    question.SegmentColor = QuizProgressCorrectColor;
                else
                    question.SegmentColor = QuizProgressWrongColor;
            });
        }

        private void ApplyAnswerColor(int answerPosition, bool isCorrect)
        {
            Application.Current.Dispatcher.Dispatch(() =>
            {
                if (answerPosition == 1)
                {
                    Answer1BackColor = isCorrect ? QuizCorrectAnswerColor : QuizWrongAnswerColor;
                }
                else if (answerPosition == 2)
                {
                    Answer2BackColor = isCorrect ? QuizCorrectAnswerColor : QuizWrongAnswerColor;
                }
                else if (answerPosition == 3)
                {
                    Answer3BackColor = isCorrect ? QuizCorrectAnswerColor : QuizWrongAnswerColor;
                }
                else if (answerPosition == 4)
                {
                    Answer4BackColor = isCorrect ? QuizCorrectAnswerColor : QuizWrongAnswerColor;
                }
            });
        }
        
        #endregion
        
        
        private async Task ExitQuiz()
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        public void CleanupResources()
        {
            PageCancellationTokenSource?.Cancel();
            PageCancellationTokenSource?.Dispose();
        }
    }
}