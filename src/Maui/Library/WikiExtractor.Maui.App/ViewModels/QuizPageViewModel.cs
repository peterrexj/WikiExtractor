using Pj.Library;
// using Syncfusion.Maui.Popup; // Temporarily disabled
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WikiExtractor.Process.DbModels;
using WikiExtractor.ViewModels;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.ViewModels.Charts;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.Core;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;

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

        public QuizPageViewModel()
        {
            PageCancellationTokenSource = new CancellationTokenSource();
            ClosePopupCommand = new Command(ClosePopupAction);
            NextQuestionCommand = new Command(async () => await OnNextClick(null));
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

        public QuizPageQuestionViewModel CurrentQuestion => Questions?.FirstOrDefault(f => f.Id == CurrentIndex);
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

        // Hardcoded quiz colors to avoid DefaultStyle dependency
        public string QuizCorrectAnswerColor { get; set; } = "#4CAF50"; // Green
        public string QuizWrongAnswerColor { get; set; } = "#F44336"; // Red
        public string QuizAnswerDefaultBackColor { get; set; } = "#E0E0E0"; // Light Gray
        public string QuizAnswerSelectionBackColor { get; set; } = "#2196F3"; // Blue

        private void InitializeQuizColors()
        {
            CustomChartColors = new ObservableCollection<Microsoft.Maui.Controls.Brush>
            {
                new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(QuizCorrectAnswerColor)),
                new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(QuizWrongAnswerColor)),
                new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb("#9E9E9E")) // Gray for not answered
            };

            Answer1BackColor = QuizAnswerDefaultBackColor;
            Answer2BackColor = QuizAnswerDefaultBackColor;
            Answer3BackColor = QuizAnswerDefaultBackColor;
            Answer4BackColor = QuizAnswerDefaultBackColor;
        }

        private string _answer1BackColor;
        public string Answer1BackColor
        {
            get => _answer1BackColor;
            set
            {
                _answer1BackColor = value;
                OnPropertyChanged(nameof(Answer1BackColor));
            }
        }

        private string _answer2BackColor;
        public string Answer2BackColor
        {
            get => _answer2BackColor;
            set
            {
                _answer2BackColor = value;
                OnPropertyChanged(nameof(Answer2BackColor));
            }
        }

        private string _answer3BackColor;
        public string Answer3BackColor
        {
            get => _answer3BackColor;
            set
            {
                _answer3BackColor = value;
                OnPropertyChanged(nameof(Answer3BackColor));
            }
        }

        private string _answer4BackColor;
        public string Answer4BackColor
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
            IsBusy = true;
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
                
                // No need to set DefaultStyle on questions anymore
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

        #region Navigation to Results

        public async void ShowQuizResults()
        {
            try
            {
                // Ensure navigation happens on the main UI thread
                await Application.Current.Dispatcher.DispatchAsync(async () =>
                {
                    // Navigate to QuizResultsPage with the quiz data using relative routing
                    await Shell.Current.GoToAsync("QuizResultsPage", new Dictionary<string, object>
                    {
                        ["Questions"] = Questions,
                        ["ChartData"] = ChartPassFailData,
                        ["ChartColors"] = CustomChartColors
                    });
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                // Fallback - just go back if navigation fails
                await Application.Current.Dispatcher.DispatchAsync(async () =>
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

        private ObservableCollection<Microsoft.Maui.Controls.Brush> _customChartColors;
        public ObservableCollection<Microsoft.Maui.Controls.Brush> CustomChartColors
        {
            get => _customChartColors;
            set
            {
                _customChartColors = value;
                OnPropertyChanged(nameof(CustomChartColors));
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

        public async Task OnNextClick(SfBusyIndicator busyIndicator = null)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        await ViewHelper.RunOnAppDispatcherAsync(() => { busyIndicator?.SetValue(SfBusyIndicator.IsRunningProperty, true); });
                        
                        // Mapping answer options to their corresponding labels
                        var answerLabels = new Dictionary<object, int>
                        {
                            { Answer1, 1 },
                            { Answer2, 2 },
                            { Answer3, 3 },
                            { Answer4, 4 }
                        };

                        // Apply color to the selected answer
                        var selectedAnswer = CurrentQuestion.UserSelection;
                        if (selectedAnswer != null)
                        {
                            var isCorrect = CurrentQuestion.IsCorrect;
                            ApplyAnswerColor(answerLabels[selectedAnswer], isCorrect);

                            // Highlight the correct answer if the selected answer is incorrect
                            if (!isCorrect)
                            {
                                var correctAnswer = CurrentQuestion.CorrectAnswer;
                                ApplyAnswerColor(answerLabels[correctAnswer], true);
                            }

                            await Task.Delay(1000);

                            SaveCurrentResponse();

                            AnswersSetDefaultColor();
                        }

                        if (CurrentIndex == Questions.Count)
                        {
                            CalculateSummary();
                            ShowQuizResults();
                        }
                        else if (Questions.Count > CurrentIndex)
                        {
                            CurrentIndex += 1;
                        }
                    }
                    catch (Exception innerException)
                    {
                        ExceptionHandler.CaptureException(innerException);
                    }
                    finally
                    {
                        await ViewHelper.RunOnAppDispatcherAsync(() => { busyIndicator?.SetValue(SfBusyIndicator.IsRunningProperty, false); });
                    }
                });
            }
            catch (Exception outerException)
            {
                ExceptionHandler.CaptureException(outerException);
            }
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