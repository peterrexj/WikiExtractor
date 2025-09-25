using Maui.Samples.Models;
using Maui.Samples.PageModels;

namespace Maui.Samples.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}