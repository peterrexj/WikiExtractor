using GeneralInformation.Exts;
using GeneralInformation.Models.Mix;
using GeneralInformation.Repository;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using MarcTron.Plugin;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.Buttons;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private PersonaListViewModel personaListViewModel;
        private int _masterId;

        public WikiListOfItemsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            try
            {
                if (BindingContext == null || personaListViewModel == null)
                {

                    personaListViewModel = new PersonaListViewModel();

                    TaskGroup taskGroup = new();
                    taskGroup.Add(() => personaListViewModel.Personas = SharedServices.WikiAppController.GetListOfWikiItems(Tags, StylePropertyHelper.GetStyleOnListItemHeightRequestOnListPage()).ToList());
                    taskGroup.Add(() => personaListViewModel.Title = SharedServices.WikiAppController.AppMenuItems().FirstOrDefault(f => f.Tags == string.Join(",", Tags)).TitleOnThePage ?? string.Empty);
                    taskGroup.Add(() => personaListViewModel.HideItemRead = SettingsHelper.ShouldShowAlreadyReadItem());
                    taskGroup.Add(() => personaListViewModel.SortBySelectedIndex = Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)), SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));
                    taskGroup.Add(() => RunOnAppDispatcher(InitializeAdsService));
                    taskGroup.WaitAll();

                    personaListViewModel.AutocompleteList = personaListViewModel.Personas.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name });

                    BindingContext = personaListViewModel;

                }

                if (SharedServices.PageDataTransferModel.Name.HasValue())
                {
                    foreach (var item in personaListViewModel.Personas.Where(f => f.Name == SharedServices.PageDataTransferModel.Name))
                    {
                        item.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;
                    }
                    SharedServices.PageDataTransferModel.Clear();
                }
                
                Task.Run(RefreshListOfListFilter);
                personaListViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();
                ThemeHelper.UpdateAppThemes(personaListViewModel.DefaultStyle);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                personaListViewModel.IsBusy = false;
            }
            base.OnAppearing();
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
                        SharedServices.PageDataTransferModel.Clear();
                        SharedServices.PageDataTransferModel.Id = _masterId;
                        SharedServices.PageDataTransferModel.Name = personaObj.Name;
                        SharedServices.PageDataTransferModel.IsMarkedAsViewed = personaObj.ItemReadStatus;
                    });

#if DEBUG == false
                    if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                    {
                        taskGroup.Add(() => LoadInterstitialAds());
                    }
