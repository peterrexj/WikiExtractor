using Microsoft.Maui.Controls;
using Syncfusion.Maui.Core;
using System;
using System.Threading.Tasks;
using WikiExtractor.Maui.App.ViewModels;

namespace WikiExtractor.Maui.App.Views
{
    public partial class QuizPage : ContentPage
    {
        private QuizPageViewModel _viewModel;

        public QuizPage(QuizPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _viewModel = BindingContext as QuizPageViewModel;
            
            if (_viewModel != null)
            {
                await _viewModel.InitializeAsync();
            }
        }

        private async void answer1EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            await HandleAnswerSelection(sender, 1);
        }

        private async void answer2EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            await HandleAnswerSelection(sender, 2);
        }

        private async void answer3EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            await HandleAnswerSelection(sender, 3);
        }

        private async void answer4EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            await HandleAnswerSelection(sender, 4);
        }

        private async Task HandleAnswerSelection(object sender, int answerNumber)
        {
            if (_viewModel == null) return;

            // Get the SfEffectsView that triggered the event
            var effectsView = sender as SfEffectsView;
            if (effectsView == null) return;

            // Show loading indicator
            busyIndicator.IsRunning = true;

            try
            {
                // Process the answer selection
                await _viewModel.OnAnswerClick(answerNumber);
            }
            catch (Exception ex)
            {
                // Handle any errors during answer processing
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                busyIndicator.IsRunning = false;
            }
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (_viewModel == null) return;

            // Show loading indicator
            //busyIndicator.IsRunning = true;

            try
            {
                if (_viewModel.NextQuestionCommand?.CanExecute(null) == true)
                {
                    _viewModel.NextQuestionCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                //busyIndicator.IsRunning = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Clean up any resources if needed
            if (_viewModel != null)
            {
                _viewModel.CleanupResources();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Handle back button press for quiz navigation
            if (_viewModel != null && _viewModel.CanGoBack)
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    var result = await DisplayAlert("Exit Quiz", "Are you sure you want to exit the quiz? Your progress will be lost.", "Yes", "No");
                    if (result)
                    {
                        if (_viewModel.ExitQuizCommand?.CanExecute(null) == true)
                        {
                            _viewModel.ExitQuizCommand.Execute(null);
                        }
                    }
                });
                return true; // Prevent default back behavior
            }

            return base.OnBackButtonPressed();
        }
    }
}