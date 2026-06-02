using Pj.Library;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.Buttons;
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
        private string? _loadedTagKey;

        public WikiListOfItemsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                /*loadingFactsControl?.Hide();*/
                /*navigationLoadingFactsControl?.Hide();*/

                Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
                Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;

                var currentTagKey = string.Join(",", Tags);
                bool tagChanged = _loadedTagKey != null && _loadedTagKey != currentTagKey;

                if (BindingContext == null || personaListViewModel == null || tagChanged)
                {
                    // First load or different flyout item — reset view model so list and title reload fresh
                    personaListViewModel = new PersonaListViewModel();
                    personaListViewModel.IsOffline = Connectivity.Current.NetworkAccess != NetworkAccess.Internet;
                    BindingContext = personaListViewModel;
                    personaListViewModel.IsDataLoading = true;
                    personaListViewModel.LoadingMessage = "Initializing list...";

                    await LoadInitialData();
                }
                else
                {
                    personaListViewModel.IsOffline = Connectivity.Current.NetworkAccess != NetworkAccess.Internet;
                    personaListViewModel.IsDataLoading = true;
                    personaListViewModel.LoadingMessage = "Refreshing data...";

                    await LoadRefreshData();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                if (personaListViewModel != null)
                    personaListViewModel.IsDataLoading = false;
            }
            finally
            {
                if (personaListViewModel != null)
                    personaListViewModel.IsPageBusy = false;
                try { autoComplete?.Unfocus(); } catch { }
                ApplyFontFamilyToAutocomplete();
            }
            ShowDailyStreakToast();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
            base.OnDisappearing();
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var isOffline = e.NetworkAccess != NetworkAccess.Internet;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (personaListViewModel != null)
                    personaListViewModel.IsOffline = isOffline;
            });
        }

        private async Task LoadInitialData()
        {
            try
            {
                personaListViewModel.IsDataLoading = true;
                personaListViewModel.LoadingMessage = "Loading persona data...";

                var tagKey = string.Join(",", Tags);
                var preloaded = SharedServices.ConsumePreloadedPersonas(tagKey);

                List<WikiExtractor.ViewModels.PersonaViewModel> personas;
                if (preloaded != null)
                {
                    // Data was pre-loaded during splash — no popup needed
                    personas = preloaded;
                }
                else
                {
                    personas = await Task.Run(() => SharedServices.WikiAppController.GetListOfWikiItems(Tags).ToList());
                }

                var titleTask = Task.Run(() =>
                {
                    var allItems = SharedServices.WikiAppController.AppMenuItems().ToList();
                    // Match by tag key; if tag is empty (Android first launch), fall back to first item
                    var match = tagKey.Length > 0
                        ? allItems.FirstOrDefault(f => f.Tags == tagKey)
                        : allItems.FirstOrDefault();
                    return match?.MenuItemName ?? string.Empty;
                });
                var hideReadTask = Task.Run(() => SettingsHelper.ShouldShowAlreadyReadItem());
                var sortIndexTask = Task.Run(() => Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)), SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));
                var showFavTask = Task.Run(() => SettingsHelper.GetShowFavouritesOnly());

                await Task.WhenAll(titleTask, hideReadTask, sortIndexTask, showFavTask);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    personaListViewModel.Personas = personas;
                    personaListViewModel.Title = titleTask.Result;
                    personaListViewModel.HideItemRead = hideReadTask.Result;
                    personaListViewModel.SortBySelectedIndex = sortIndexTask.Result;
                    personaListViewModel.ShowFavouritesOnly = showFavTask.Result;
                    lblNavTitle.Text = titleTask.Result;
                });

                personaListViewModel.LoadingMessage = "Building autocomplete list...";
                var autoList = personas.Select(f => new PersonaAutoCompleteModel { Id = f.Id, Name = f.Name }).ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                    personaListViewModel.AutocompleteList = autoList);

                personaListViewModel.LoadingMessage = "Applying filters...";
                await MainThread.InvokeOnMainThreadAsync(() => personaListViewModel.ApplyFilter());
                _loadedTagKey = tagKey;
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                /*loadingFactsControl.Hide();*/
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

                /*var loadingModel = new LoadingFactsModel
                {
                    ShowFacts = false,
                    LoadingText = "Refreshing data..."
                };
                loadingFactsControl.Show(loadingModel);*/

                personaListViewModel.LoadingMessage = "Checking read status...";

                // Snapshot the list on the main thread before handing to background work
                var personasSnapshot = personaListViewModel.Personas?.ToList() ?? [];

                var readStatusTask = Task.Run(() =>
                {
                    var trackData = SharedServices.WikiAppController.GetItemReadTrackData();
                    var mapData = (from data in personasSnapshot
                                   join tagItemJoin in trackData on data.Name equals tagItemJoin.ItemIdentifier
                                   select new { Data = data, Status = tagItemJoin.IsReadAsBool }).ToList();

                    bool localStatusChanged = false;
                    foreach (var item in mapData)
                        item.Data.ItemReadStatus = item.Status;

                    // Batch-sync IsFavourite for all items so cross-menu favouriting is reflected
                    var favData = SharedServices.WikiAppController.GetFavouriteTrackData();
                    var favLookup = favData.ToDictionary(f => f.ItemIdentifier, f => f.IsFavouriteAsBool, StringComparer.OrdinalIgnoreCase);
                    foreach (var persona in personasSnapshot)
                    {
                        var isFav = favLookup.TryGetValue(persona.Name, out var v) && v;
                        if (persona.IsFavourite != isFav)
                        {
                            persona.IsFavourite = isFav;
                            localStatusChanged = true;
                        }
                    }

                    if (SharedServices.PageDataTransferModel.Name.HasValue())
                    {
                        var targetName = SharedServices.PageDataTransferModel.Name;
                        var isMarked = SharedServices.PageDataTransferModel.IsMarkedAsViewed;

                        foreach (var item in personasSnapshot.Where(f => f.Name == targetName))
                            item.ItemReadStatus = isMarked;

                        // Always re-filter when returning from a detail page — the item's
                        // read/fav state may have changed and the DB read above may already
                        // reflect the new value, so diffing gives a false "no change".
                        localStatusChanged = true;
                        SharedServices.PageDataTransferModel.Clear();
                    }

                    return localStatusChanged;
                });

                var settingsTask = Task.Run(() => SettingsHelper.ShouldShowAlreadyReadItem());
                var showFavTask = Task.Run(() => SettingsHelper.GetShowFavouritesOnly());

                var sortIndexTask = Task.Run(() =>
                    Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)),
                    SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor())));

                await Task.WhenAll(readStatusTask, settingsTask, showFavTask, sortIndexTask);

                hasReadStatusChanged = readStatusTask.Result;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // HideItemRead setter now calls ApplyFilter() when the value changes,
                    // so no separate ApplyFilter call is needed for the hide-read toggle path.
                    personaListViewModel.HideItemRead = settingsTask.Result;
                    personaListViewModel.ShowFavouritesOnly = showFavTask.Result;
                    personaListViewModel.SortBySelectedIndex = sortIndexTask.Result;
                });

                if (hasReadStatusChanged)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => personaListViewModel.ApplyFilter());
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                /*loadingFactsControl.Hide();*/
                personaListViewModel.IsDataLoading = false;
            }
        }

        private async Task ProcessRequestToSubPage()
        {
            try
            {
                var personaObj = personaListViewModel.Personas.FirstOrDefault(f => f.Id == _masterId);

                if (personaObj != null)
                {
                    personaObj.IsPageBusy = true;
                }

                SharedServices.PageDataTransferModel.Clear();
                SharedServices.PageDataTransferModel.Id = _masterId;
                SharedServices.PageDataTransferModel.Name = personaObj?.Name;
                SharedServices.PageDataTransferModel.IsMarkedAsViewed = personaObj?.ItemReadStatus ?? false;

                SharedServiceCore.AdManager?.RecordUserInteraction();
                await (SharedServiceCore.AdManager?.TryShowInterstitialAdAsync() ?? Task.FromResult(false));

                var route = $"{nameof(PersonaDetailPage)}?MasterId={_masterId}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        #region Search / Filter

        private void RefreshFilter()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try { personaListViewModel?.ApplyFilter(); }
                catch (Exception ex) { ExceptionHandler.CaptureException(ex); }
            });
        }

        #endregion

        #region Auto Complete

        private static readonly GridLength[] _toolbarCollapsedWidths = { new GridLength(80), new GridLength(32), new GridLength(32), new GridLength(32), new GridLength(32) };

        private bool _toolbarExpanded;

        private void autoComplete_Focused(object sender, FocusEventArgs e)
        {
            try
            {
                if (toolbarGrid?.ColumnDefinitions == null || toolbarGrid.ColumnDefinitions.Count < 6) return;
                for (int i = 1; i <= 5; i++)
                    toolbarGrid.ColumnDefinitions[i].Width = new GridLength(0);
                toolbarGrid.ColumnSpacing = 0;
                quizButtonContainer.IsVisible = false;
                themeButtonContainer.IsVisible = false;
                settingsButtonContainer.IsVisible = false;
                btnFavouritesFilter.IsVisible = false;
                btnShuffleItem.IsVisible = false;
                _toolbarExpanded = true;
            }
            catch (Exception ex) { ExceptionHandler.CaptureException(ex); }
        }

        private void autoComplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            if (sender is not SfAutocomplete autocomplete) return;
            if (personaListViewModel == null) return;

            try
            {
                personaListViewModel.IsPageBusy = true;

                if (string.IsNullOrWhiteSpace(autocomplete.Text))
                {
                    autocomplete.SelectedItem = null;
                    personaListViewModel.SearchText = string.Empty;
                    autocomplete.IsDropDownOpen = false;
                }
                else if (e.AddedItems?.Count > 0)
                {
                    var selected = e.AddedItems[0] as PersonaAutoCompleteModel;
                    personaListViewModel.SearchText = selected?.Name ?? string.Empty;
                    autocomplete.IsDropDownOpen = false;
                    autocomplete.Unfocus();
                }
                else if (autocomplete.SelectedItem == null)
                {
                    personaListViewModel.SearchText = string.Empty;
                }

                RefreshFilter();
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
            if (personaListViewModel == null) return;

            try
            {
                // Restore toolbar columns
                if (toolbarGrid?.ColumnDefinitions != null && toolbarGrid.ColumnDefinitions.Count >= 6)
                {
                    for (int i = 1; i <= 5; i++)
                        toolbarGrid.ColumnDefinitions[i].Width = _toolbarCollapsedWidths[i - 1];
                    toolbarGrid.ColumnSpacing = 6;
                }
                quizButtonContainer.IsVisible = true;
                themeButtonContainer.IsVisible = true;
                settingsButtonContainer.IsVisible = true;
                btnFavouritesFilter.IsVisible = true;
                btnShuffleItem.IsVisible = true;
                _toolbarExpanded = false;

                if (autocomplete.SelectedItem == null)
                    personaListViewModel.SearchText = autocomplete.Text ?? string.Empty;

                RefreshFilter();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void autoComplete_ValueChanged(object sender, AutocompleteValueChangedEventArgs e)
        {
            if (personaListViewModel == null) return;

            try
            {
                if (string.IsNullOrEmpty(e.NewValue?.ToString()))
                {
                    personaListViewModel.SearchText = string.Empty;

                    if (sender is SfAutocomplete autocomplete)
                    {
                        autocomplete.SelectedItem = null;
                        autocomplete.Unfocus();
                    }

                    RefreshFilter();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void btnClearSearch_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                autoComplete.IsDropDownOpen = false;
                autoComplete.SelectedItem = null;
                autoComplete.Text = string.Empty;

                if (personaListViewModel != null)
                {
                    personaListViewModel.SearchText = string.Empty;
                    RefreshFilter();
                }

                // Defer the second clear pass so Syncfusion processes the tap first,
                // which ensures the visual text is wiped even when no item was selected
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        autoComplete.IsDropDownOpen = false;
                        autoComplete.Text = string.Empty;
                        if (_toolbarExpanded)
                            autoComplete.Unfocus();
                    }
                    catch { }
                });
            }
            catch (Exception ex) { ExceptionHandler.CaptureException(ex); }
        }

        #endregion

        private async void itemOnList_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (e.Parameter is int id)
                {
                    _masterId = id;
                    await ProcessRequestToSubPage();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private bool _isThemeBusy;

        private async void BtnThemePick_Clicked(object sender, EventArgs e)
        {
            if (_isThemeBusy) return;
            _isThemeBusy = true;
            btnThemePick.IsEnabled = false;
            themeSpinner.IsVisible = true;
            try
            {
                var currentTheme = await Task.Run(() => SettingsHelper.SelectedTheme);
                var nextTheme = currentTheme switch
                {
                    WikiExtractor.Maui.App.Services.AppThemes.Light   => WikiExtractor.Maui.App.Services.AppThemes.Dark,
                    WikiExtractor.Maui.App.Services.AppThemes.Dark     => WikiExtractor.Maui.App.Services.AppThemes.Forest,
                    WikiExtractor.Maui.App.Services.AppThemes.Forest   => WikiExtractor.Maui.App.Services.AppThemes.Candy,
                    WikiExtractor.Maui.App.Services.AppThemes.Candy    => WikiExtractor.Maui.App.Services.AppThemes.Sunset,
                    WikiExtractor.Maui.App.Services.AppThemes.Sunset   => WikiExtractor.Maui.App.Services.AppThemes.Ocean,
                    WikiExtractor.Maui.App.Services.AppThemes.Ocean    => WikiExtractor.Maui.App.Services.AppThemes.Light,
                    _ => WikiExtractor.Maui.App.Services.AppThemes.Light
                };
                await Task.Run(async () =>
                {
                    SettingsHelper.SaveTheme(nextTheme);
                    await SharedServiceCore.SaveData(new WikiExtractor.Maui.App.Services.ThemeSelect { Theme = nextTheme });
                    AppSettingsService.SetThemeBackgroundColor(nextTheme);
                });
                await MainThread.InvokeOnMainThreadAsync(() =>
                    SharedServiceCore.ThemeHandler?.LoadDefaultStyle(nextTheme));
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                _isThemeBusy = false;
                themeSpinner.IsVisible = false;
                btnThemePick.IsEnabled = true;
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

        private void BtnFavouritesFilter_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (personaListViewModel == null) return;
                var newValue = !personaListViewModel.ShowFavouritesOnly;
                personaListViewModel.ShowFavouritesOnly = newValue;
                Task.Run(() => SettingsHelper.SaveShowFavouritesOnly(newValue));
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private async void BtnShuffleItem_Clicked(object sender, TappedEventArgs e)
        {
            try
            {
                var list = personaListViewModel?.FilteredPersonas;
                if (list == null || list.Count == 0) return;
                var random = list[new Random().Next(list.Count)];
                _masterId = random.Id;
                await ProcessRequestToSubPage();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private async void heartOnListItem_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (e.Parameter is not int id) return;
                var persona = personaListViewModel?.Personas?.FirstOrDefault(f => f.Id == id);
                if (persona == null) return;

                var newValue = !persona.IsFavourite;
                await Task.Run(() => SharedServices.WikiAppController.UpdateFavourite(persona.Name, newValue));
                persona.IsFavourite = newValue;

                if (personaListViewModel!.ShowFavouritesOnly)
                    personaListViewModel.ApplyFilter();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private async void BtnTakeQuiz_OnClicked(object sender, EventArgs e)
        {
            try
            {
                SharedServiceCore.AdManager?.RecordUserInteraction();
                await (SharedServiceCore.AdManager?.TryShowInterstitialAdAsync() ?? Task.FromResult(false));
                await Shell.Current.GoToAsync($"{nameof(QuizPage)}");
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void ShowDailyStreakToast()
        {
            try
            {
                var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
                var lastShown = Preferences.Get("StreakToastLastShownDate", string.Empty);
                if (lastShown == today) return;
                Preferences.Set("StreakToastLastShownDate", today);

                var streak = Task.Run(() => SharedServices.WikiAppController.UpdateStreak()).GetAwaiter().GetResult();
                var message = streak.CurrentStreak == 1
                    ? "Welcome back! Start your streak today."
                    : $"Day {streak.CurrentStreak} streak! Keep it going.";

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        streakBannerText.Text = message;
                        streakBanner.Opacity = 0;
                        streakBanner.TranslationY = -60;
                        streakBanner.IsVisible = true;

                        await Task.WhenAll(
                            streakBanner.FadeTo(1, 300, Easing.CubicOut),
                            streakBanner.TranslateTo(0, 0, 300, Easing.CubicOut));

                        await Task.Delay(3000);

                        await Task.WhenAll(
                            streakBanner.FadeTo(0, 400, Easing.CubicIn),
                            streakBanner.TranslateTo(0, -60, 400, Easing.CubicIn));

                        streakBanner.IsVisible = false;
                    }
                    catch { }
                });
            }
            catch { }
        }

        private void ApplyFontFamilyToAutocomplete()        {
            try
            {
                if (autoComplete == null) return;
                if (Application.Current?.Resources == null) return;
                if (!Application.Current.Resources.TryGetValue("DefaultFontFamily", out var fontFamilyValue)) return;
                var fontFamily = fontFamilyValue as string;
                if (string.IsNullOrEmpty(fontFamily)) return;
                autoComplete.FontFamily = fontFamily;
                autoComplete.DropDownItemFontFamily = fontFamily;
            }
            catch { }
        }

        // Kept for backwards compatibility — not wired in new XAML but may be called from AppShell
        private async void itemOnList_TouchUp(object sender, EventArgs e)
        {
            try
            {
                if (sender is Syncfusion.Maui.Core.SfEffectsView effectsView &&
                    effectsView.AutomationId != null &&
                    int.TryParse(effectsView.AutomationId, out var parsedId))
                {
                    _masterId = parsedId;
                    await ProcessRequestToSubPage();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
    }
}