#endif

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

        #region Ads
        private void InitializeAdsService()
        {
            try
            {
                if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                {
                    DatabaseService.UserStoreDatabase.RequestRecordRepository.RefreshCountData();
                    DatabaseService.UserStoreDatabase.AppSettingsRepository.UpdateLimitsOnInitialize(
                        DependencyService.Get<IAppInformation>().ShowFirstInterstitialAdOnClickLimit,
                        DependencyService.Get<IAppInformation>().ShowLaterInterstitialAdOnClickLimit);
                    DatabaseService.UserStoreDatabase.AppSettingsRepository.InitializeGoogleAds();
                    CrossMTAdmob.Current.LoadInterstitial(personaListViewModel.AdsInterstitialId);
                    CrossMTAdmob.Current.OnInterstitialOpened += Current_OnInterstitialOpened;
                }
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
                DatabaseService.UserStoreDatabase.AppSettingsRepository.GoogleAdsIntersitialUpdateLimit();
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
        private void LoadInterstitialAds()
        {
            try
            {
                if (DatabaseService.UserStoreDatabase.RequestRecordRepository.RequestOnLimit &&
                    CrossMTAdmob.Current.IsInterstitialLoaded())
                {
                    RunOnAppDispatcher(() =>
                    {
                        CrossMTAdmob.Current.ShowInterstitial();
                        CrossMTAdmob.Current.LoadInterstitial(personaListViewModel.AdsInterstitialId);
                    });
                }
                DatabaseService.UserStoreDatabase.RequestRecordRepository.UpdateCount();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion

        #region Filter
        private async Task RefreshListOfListFilter()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (lstListOfItems.DataSource != null)
                    {
                        lstListOfItems.DataSource.Filter = FilterPersonas;
                        lstListOfItems.DataSource.RefreshFilter();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }

        private bool FilterPersonas(object obj)
        {
            try
            {
                var persona = obj as PersonaViewModel;
                var filterText = string.Empty;
                if (autoComplete.SelectedItem != null)
                {
                    filterText = (autoComplete.SelectedItem as PersonaAutoCompleteModel).Name;
                }
                else if (autoComplete.Text.HasValue())
                {
                    filterText = autoComplete.Text;
                }

                if (personaListViewModel.HideItemRead && persona.ItemReadStatus)
                {
                    return false;
                }
                if (autoComplete.Text.IsEmpty())
                {
                    return true;
                }

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

        #endregion

        #region Sort
        private async void sfSegmentSortOrder_SelectionChanged(object sender, Syncfusion.XForms.Buttons.SelectionChangedEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var sortAttrib = (MainListSortDescriptorModel.SortByAttribute)Enum.ToObject(typeof(MainListSortDescriptorModel.SortByAttribute), e.Index);
                    var sortInfo = SettingsHelper.GetSortDescriptorBySelectedItem(sortAttrib);
                    if (sortInfo != null && sortInfo.PropertyName == "RandomId")
                    {
                        var count = personaListViewModel.Personas.Count;
                        Parallel.ForEach(personaListViewModel.Personas, new ParallelOptions { MaxDegreeOfParallelism = 5 }, item =>
                        {
                            item.RandomId = RandomHelper.RandomNumberGeneratorBetweenRange(0, count);
                        });
                    }
                    await SortList(sortInfo);
                    SettingsHelper.SaveSortDescriptor(sortInfo.PropertyName, sortInfo.Direction.ToString());
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }

        private async Task SortList(MainListSortDescriptorModel sortInfo)
        {
            await Task.Run(() =>
            {
                try
                {
                    lstListOfItems.DataSource.SortDescriptors.Clear();
                    lstListOfItems.DataSource.SortDescriptors.Add(new Syncfusion.DataSource.SortDescriptor { PropertyName = sortInfo.PropertyName, Direction = sortInfo.Direction });
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }

        #endregion

        #region Auto Complete
        private async void autoComplete_Completed(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (sender is SfAutoComplete)
                    {
                        await RefreshListOfListFilter();
                        autoComplete.IsDropDownOpen = false;
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }

        private async void autoComplete_ValueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (autoComplete.Text.IsEmpty())
                    {
                        await RefreshListOfListFilter();
                        autoComplete.IsDropDownOpen = false;
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }

        private async void autoComplete_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            await Task.Run(() =>
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
            });
        }

        #endregion

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
        private async void btnThemePick_Clicked(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    RunOnAppDispatcher(() =>
                    {
                        if (SettingsHelper.SelectedTheme == AppThemes.Light)
                        {
                            SettingsHelper.SaveTheme(AppThemes.Dark);
                        }
                        else
                        {
                            SettingsHelper.SaveTheme(AppThemes.Light);
                        }
                        personaListViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();
                        ThemeHelper.UpdateAppThemes(personaListViewModel.DefaultStyle);
                    });
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }
        //private async void swtReadItem_StateChanged(object sender, Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs e)
        //{
        //    await Task.Run(async () =>
        //    {
        //        try
        //        {
        //            if (sender != null)
        //            {
        //                if (sender is SfSwitch && (sender as SfSwitch).AutomationId.HasValue())
        //                {
        //                    var name = (sender as SfSwitch).AutomationId;
        //                    SharedServices.WikiAppController.UpdateItemRead(name, e.NewValue.HasValue ? e.NewValue.Value : false);

        //                    foreach (var item in personaListViewModel.Personas.Where(f => f.Name == name))
        //                    {
        //                        item.ItemReadStatus = e.NewValue.Value;
        //                    }
        //                    await RefreshListOfListFilter();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Crashes.TrackError(ex);
        //        }
        //    });
        //}
        private async void swtToggleItemRead_StateChanged(object sender, SwitchStateChangedEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    SettingsHelper.SaveShouldShowAlreadyReadItems(e.NewValue.Value);
                    if (e.OldValue.Value != e.NewValue.Value)
                    {
                        await RefreshListOfListFilter();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }
    }
}