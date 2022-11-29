using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.ViewModels;

namespace GeneralInformation.ViewModels
{
    public class PersonaListViewModel : BaseViewModel
    {

        public IEnumerable<PersonaViewModel> Personas { get; set; }
        public IEnumerable<PersonaAutoCompleteModel> AutocompleteList { get; set; }

    }
}
