using WikiExtractor.Maui.App.ViewModels;

namespace WikiExtractor.Maui.App.Views;

public partial class QuizResultsPage : ContentPage
{
    public QuizResultsPage(QuizResultsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}