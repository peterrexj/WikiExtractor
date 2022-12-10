using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WikiExtractor.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation.ViewModels
{
    public class PersonaListViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public IEnumerable<PersonaViewModel> Personas { get; set; }
        public IEnumerable<PersonaAutoCompleteModel> AutocompleteList { get; set; }
        public ICommand TapHyperLinkToWikiPage => new Command<string>(async (url) => await Launcher.OpenAsync($"https://en.wikipedia.org/{url}"));
    }
}
