using GeneralInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikiExtractor.XamarinForms.Exts;
using WikiExtractor.XamarinForms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WikiExtractor.XamarinForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuizPage : ContentPage
    {
        private readonly QuizPageViewModel _viewModel;

        public QuizPage()
        {
            InitializeComponent();

            _viewModel = new QuizPageViewModel(SummaryPopup);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                _viewModel.Initialize();
                ViewHelper.RunOnAppDispatcher(InitializeAdsControls);
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                _viewModel.IsBusy = false;
            }
        }

        private void AnswersSetDefaultColor()
        {
            lblAnswer1Option.BackgroundColor = _viewModel.QuizAnswerDefaultBackColor;
            lblAnswer2Option.BackgroundColor = _viewModel.QuizAnswerDefaultBackColor;
            lblAnswer3Option.BackgroundColor = _viewModel.QuizAnswerDefaultBackColor;
            lblAnswer4Option.BackgroundColor = _viewModel.QuizAnswerDefaultBackColor;
        }

        private async void AnswerClick(int answerIndex)
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
                                    lblAnswer1Option.BackgroundColor = _viewModel.QuizAnswerSelectionBackColor;
                                    _viewModel.CurrentQuestion.UserSelection = _viewModel.Answer1;
                                    break;
                                case 2:
                                    lblAnswer2Option.BackgroundColor = _viewModel.QuizAnswerSelectionBackColor;
                                    _viewModel.CurrentQuestion.UserSelection = _viewModel.Answer2;
                                    break;
                                case 3:
                                    lblAnswer3Option.BackgroundColor = _viewModel.QuizAnswerSelectionBackColor;
                                    _viewModel.CurrentQuestion.UserSelection = _viewModel.Answer3;
                                    break;
                                case 4:
                                    lblAnswer4Option.BackgroundColor = _viewModel.QuizAnswerSelectionBackColor;
                                    _viewModel.CurrentQuestion.UserSelection = _viewModel.Answer4;
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

        private async void answer1EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            AnswerClick(1);
        }

        private async void answer2EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            AnswerClick(2);
        }

        private async void answer3EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            AnswerClick(3);
        }

        private async void answer4EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            AnswerClick(4);
        }

        private async void lblNext_OnAnimationCompleted(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        await ViewHelper.RunOnAppDispatcherAsync(() => { busyIndicator.IsBusy = true; });

                        // Mapping answer options to their corresponding labels
                        var answerLabels = new Dictionary<object, Label>
                        {
                            { _viewModel.Answer1, lblAnswer1Option },
                            { _viewModel.Answer2, lblAnswer2Option },
                            { _viewModel.Answer3, lblAnswer3Option },
                            { _viewModel.Answer4, lblAnswer4Option }
                        };

                        // Apply color to the selected answer
                        var selectedAnswer = _viewModel.CurrentQuestion.UserSelection;
                        if (selectedAnswer != null)
                        {
                            var isCorrect = _viewModel.CurrentQuestion.IsCorrect;
                            ApplyAnswerColor(answerLabels[selectedAnswer], isCorrect);

                            // Highlight the correct answer if the selected answer is incorrect
                            if (!isCorrect)
                            {
                                var correctAnswer = _viewModel.CurrentQuestion.CorrectAnswer;
                                ApplyAnswerColor(answerLabels[correctAnswer], true);
                            }

                            await Task.Delay(1000);

                            AnswersSetDefaultColor();
                        }

                        _viewModel.SaveCurrentResponse();

                        if (_viewModel.CurrentIndex == _viewModel.Questions.Count)
                        {
                            _viewModel.CalculateSummary();

                            _viewModel.ShowSummaryPopup();
                        }
                        else if (_viewModel.Questions.Count > _viewModel.CurrentIndex)
                        {
                            _viewModel.CurrentIndex += 1;
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

        private void ApplyAnswerColor(Label lbl, bool isCorrect)
        {
            lbl.BackgroundColor = isCorrect ? _viewModel.QuizCorrectAnswerColor : _viewModel.QuizWrongAnswerColor;
        }

        #region Ads
        private void InitializeAdsControls()
        {
            try
            {
                if (AdsHelper.IsAdsServiceAvailable)
                {
                    if (StackBannerAds.Children.Count == 0)
                    {
                        var adsBanner = AdsHelper.BuildAdsBanner();
                        if (adsBanner != null)
                        {
                            StackBannerAds.Children.Add(adsBanner);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        #endregion
    }
}