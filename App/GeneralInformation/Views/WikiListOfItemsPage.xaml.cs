using GeneralInformation.Exts;
using GeneralInformation.Repository;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using MarcTron.Plugin;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.SfCarousel.XForms;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WikiExtractor.Exts;
using WikiExtractor.Process;
using WikiExtractor.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeneralInformation.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WikiListOfItemsPage : ContentPage
    {
        public static string Tag { get; set; }
        public List<string> Tags => Tag.HasValue() ? Tag.SplitAndTrim(",").ToList() : new List<string>();

        private PersonaListViewModel personaListViewModel;
        private readonly WikiAppController wikiAppController;

        //#region Back Press
        //public static Action EmulateBackPressed;
        //private bool AcceptBack;

        //protected override bool OnBackButtonPressed()
        //{
        //    if (AcceptBack)
        //        return false;

        //    PromptForExit();
        //    return true;
        //}

        //private async void PromptForExit()
        //{
        //    if (await DisplayAlert("", "Are you sure to exit?", "Yes", "No"))
        //    {
        //        AcceptBack = true;
        //        EmulateBackPressed();
        //    }
        //}
        //#endregion

        public WikiListOfItemsPage()
        {
            InitializeComponent();
            try
            {
                wikiAppController = new WikiAppController(DatabaseService.AppDatabase);
                var data = wikiAppController.GetListOfWikiItems(Tags, StylePropertyHelper.GetStyleOnListItemHeightRequestOnListPage()).ToList();
                var title = wikiAppController.AppMenuItems().FirstOrDefault(f => f.Tags == string.Join(",", Tags)).TitleOnThePage ?? string.Empty;

                BindingContext = personaListViewModel = new PersonaListViewModel
                {
                    Title = title,
                    Personas = data,
                    AutocompleteList = data.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name })
                };
                ThemeHelper.SetTheme();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            try
            {
                DatabaseService.AppDatabase.PhoneSettingsRepository.UpdateLimitsOnInitialize(
                    DependencyService.Get<IAppInformation>().ShowFirstInterstitialAdOnClickLimit,
                    DependencyService.Get<IAppInformation>().ShowLaterInterstitialAdOnClickLimit);
                DatabaseService.AppDatabase.PhoneSettingsRepository.InitializeGoogleAds();
                CrossMTAdmob.Current.LoadInterstitial(personaListViewModel.AdsInterstitialId);
                CrossMTAdmob.Current.OnInterstitialOpened += Current_OnInterstitialOpened;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            personaListViewModel.IsBusy = false;
        }

        public void RunOnAppDispatcher(Action action)
        {
            try
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    action();
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void Current_OnInterstitialOpened(object sender, EventArgs e)
        {
            try
            {
                DatabaseService.AppDatabase.PhoneSettingsRepository.GoogleAdsIntersitialUpdateLimit();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private void autoComplete_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                personaListViewModel.IsBusy = true;
                if (sender is SfAutoComplete)
                {
                    if (lstListOfItems.DataSource != null)
                    {
                        lstListOfItems.DataSource.Filter = FilterPersonas;
                        lstListOfItems.DataSource.RefreshFilter();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                personaListViewModel.IsBusy = false;
            }
        }
        private bool FilterPersonas(object obj)
        {
            try
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return true;
            }
        }

        //private async void lstListOfItems_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        //{
        //    if (e != null && e.AddedItems.Count > 0)
        //    {
        //        DatabaseService.AppDatabase.RequestRecordRepository.UpdateCount();

        //        if (/*ConfigData.AdsIsIntersitialDisplayed == false && */
        //               DatabaseService.AppDatabase.RequestRecordRepository.RequestOnLimit &&
        //               CrossMTAdmob.Current.IsInterstitialLoaded())
        //        {
        //            //RunOnAppDispatcher(() =>
        //            //{
        //            //    CrossMTAdmob.Current.ShowInterstitial();
        //            //    CrossMTAdmob.Current.LoadInterstitial(ConfigData.AdsIntersitialUnitId);
        //            //});
        //        }
        //        else
        //        {
        //            var masterId = (e.AddedItems.First() as PersonaViewModel).Id;
        //            var route = $"{nameof(PersonaDetailPage)}?MasterId={masterId}";
        //            await Shell.Current.GoToAsync(route);
        //        }
        //    }
        //}

        private async void lstItemEffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (sender is SfEffectsView && (sender as SfEffectsView).AutomationId.HasValue())
                    {
                        var masterId = (sender as SfEffectsView).AutomationId;
                        await NavigateToChildPage(masterId.ToInteger());
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task NavigateToChildPage(int masterId)
        {
            PersonaViewModel personaObj = null;
            try
            {
                personaListViewModel.IsBusy = true;
                personaObj = personaListViewModel.Personas.FirstOrDefault(f => f.Id == masterId);
                if (personaObj != null)
                {
                    personaObj.IsBusy = true;
                }
                var route = $"{nameof(PersonaDetailPage)}?MasterId={masterId}";
                await Shell.Current.GoToAsync(route);

                if (DatabaseService.AppDatabase.RequestRecordRepository.RequestOnLimit &&
                    CrossMTAdmob.Current.IsInterstitialLoaded())
                {
                    RunOnAppDispatcher(() =>
                    {
                        CrossMTAdmob.Current.ShowInterstitial();
                        CrossMTAdmob.Current.LoadInterstitial(personaListViewModel.AdsInterstitialId);
                    });
                }
                DatabaseService.AppDatabase.RequestRecordRepository.UpdateCount();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                personaListViewModel.IsBusy = false;
                if (personaObj != null)
                {
                    personaObj.IsBusy = false;
                }
            }
        }

        private void autoComplete_Completed(object sender, EventArgs e)
        {
            try
            {
                if (sender is SfAutoComplete)
                {
                    if (lstListOfItems.DataSource != null)
                    {
                        lstListOfItems.DataSource.Filter = FilterPersonas;
                        lstListOfItems.DataSource.RefreshFilter();
                        autoComplete.IsDropDownOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void autoComplete_ValueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            try
            {
                if (autoComplete.Text.IsEmpty())
                {
                    if (lstListOfItems.DataSource != null)
                    {
                        lstListOfItems.DataSource.Filter = FilterPersonas;
                        lstListOfItems.DataSource.RefreshFilter();
                        autoComplete.IsDropDownOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}