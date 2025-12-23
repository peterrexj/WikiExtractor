using Microsoft.Maui.Controls;
using Pj.Library;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.Buttons;
using Syncfusion.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Maui.DataSource;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Models;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;
using WikiExtractor.ViewModels;
using PjAds.Maui.Services;
using PjAds.Maui.Models;

namespace WikiExtractor.Maui.App.Views
{
    public partial class WikiListOfItemsPage : ContentPage
    {
        public static string Tag { get; set; }
        public List<string> Tags => Tag.HasValue() ? Tag.SplitAndTrim(",").ToList() : new List<string>();

        private PersonaListViewModel personaListViewModel;
        private int _masterId;
        private readonly IAdManager _adManager;

        public WikiListOfItemsPage()
        {
            InitializeComponent();
            
            try
            {
                // Get the ad manager from dependency injection with fallback
                _adManager = GetAdManagerService();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Failed to initialize AdManager in WikiListOfItemsPage: {ex.Message}");
                _adManager = null; // The code already handles null _adManager gracefully
            }
        }

        private IAdManager GetAdManagerService()
        {
            try
            {
                return ServiceLocator.GetService<IAdManager>();
            }
            catch
            {
                return null; // The existing code already handles null _adManager
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                if (BindingContext == null || personaListViewModel == null)
                {
                    personaListViewModel = new PersonaListViewModel();
                    
                    // Set initial loading state and bind context early for loading indicator visibility
                    personaListViewModel.IsDataLoading = true;
                    personaListViewModel.LoadingMessage = "Initializing list...";
                    BindingContext = personaListViewModel;

                    await LoadInitialData();
                }
                else
                {
                    // Set loading state for refresh operations
                    personaListViewModel.IsDataLoading = true;
                    personaListViewModel.LoadingMessage = "Refreshing data...";

                    await LoadRefreshData();
                }

                //personaListViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();
                //ThemeHelper.UpdateAppThemes(personaListViewModel.DefaultStyle);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                if (personaListViewModel != null)
                {
                    personaListViewModel.IsDataLoading = false;
                }
            }
            finally
            {
                if (personaListViewModel != null)
                {
                    personaListViewModel.IsBusy = false;
                }
            }
            base.OnAppearing();
        }

        private async Task LoadInitialData()
        {
            try
            {
                personaListViewModel.LoadingMessage = "Loading persona data...";
                await Task.Delay(200);

                TaskGroup taskGroup = new();
                taskGroup.Add(() => personaListViewModel.Personas = SharedServices.WikiAppController.GetListOfWikiItems(Tags).ToList());
                taskGroup.Add(() => personaListViewModel.Title = SharedServices.WikiAppController.AppMenuItems().FirstOrDefault(f => f.Tags == string.Join(",", Tags)).TitleOnThePage ?? string.Empty);
                taskGroup.Add(() => personaListViewModel.HideItemRead = SettingsHelper.ShouldShowAlreadyReadItem());
                taskGroup.Add(() => personaListViewModel.SortBySelectedIndex = Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)), SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));
                taskGroup.WaitAll();

                personaListViewModel.LoadingMessage = "Building autocomplete list...";
                await Task.Delay(200);

