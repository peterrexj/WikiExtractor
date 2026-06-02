using Pj.Library;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;

namespace WikiExtractor.Maui.App.Views
{
    public partial class StatsPage : ContentPage
    {
        private StatsPageViewModel _vm;

        public StatsPage(StatsPageViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await _vm.InitializeAsync();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
    }
}
