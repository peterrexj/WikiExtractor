using System.ComponentModel;
using Xamarin.Forms;
using SampleAutoComplete.ViewModels;

namespace SampleAutoComplete.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
