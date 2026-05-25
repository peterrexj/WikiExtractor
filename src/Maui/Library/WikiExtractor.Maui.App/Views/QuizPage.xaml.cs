using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;

namespace WikiExtractor.Maui.App.Views
{
    public partial class QuizPage : ContentPage
    {
        private QuizPageViewModel _viewModel;
        private bool _isInitialized = false;

        public QuizPage(QuizPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Prevent re-loading if they navigate back to this page
                if (_isInitialized) return;

                if (_viewModel != null)
                {
                    _viewModel.IsPageBusy = true;

                    // Give the UI a tiny moment to finish the "Slide" animation of the page
                    await Task.Delay(100);

                    await _viewModel.InitializeAsync();
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                if (_viewModel != null) _viewModel.IsPageBusy = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                _viewModel?.CleanupResources();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Handle back button press for quiz navigation
            if (_viewModel != null && _viewModel.CanGoBack)
            {
                ViewHelper.RunOnAppDispatcherAsync(async () =>
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