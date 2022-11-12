using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
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
    [QueryProperty(nameof(MasterId), nameof(MasterId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonaDetailPage : ContentPage
    {
        public string MasterId { get; set; }
        private readonly WikiAppController wikiAppController;
        private PersonaDetailViewModel personaDetailViewModel;

        public PersonaDetailPage()
        {
            InitializeComponent();
            wikiAppController = new WikiAppController(DatabaseService.AppDatabase);
            personaDetailViewModel = new PersonaDetailViewModel();
            BindingContext = personaDetailViewModel = new PersonaDetailViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(MasterId, out var result);
            personaDetailViewModel.Persona = wikiAppController.GetViewModelById(result);
            if (personaDetailViewModel.ArePicturesAvailable)
            {
                personaDetailViewModel.CurrentSelectedPictureCaption = personaDetailViewModel.Persona.Pictures.FirstOrDefault().PictureCaption;
            }
        }

        private void carousel_SelectionChanged(object sender, Syncfusion.SfCarousel.XForms.SelectionChangedEventArgs e)
        {
            if (e != null && e.SelectedItem != null)
            {
                personaDetailViewModel.CurrentSelectedPictureCaption = (e.SelectedItem as PictureViewModel).PictureCaption;
            }
        }
    }
}