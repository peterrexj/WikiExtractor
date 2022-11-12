using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeneralInformation.ViewModels
{
    public class AboutViewModel : BaseViewModel_NotUsed
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        public ICommand OpenWebCommand { get; }
    }
}