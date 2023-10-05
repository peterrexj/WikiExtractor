using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SampleAutoComplete.Views
{
    public partial class AboutPage : ContentPage
    {
        private readonly AboutPageViewModel aboutPageViewModel;
        public AboutPage()
        {
            InitializeComponent();

            aboutPageViewModel = new AboutPageViewModel
            {
                AutocompleteList = new List<PersonaAutoCompleteModel>
                {
                    { new PersonaAutoCompleteModel { Id = 1, Name = "Xamarin" } },
                    { new PersonaAutoCompleteModel { Id = 1, Name = "Uwp" } },
                    { new PersonaAutoCompleteModel { Id = 1, Name = "iOS" } },
                    { new PersonaAutoCompleteModel { Id = 1, Name = "Android" } }
                }
            };

            this.BindingContext = aboutPageViewModel;
        }

        private void autoComplete_Completed(object sender, EventArgs e)
        {
            
        }

        private void autoComplete_ValueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            if (e.Value != null)
            {
                aboutPageViewModel.SelectedValue = e.Value;
            }
        }

        private void autoComplete_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            var casted = e.Value as PersonaAutoCompleteModel;
            if ( casted != null)
            {
                aboutPageViewModel.SelectedName = casted.Name;
            }
        }
    }

    public class AboutPageViewModel : INotifyPropertyChanged
    {
        public IEnumerable<PersonaAutoCompleteModel> AutocompleteList { get; set; }

        private string _selectedValue;
        public string SelectedValue
        {
            get
            {
                return _selectedValue;
            }
            set
            {
                _selectedValue = value;
                OnPropertyChanged("SelectedValue");
            }
        }

        private string _selectedName;
        public string SelectedName
        {
            get
            {
                return _selectedName;
            }
            set
            {
                _selectedName = value;
                OnPropertyChanged("SelectedName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class PersonaAutoCompleteModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