                personaListViewModel.AutocompleteList = personaListViewModel.Personas.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name }).ToList();

                personaListViewModel.LoadingMessage = "Applying filters...";
                await Task.Delay(200);

                await Task.Run(RefreshListOfListFilter);

                personaListViewModel.LoadingMessage = "Finalizing...";
                await Task.Delay(100);

                // Complete loading
                personaListViewModel.IsDataLoading = false;
            }
            catch (Exception ex)
            {
                personaListViewModel.IsDataLoading = false;
                throw;
            }
        }

        private async Task LoadRefreshData()
        {
            try
            {
                bool hasReadStatusChanged = false;
                bool hasItemReadToggled = false;

                personaListViewModel.LoadingMessage = "Checking read status...";
                await Task.Delay(200);

                TaskGroup taskGroup = new();
                taskGroup.Add(() =>
                {
                    //This section reach on two occasions
                    //1. When redirected back from the subpage to the main page
                    //2. Navigate to another page from the left menu and then come back to the same page

                    //Reading all the Item Read status and apply for the loaded items on this page
                    var mapData = (from data in personaListViewModel.Personas
                                   join tagItemJoin in SharedServices.WikiAppController.GetItemReadTrackData() on data.Name equals tagItemJoin.ItemIdentifier into tagItemGrp
                                   from tagItem in tagItemGrp
                                   select new
                                   {
                                       Data = data,
                                       Status = tagItem.IsReadAsBool
                                   }).ToList();

                    foreach (var data in mapData)
                    {
                        data.Data.ItemReadStatus = data.Status;
                    }

                    //This section handles the Item Read from the sub page to main page. Stores the information in a shared place and utilize that see any changes need to apply
                    if (SharedServices.PageDataTransferModel.Name.HasValue())
                    {
                        foreach (var item in personaListViewModel.Personas.Where(f => f.Name == SharedServices.PageDataTransferModel.Name))
                        {
                            hasReadStatusChanged = item.ItemReadStatus != SharedServices.PageDataTransferModel.IsMarkedAsViewed;
                            item.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;
                        }
                        SharedServices.PageDataTransferModel.Clear();
                    }
                    personaListViewModel.SearchItemName = "";
                });
                taskGroup.Add(() =>
                {
                    var hideItemReadStatusFromStore = SettingsHelper.ShouldShowAlreadyReadItem();
                    hasItemReadToggled = hideItemReadStatusFromStore != personaListViewModel.HideItemRead;
                    personaListViewModel.HideItemRead = hideItemReadStatusFromStore;
                });
                taskGroup.Add(() => personaListViewModel.SortBySelectedIndex = Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)), SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));
                taskGroup.WaitAll();

                if (hasReadStatusChanged || hasItemReadToggled)
                {
                    personaListViewModel.LoadingMessage = "Updating filters...";
                    await Task.Delay(200);
                    
                    await Task.Run(RefreshListOfListFilter);
                }

                personaListViewModel.LoadingMessage = "Finalizing...";
                await Task.Delay(100);

                // Complete loading
                personaListViewModel.IsDataLoading = false;
            }
            catch (Exception ex)
            {
                personaListViewModel.IsDataLoading = false;
                throw;
            }
        }

        private async Task ProcessRequestToSubPage()
        {
            PersonaViewModel personaObj = null;

            try
            {
                // Start navigation loading state
                personaListViewModel.IsNavigating = true;
                personaListViewModel.NavigationMessage = "Preparing navigation...";
                await Task.Delay(100); // Brief delay for UI feedback

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
                            SharedServices.PageDataTransferModel.Name = personaObj?.Name;
                            SharedServices.PageDataTransferModel.IsMarkedAsViewed = personaObj?.ItemReadStatus ?? false;
                        });

                        taskGroup.WaitAll();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.CaptureException(ex);
                    }
                });

                // Update navigation message
                personaListViewModel.NavigationMessage = "Opening details...";
                await Task.Delay(100); // Brief delay for UI feedback

                var route = $"{nameof(PersonaDetailPage)}?MasterId={_masterId}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                // Always clean up navigation state
                personaListViewModel.IsNavigating = false;
                personaListViewModel.IsBusy = false;
                if (personaObj != null)
                {
                    personaObj.IsBusy = false;
                }
            }
        }

        #region Filter
        
        
        private async Task RefreshListOfListFilter()
        {
            await Task.Run(() =>
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (listOfItems.DataSource != null)
                        {
                            listOfItems.DataSource.Filter = FilterPersonas;
                            listOfItems.DataSource.RefreshFilter();
                        }
                    });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            });
        }

        private bool FilterPersonas(object obj)
        {
            try
            {
                var persona = obj as PersonaViewModel;
                var filterText = string.Empty;
                
                // Priority order for filter text:
                // 1. Selected item from autocomplete
                // 2. SearchItemName from view model
                // 3. Current text in autocomplete (for when user types but hasn't selected)
                if (autoComplete.SelectedItem != null)
                {
                    filterText = (autoComplete.SelectedItem as WikiExtractor.ViewModels.PersonaAutoCompleteModel)?.Name ?? string.Empty;
                }
                else if (personaListViewModel.SearchItemName.HasValue())
                {
                    filterText = personaListViewModel.SearchItemName;
                }
                else if (!string.IsNullOrWhiteSpace(autoComplete.Text))
                {
                    filterText = autoComplete.Text;
                }

                // Apply read status filter first
                if (personaListViewModel.HideItemRead && persona.ItemReadStatus)
                {
                    return false;
                }
                
                // If no filter text, show all items (that pass read status filter)
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    return true;
                }

                // Apply text filter using contains logic
                return persona.Name.ContainsIgnoreCase(filterText);
            }
            catch (OperationCanceledException)
            {
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                return true;
            }
        }
        #endregion

        #region Auto Complete
        private async void AutoComplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    personaListViewModel.IsBusy = true;
                    if (sender is SfAutocomplete autocomplete)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Check if the autocomplete text is empty (cleared) - this is the key fix from LoanCalculator
                            if (string.IsNullOrWhiteSpace(autocomplete.Text))
                            {
                                // Text was cleared - reset selection and refresh to show all items
                                autocomplete.SelectedItem = null;
                                personaListViewModel.SearchItemName = "";
                                
                                if (listOfItems.DataSource != null)
                                {
                                    listOfItems.DataSource.Filter = FilterPersonas;
                                    listOfItems.DataSource.RefreshFilter();
                                }
                                
                                // Close the dropdown
                                autocomplete.IsDropDownOpen = false;
                            }
                            else if (listOfItems.DataSource != null)
                            {
                                // Handle normal selection changes when text is not empty
                                // Handle item selection
                                if (e.AddedItems != null && e.AddedItems.Count > 0)
                                {
                                    var castedArgs = e.AddedItems[0] as WikiExtractor.ViewModels.PersonaAutoCompleteModel;
                                    if (castedArgs != null)
                                    {
                                        personaListViewModel.SearchItemName = castedArgs.Name;
                                        listOfItems.DataSource.Filter = FilterPersonas;
                                        listOfItems.DataSource.RefreshFilter();
                                    }
                                }
                                // Handle item deselection or clearing
                                else if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                                {
                                    personaListViewModel.SearchItemName = "";
                                    listOfItems.DataSource.Filter = FilterPersonas;
                                    listOfItems.DataSource.RefreshFilter();
                                }
                                // Handle case where selection is cleared but no specific removed items
                                else if (autocomplete.SelectedItem == null)
                                {
                                    personaListViewModel.SearchItemName = "";
                                    listOfItems.DataSource.Filter = FilterPersonas;
                                    listOfItems.DataSource.RefreshFilter();
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
                finally
                {
                    personaListViewModel.IsBusy = false;
                }
            });
        }

        private async void AutoComplete_Unfocused(object sender, FocusEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (sender is SfAutocomplete autocomplete)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (listOfItems.DataSource != null)
                            {
                                // Refresh the filter when autocomplete loses focus
                                // This will handle cases where text was cleared but no selection change occurred
                                listOfItems.DataSource.Filter = FilterPersonas;
                                listOfItems.DataSource.RefreshFilter();
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            });
        }

        
        #endregion


        private async void BtnThemePick_Clicked(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Cycle through all available themes: Light -> Dark -> Forest -> Light
                        var currentTheme = SettingsHelper.SelectedTheme;
                        var nextTheme = currentTheme switch
                        {
                            WikiExtractor.Maui.App.Services.AppThemes.Light => WikiExtractor.Maui.App.Services.AppThemes.Dark,
                            WikiExtractor.Maui.App.Services.AppThemes.Dark => WikiExtractor.Maui.App.Services.AppThemes.Forest,
                            WikiExtractor.Maui.App.Services.AppThemes.Forest => WikiExtractor.Maui.App.Services.AppThemes.Light,
                            _ => WikiExtractor.Maui.App.Services.AppThemes.Light // Default fallback
                        };
                        
                        SettingsHelper.SaveTheme(nextTheme);
                        personaListViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();
                        ThemeHelper.UpdateAppThemes(personaListViewModel.DefaultStyle);
                    });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            });
        }

        private async void LstItemEffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (sender is SfEffectsView effectsView && effectsView.AutomationId != null)
                    {
                        _masterId = int.Parse(effectsView.AutomationId);
                        
                        // Track user interaction for interstitial ad frequency
                        _adManager?.TrackUserInteraction();
                        
                        await ProcessRequestToSubPage();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
        
        private async void BtnTakeQuiz_OnClicked(object sender, EventArgs e)
        {
            // Track user interaction for interstitial ad frequency
            _adManager?.TrackUserInteraction();
            
            await Shell.Current.GoToAsync($"{nameof(QuizPage)}");
        }

        #region Banner Ad Event Handlers

        private void OnBannerAdLoaded(object sender, AdLoadedEventArgs e)
        {
            try
            {
                // Banner ad loaded successfully
                System.Diagnostics.Debug.WriteLine($"Banner ad loaded successfully for {e.BannerType}");
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void OnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            try
            {
                // Banner ad failed to load
                System.Diagnostics.Debug.WriteLine($"Banner ad failed to load: {e.ErrorMessage}");
                ExceptionHandler.CaptureException(new Exception($"Banner ad load failed: {e.ErrorMessage}"));
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void OnBannerAdClicked(object sender, AdClickedEventArgs e)
        {
            try
            {
                // Banner ad was clicked
                System.Diagnostics.Debug.WriteLine($"Banner ad clicked for {e.BannerType}");
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        #endregion
    }
}