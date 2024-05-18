using System;
using System.Collections.Generic;
using SampleAutoComplete.ViewModels;
using SampleAutoComplete.Views;
using Xamarin.Forms;

namespace SampleAutoComplete
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}

