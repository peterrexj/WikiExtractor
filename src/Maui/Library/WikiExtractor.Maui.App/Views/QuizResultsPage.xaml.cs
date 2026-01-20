using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;

namespace WikiExtractor.Maui.App.Views;

public partial class QuizResultsPage : ContentPage
{
    public QuizResultsPage(QuizResultsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.IsPageBusy = true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is QuizResultsPageViewModel vm)
        {
            await vm.LoadChartDataAsync();
            vm.BannerAdsUnitId = SharedServiceCore.AdsConfig?.QuizBannerAdUnitId ?? SharedServiceCore.AdsConfig?.BannerAdUnitId;
        }
    }
}