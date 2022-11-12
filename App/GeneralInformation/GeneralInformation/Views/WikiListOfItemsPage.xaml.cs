using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
using Pj.Library;
using Syncfusion.SfAutoComplete.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process;
using WikiExtractor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeneralInformation.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WikiListOfItemsPage : ContentPage
    {
        private readonly PersonaListViewModel personaListViewModel;
        private readonly WikiAppController wikiAppController;

        public WikiListOfItemsPage()
        {
            InitializeComponent();
            wikiAppController = new WikiAppController(DatabaseService.AppDatabase);
            var data = wikiAppController.GetListOfWikiItems().Where(f => f.Name.StartsWith("Alb"));

            BindingContext = personaListViewModel = new PersonaListViewModel
            {
                Personas = data,
                AutocompleteList = data.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name })
            };

            btnTabDefinitionsHeader.Text = DatabaseService.AppDatabase.MasterRepository.GetAll()?.FirstOrDefault()?.Name;
        }

        private void autoComplete_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            if (sender is SfAutoComplete)
            {
                if (lstSaints.DataSource != null)
                {
                    lstSaints.DataSource.Filter = FilterPersonas;
                    lstSaints.DataSource.RefreshFilter();
                }
            }
        }

        //private string _filterText => 

        private bool FilterPersonas(object obj)
        {
            var filterText = string.Empty;
            if (autoComplete.SelectedItem != null)
            {
                filterText = (autoComplete.SelectedItem as PersonaAutoCompleteModel).Name;
            }
            else
            {

            }

            var persona = obj as PersonaViewModel;
            return persona.Name.ContainsIgnoreCase(filterText);
        }

        private async void lstSaints_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            if (e != null && e.AddedItems.Count > 0)
            {
                var masterId = (e.AddedItems.First() as PersonaViewModel).Id;
                var route = $"{nameof(PersonaDetailPage)}?MasterId={masterId}";
                await Shell.Current.GoToAsync(route); 
            }
        }
    }
}