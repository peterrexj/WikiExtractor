using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
using Pj.Library;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.EffectsView;
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
            var data = wikiAppController.GetListOfWikiItems(); //.Where(f => f.Name.StartsWith("Alb"));

            BindingContext = personaListViewModel = new PersonaListViewModel
            {
                Personas = data,
                AutocompleteList = data.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name })
            };
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
            else if (autoComplete.Text.HasValue())
            {
                filterText = autoComplete.Text;
            }
            else if (autoComplete.Text.IsEmpty())
            {
                return true;
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

        private async void lstItemEffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            if (sender != null)
            {
                if (sender is SfEffectsView && (sender as SfEffectsView).AutomationId.HasValue())
                {
                    var masterId = (sender as SfEffectsView).AutomationId;
                    var route = $"{nameof(PersonaDetailPage)}?MasterId={masterId}";
                    await Shell.Current.GoToAsync(route);
                }
            }
        }

        private void autoComplete_Completed(object sender, EventArgs e)
        {
            if (sender is SfAutoComplete)
            {
                if (lstSaints.DataSource != null)
                {
                    lstSaints.DataSource.Filter = FilterPersonas;
                    lstSaints.DataSource.RefreshFilter();
                    autoComplete.IsDropDownOpen = false;
                }
            }
        }
    }
}