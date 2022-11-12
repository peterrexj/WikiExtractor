using GeneralInformation.ViewModels;
using GeneralInformation.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GeneralInformation
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(WikiListOfItemsPage), typeof(WikiListOfItemsPage));
            Routing.RegisterRoute(nameof(PersonaDetailPage), typeof(PersonaDetailPage));
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SaintsPage");
        }
    }
}
