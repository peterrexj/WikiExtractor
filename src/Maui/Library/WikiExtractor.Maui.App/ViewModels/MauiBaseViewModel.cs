using WikiExtractor.Maui.App.Services;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class MauiBaseViewModel : BaseViewModel
    {
        public override bool AdsEnabled => SharedServiceCore.AdsConfig.AdsEnabled;
    }
}
