using GeneralInformation;
using GeneralInformation.Exts;
using GeneralInformation.Models.Mix;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using Pj.Library;
using Syncfusion.SfBusyIndicator.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
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

                QuizCorrectAnswerColor = DefaultStyle.ChartCorrectAnswerColor;
                QuizWrongAnswerColor = DefaultStyle.ChartWrongAnswerColor;
                QuizAnswerDefaultBackColor = DefaultStyle.QuizAnswerDefaultBackColor;
                QuizAnswerSelectionBackColor = DefaultStyle.QuizAnswerSelectionBackColor;

                Answer1BackColor = DefaultStyle.QuizAnswerDefaultBackColor;
                Answer2BackColor = DefaultStyle.QuizAnswerDefaultBackColor;
                Answer3BackColor = DefaultStyle.QuizAnswerDefaultBackColor;
                Answer4BackColor = DefaultStyle.QuizAnswerDefaultBackColor;
            }
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

        public string QuizCorrectAnswerColor { get; set; }
        public string QuizWrongAnswerColor { get; set; }
        public string QuizAnswerDefaultBackColor { get; set; }
        public string QuizAnswerSelectionBackColor { get; set; }

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

        #region answer interaction

        public void AnswersSetDefaultColor()
        {
            Answer1BackColor = QuizAnswerDefaultBackColor;
            Answer2BackColor = QuizAnswerDefaultBackColor;
            Answer3BackColor = QuizAnswerDefaultBackColor;
            Answer4BackColor = QuizAnswerDefaultBackColor;
        }

        public async void OnAnswerClick(int answerIndex)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        ViewHelper.RunOnAppDispatcher(() =>
                        {
                            // Reset all labels to LightGray
                            AnswersSetDefaultColor();

                            // Apply DarkGray to the selected answer
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
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        await ViewHelper.RunOnAppDispatcherAsync(() => { busyIndicator.IsBusy = true; });

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

                            AnswersSetDefaultColor();
                        }

                        SaveCurrentResponse();

                        if (CurrentIndex == Questions.Count)
                        {
                            CalculateSummary();

                            ShowSummaryPopup();
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
                        await ViewHelper.RunOnAppDispatcherAsync(() => { busyIndicator.IsBusy = false; });
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
        }
        #endregion
    }
}
