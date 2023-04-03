using GeneralInformation.Exts;
using GeneralInformation.Repository;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using MarcTron.Plugin;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static string Tag { get; set; }
        public List<string> Tags => Tag.HasValue() ? Tag.SplitAndTrim(",").ToList() : new List<string>();

        private readonly PersonaListViewModel personaListViewModel;
        private int _masterId;

        public WikiListOfItemsPage()
        {
            InitializeComponent();
            try
            {
                personaListViewModel = new PersonaListViewModel();

                TaskGroup taskGroup = new();
                taskGroup.Add(() => personaListViewModel.Personas = SharedServices.WikiAppController.GetListOfWikiItems(Tags, StylePropertyHelper.GetStyleOnListItemHeightRequestOnListPage()).ToList());
                taskGroup.Add(() => personaListViewModel.Title = SharedServices.WikiAppController.AppMenuItems().FirstOrDefault(f => f.Tags == string.Join(",", Tags)).TitleOnThePage ?? string.Empty);
                taskGroup.Add(() => RunOnAppDispatcher(InitializeAdsService));
                taskGroup.WaitAll();

                personaListViewModel.AutocompleteList = personaListViewModel.Personas.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name });

                BindingContext = personaListViewModel;
                ThemeHelper.SetTheme();
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

        private void InitializeAdsService()
        {
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
            catch (OperationCanceledException)
            {
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return true;
            }
        }

        private async void lstItemEffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (sender is SfEffectsView && (sender as SfEffectsView).AutomationId.HasValue())
                    {
                        _masterId = (sender as SfEffectsView).AutomationId.ToInteger();
                        await ProcessRequestToSubPage();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void LoadInterstitialAds()
        {
            try
            {
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
        }

        private async Task ProcessRequestToSubPage()
        {
            PersonaViewModel personaObj = null;

            await Task.Run(() =>
            {
                try
                {
                    TaskGroup taskGroup = new();

                    taskGroup.Add(() =>
                    {
                        personaListViewModel.IsBusy = true;
                        personaObj = personaListViewModel.Personas.FirstOrDefault(f => f.Id == _masterId);
                        if (personaObj != null)
                        {
                            personaObj.IsBusy = true;
                        }
                    });
                    taskGroup.Add(() => LoadInterstitialAds());
                    taskGroup.Add(() => PersonaDetailPage.LoadContent(_masterId));
                    taskGroup.WaitAll();

                    taskGroup.Add(() => PersonaDetailPage.LoadParaGrids());
                    taskGroup.WaitAll();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                   
                }
            });

            var route = $"{nameof(PersonaDetailPage)}?MasterId={_masterId}";
            await Shell.Current.GoToAsync(route);

            personaListViewModel.IsBusy = false;
            if (personaObj != null)
            {
                personaObj.IsBusy = false;
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