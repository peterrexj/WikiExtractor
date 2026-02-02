using Pj.Library;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.Core;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels;
using WikiExtractor.ViewModels;
using PjAds.Maui.Services;
using PjAds.Maui.Models;
using WikiExtractor.Maui.App.Models;

namespace WikiExtractor.Maui.App.Views
{
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

        protected override async void OnAppearing()
        {
            try
            {
                // Ensure loading controls are hidden when page appears (e.g., navigating back)
                loadingFactsControl?.Hide();
                navigationLoadingFactsControl?.Hide();
                
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
                    personaListViewModel.IsPageBusy = false;
                }
                autoComplete.Unfocus();
            }
            base.OnAppearing();
        }

        private async Task LoadInitialData()
        {
            try
            {
                personaListViewModel.IsDataLoading = true;
                personaListViewModel.LoadingMessage = "Loading persona data...";
                
                // Initialize loading facts control in lite mode
                var loadingModel = new LoadingFactsModel
                {
                    ShowFacts = false,
                    LoadingText = "Loading list..."
                };
                loadingFactsControl.Show(loadingModel);
                
                await Task.Delay(200); // Small delay to let UI show the loading state

                // 1. Fetch data in parallel on background threads
                var personasTask = Task.Run(() => SharedServices.WikiAppController.GetListOfWikiItems(Tags).ToList());
                var titleTask = Task.Run(() => SharedServices.WikiAppController.AppMenuItems().FirstOrDefault(f => f.Tags == string.Join(",", Tags))?.TitleOnThePage ?? string.Empty);
                var hideReadTask = Task.Run(() => SettingsHelper.ShouldShowAlreadyReadItem());
                var sortIndexTask = Task.Run(() => Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)), SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));

                await Task.WhenAll(personasTask, titleTask, hideReadTask, sortIndexTask);

                // 2. Update the UI-bound properties on the Main Thread
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    personaListViewModel.Personas = personasTask.Result;
                    personaListViewModel.Title = titleTask.Result;
                    personaListViewModel.HideItemRead = hideReadTask.Result;
                    personaListViewModel.SortBySelectedIndex = sortIndexTask.Result;
                });

                // 3. Process Autocomplete (can be done on background, but assigned on Main)
                personaListViewModel.LoadingMessage = "Building autocomplete list...";
                var autoList = personasTask.Result.Select(f => new WikiExtractor.ViewModels.PersonaAutoCompleteModel { Id = f.Id, Name = f.Name }).ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                    personaListViewModel.AutocompleteList = autoList);

                // 4. Apply Filters
                personaListViewModel.LoadingMessage = "Applying filters...";
                await Task.Run(RefreshListOfListFilter);

                personaListViewModel.LoadingMessage = "Finalizing...";
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                // Use finally to ensure loading always stops even on error
                loadingFactsControl.Hide();
                await MainThread.InvokeOnMainThreadAsync(() =>
                    personaListViewModel.IsDataLoading = false);
            }
        }

        private async Task LoadRefreshData()
        {
            try
            {
                personaListViewModel.IsDataLoading = true;
                bool hasReadStatusChanged = false;
                bool hasItemReadToggled = false;

                // Show loading facts control for refresh in lite mode
                var loadingModel = new LoadingFactsModel
                {
                    ShowFacts = false,
                    LoadingText = "Refreshing data..."
                };
                loadingFactsControl.Show(loadingModel);

                personaListViewModel.LoadingMessage = "Checking read status...";
                await Task.Delay(200);

                // 1. Prepare Background Tasks
                var readStatusTask = Task.Run(() =>
                {
                    // Perform the heavy LINQ join on a background thread
                    var trackData = SharedServices.WikiAppController.GetItemReadTrackData();

                    var mapData = (from data in personaListViewModel.Personas
                                   join tagItemJoin in trackData on data.Name equals tagItemJoin.ItemIdentifier
                                   select new { Data = data, Status = tagItemJoin.IsReadAsBool }).ToList();

                    // Local flags to avoid direct VM property access inside the background loop
                    bool localStatusChanged = false;

                    foreach (var item in mapData)
                    {
                        item.Data.ItemReadStatus = item.Status;
                    }

                    // Handle Page Data Transfer
                    if (SharedServices.PageDataTransferModel.Name.HasValue())
                    {
                        var targetName = SharedServices.PageDataTransferModel.Name;
                        var isMarked = SharedServices.PageDataTransferModel.IsMarkedAsViewed;

                        foreach (var item in personaListViewModel.Personas.Where(f => f.Name == targetName))
                        {
                            if (item.ItemReadStatus != isMarked) localStatusChanged = true;
                            item.ItemReadStatus = isMarked;
                        }
                        SharedServices.PageDataTransferModel.Clear();
                    }

                    return localStatusChanged;
                });

                var settingsTask = Task.Run(() =>
                {
                    var hideItemReadFromStore = SettingsHelper.ShouldShowAlreadyReadItem();
                    bool toggled = hideItemReadFromStore != personaListViewModel.HideItemRead;
                    return new { Hide = hideItemReadFromStore, Toggled = toggled };
                });

                var sortIndexTask = Task.Run(() =>
                    Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)),
                    SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));

                // 2. Wait for all
                await Task.WhenAll(readStatusTask, settingsTask, sortIndexTask);

                // 3. Apply results to ViewModel on Main Thread
                hasReadStatusChanged = readStatusTask.Result;
                hasItemReadToggled = settingsTask.Result.Toggled;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    personaListViewModel.HideItemRead = settingsTask.Result.Hide;
                    personaListViewModel.SortBySelectedIndex = sortIndexTask.Result;
                    personaListViewModel.SearchItemName = ""; // Clear search on refresh
                });

                // 4. Trigger Filter Refresh if needed
                if (hasReadStatusChanged || hasItemReadToggled)
                {
                    personaListViewModel.LoadingMessage = "Updating filters...";
                    await Task.Delay(200);
                    await Task.Run(RefreshListOfListFilter);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                personaListViewModel.LoadingMessage = "Finalizing...";
                await Task.Delay(100);
                loadingFactsControl.Hide();
                personaListViewModel.IsDataLoading = false;
            }
        }

        private async Task ProcessRequestToSubPage()
        {
            PersonaViewModel personaObj = null;

            try
            {
                // 1. Start UI feedback immediately on the Main Thread
                personaListViewModel.IsNavigating = true;
                personaListViewModel.IsPageBusy = true;
                personaListViewModel.NavigationMessage = "Preparing navigation...";

                // Initialize navigation loading facts control
                var loadingModel = new LoadingFactsModel
                {
                    FactCount = 3,
                    FactDisplayDurationMs = 3000,
                    ShowMasterImage = true,
                    AutoMarkFactsAsShown = true,
                    MasterId = _masterId
                };
                navigationLoadingFactsControl.Show(loadingModel);

                // 2. Perform logic. No Task.Run needed here as these are simple assignments.
                personaObj = personaListViewModel.Personas.FirstOrDefault(f => f.Id == _masterId);

                if (personaObj != null)
                {
                    personaObj.IsPageBusy = true;
                }

                // 3. Prepare Data Transfer Service
                SharedServices.PageDataTransferModel.Clear();
                SharedServices.PageDataTransferModel.Id = _masterId;
                SharedServices.PageDataTransferModel.Name = personaObj?.Name;
                SharedServices.PageDataTransferModel.IsMarkedAsViewed = personaObj?.ItemReadStatus ?? false;

                // 4. Update message and allow the UI to render the change
                personaListViewModel.NavigationMessage = "Opening details...";
                await Task.Delay(50); // Smallest delay just to ensure the message renders

                // 5. Navigate
                var route = $"{nameof(PersonaDetailPage)}?MasterId={_masterId}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                // 6. Clean up state
                navigationLoadingFactsControl.Hide();
                personaListViewModel.IsNavigating = false;
                personaListViewModel.IsPageBusy = false;
                if (personaObj != null)
                {
                    personaObj.IsPageBusy = false;
                }
            }
        }

        #region Filter

        private void RefreshListOfListFilter()
        {
            // No Task.Run needed for UI-thread operations
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (listOfItems.DataSource != null)
                    {
                        // Assigning the delegate only once is better performance, 
                        // but re-assigning is fine if the logic inside FilterPersonas changes.
                        listOfItems.DataSource.Filter = FilterPersonas;
                        listOfItems.DataSource.RefreshFilter();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            });
        }

        private bool FilterPersonas(object obj)
        {
            if (obj is not PersonaViewModel persona) return false;

            // Apply "Hide Read" logic first - it's the fastest "exit" for the filter
            if (personaListViewModel.HideItemRead && persona.ItemReadStatus)
                return false;

            string filterText = string.Empty;

            // Retrieve filter text safely from UI components
            if (autoComplete.SelectedItem is WikiExtractor.ViewModels.PersonaAutoCompleteModel selected)
            {
                filterText = selected.Name;
            }
            else if (!string.IsNullOrWhiteSpace(personaListViewModel.SearchItemName))
            {
                filterText = personaListViewModel.SearchItemName;
            }
            else if (!string.IsNullOrWhiteSpace(autoComplete.Text))
            {
                filterText = autoComplete.Text;
            }

            if (string.IsNullOrWhiteSpace(filterText))
                return true;

            return persona.Name.ContainsIgnoreCase(filterText);
        }
        #endregion

        #region Auto Complete
        private void autoComplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            if (sender is not SfAutocomplete autocomplete) return;

            try
            {
                personaListViewModel.IsPageBusy = true;

                // 1. Determine the Search Name
                if (string.IsNullOrWhiteSpace(autocomplete.Text))
                {
                    autocomplete.SelectedItem = null;
                    personaListViewModel.SearchItemName = string.Empty;
                    autocomplete.IsDropDownOpen = false;
                }
                else if (e.AddedItems?.Count > 0)
                {
                    var selected = e.AddedItems[0] as WikiExtractor.ViewModels.PersonaAutoCompleteModel;
                    personaListViewModel.SearchItemName = selected?.Name ?? string.Empty;
                }
                else if (autocomplete.SelectedItem == null)
                {
                    personaListViewModel.SearchItemName = string.Empty;
                }

                // 2. Refresh the UI List
                RefreshListOfListFilter();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                personaListViewModel.IsPageBusy = false;
            }
        }

        private void autoComplete_Unfocused(object sender, FocusEventArgs e)
        {
            if (sender is not SfAutocomplete autocomplete) return;

            try
            {
                // 1. If no item was selected, treat the typed text as the filter
                if (autocomplete.SelectedItem == null)
                {
                    // Update the ViewModel with the raw text currently in the box
                    personaListViewModel.SearchItemName = autocomplete.Text ?? string.Empty;
                }

                // 2. Refresh the list
                if (listOfItems?.DataSource != null)
                {
                    // The FilterPersonas method will now use the SearchItemName we just set
                    listOfItems.DataSource.Filter = FilterPersonas;
                    listOfItems.DataSource.RefreshFilter();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void autoComplete_ValueChanged(object sender, AutocompleteValueChangedEventArgs e)
        {
            try
            {
                // When the clear button is clicked, NewValue becomes null or empty
                if (string.IsNullOrEmpty(e.NewValue?.ToString()))
                {
                    // 1. Reset the underlying filter value
                    personaListViewModel.SearchItemName = string.Empty;

                    // 2. Ensure SelectedItem is also cleared
                    if (sender is SfAutocomplete autocomplete)
                    {
                        autocomplete.SelectedItem = null;
                    }

                    // 3. Refresh the UI list immediately
                    if (listOfItems?.DataSource != null)
                    {
                        // FilterPersonas will now return 'true' for all items
                        listOfItems.DataSource.RefreshFilter();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        #endregion


        private async void BtnThemePick_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
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
                    
                    // Apply the theme immediately
                    SharedServiceCore.ThemeHandler?.LoadDefaultStyle(nextTheme);
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private async void BtnSettings_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("settings");
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
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
                        //_adManager?.TrackUserInteraction();

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
            //_adManager?.TrackUserInteraction();

            await Shell.Current.GoToAsync($"{nameof(QuizPage)}");
        }


        private async void itemOnList_TouchUp(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (sender is SfEffectsView effectsView && effectsView.AutomationId != null)
                    {
                        _masterId = int.Parse(effectsView.AutomationId);

                        // Track user interaction for interstitial ad frequency
                        //_adManager?.TrackUserInteraction();

                        await ProcessRequestToSubPage();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
    }
}