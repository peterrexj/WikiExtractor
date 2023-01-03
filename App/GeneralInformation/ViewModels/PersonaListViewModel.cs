using GeneralInformation.Services;
using System.Collections.Generic;
using System.Windows.Input;
using WikiExtractor.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation.ViewModels
{
    public class PersonaListViewModel : BaseViewModel
    {
        public PersonaListViewModel()
        {
            AdsInterstitialId = DependencyService.Get<IAppInformation>().AdsInterstitialId;
            AdsBannerId = DependencyService.Get<IAppInformation>().AdsBannerId;
        }

        public string Title { get; set; }

        private IList<PersonaViewModel> _personas;
        public IList<PersonaViewModel> Personas { get => _personas; set => SetProperty(ref _personas, value); }

        public IEnumerable<PersonaAutoCompleteModel> AutocompleteList { get; set; }
        public ICommand TapHyperLinkToWikiPage => new Command<string>(async (url) => await Launcher.OpenAsync($"https://en.wikipedia.org/{url}"));


        #region Ads
        private string _adsBannerId;
        public string AdsBannerId
        {
            get
            {
                return _adsBannerId;
            }
            set
            {
                _adsBannerId = value;
                OnPropertyChanged("AdsBannerId");
            }
        }

        private string _adsInterstitialId;
        public string AdsInterstitialId
        {
            get
            {
                return _adsInterstitialId;
            }
            set
            {
                _adsInterstitialId = value;
                OnPropertyChanged("AdsInterstitialId");
            }
        }
        #endregion
    }
}
