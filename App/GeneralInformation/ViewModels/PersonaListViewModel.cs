using GeneralInformation.Exts;
using GeneralInformation.Services;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
            var appInfo = DependencyService.Get<IAppInformation>();
            AdsInterstitialId = appInfo.AdsInterstitialId;
            AdsBannerId = appInfo.AdsBannerId;
            StyleDrive = new StyleDrive
            {
                StyleOnImageHeightRequestOnListPage = appInfo.StyleOnImageHeightRequestOnListPage,
                //StyleOnListItemHeightRequestOnListPage = StylePropertyHelper.GetStyleOnListItemHeightRequestOnListPage()
            };
        }

        public string Title { get; set; }

        private IList<PersonaViewModel> _personas;
        public IList<PersonaViewModel> Personas { get => _personas; set => SetProperty(ref _personas, value); }

        public IEnumerable<PersonaAutoCompleteModel> AutocompleteList { get; set; }
        public ICommand TapHyperLinkToWikiPage => new Command<string>(async (url) => await Launcher.OpenAsync($"https://en.wikipedia.org/{url}"));
        public StyleDrive StyleDrive { get; set; }

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

    public class StyleDrive
    {
        public int StyleOnImageHeightRequestOnListPage { get; set;}
        public int StyleOnListItemHeightRequestOnListPage { get; set; }
    }
}
