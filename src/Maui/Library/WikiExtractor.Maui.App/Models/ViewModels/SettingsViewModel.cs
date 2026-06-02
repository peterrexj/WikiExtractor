using System.Collections.ObjectModel;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Models.Mix;
using Syncfusion.Maui.Buttons;
using WikiExtractor.ViewModels;
using Microsoft.Maui.Media;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IThemeHandler _themeHandler;
        private readonly IErrorHandlingService _errorHandlingService;
        private string? _selectedTheme;
        private string? _selectedFontFamily;
        private bool isUpdating;
        private bool _hideViewedItems;
        private int _sortBySelectedIndex;
        private float _speechPitch;
        private string? _selectedVoice;
        private bool _noAdsEnabled;
        private bool _noAdsBusy;
        private int _currentStreak;
        private int _bestStreak;
        private double _paragraphFontSize = AppSettingsService.DEFAULT_PARAGRAPH_FONT_SIZE;

        public ObservableCollection<string> Themes { get; }
        public ObservableCollection<string> FontFamilies { get; }
        public ObservableCollection<SfSegmentItem> SortByCollection { get; }
        public ObservableCollection<string> AvailableVoices { get; } = new();

        public bool NoAdsEnabled
        {
            get => _noAdsEnabled;
            private set { _noAdsEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowRemoveAdsButton)); }
        }

        public bool NoAdsBusy
        {
            get => _noAdsBusy;
            private set { _noAdsBusy = value; OnPropertyChanged(); }
        }

        public bool ShowRemoveAdsButton => !_noAdsEnabled;

        public int CurrentStreak
        {
            get => _currentStreak;
            private set { _currentStreak = value; OnPropertyChanged(); }
        }

        public int BestStreak
        {
            get => _bestStreak;
            private set { _bestStreak = value; OnPropertyChanged(); }
        }

        public double ParagraphFontSize
        {
            get => _paragraphFontSize;
            set
            {
                var clamped = Math.Max(AppSettingsService.MIN_PARAGRAPH_FONT_SIZE, Math.Min(AppSettingsService.MAX_PARAGRAPH_FONT_SIZE, value));
                if (Math.Abs(_paragraphFontSize - clamped) < 0.1) return;
                _paragraphFontSize = clamped;
                OnPropertyChanged();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current?.Resources != null)
                        Application.Current.Resources["WikiAppParagraphFontSize"] = clamped;
                });
                Task.Run(() => AppSettingsService.SetParagraphFontSizeAsync(clamped));
            }
        }

        public double MinParagraphFontSize => AppSettingsService.MIN_PARAGRAPH_FONT_SIZE;
        public double MaxParagraphFontSize => AppSettingsService.MAX_PARAGRAPH_FONT_SIZE;

        public Command RemoveAdsCommand { get; }
        public Command RestoreAdsCommand { get; }
        public Command ShareAppCommand { get; }
        public Command RateAppCommand { get; }
        public Command SendFeedbackCommand { get; }

        public string? SelectedTheme
        {
            get
            {
                if (_selectedTheme == null)
                {
                    _themeHandler.GetCurrentThemeAsync().ContinueWith(task =>
                    {
                        _selectedTheme = task.Result != null ?
                            Themes.FirstOrDefault(t => t == task.Result.ToString()) :
                            Themes.FirstOrDefault(t => t == SharedServiceCore.DefaultAppTheme.ToString());

                        MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(SelectedTheme)));
                    });
                }
                return _selectedTheme;
            }
            set
            {
                if (value == null) return;
                if (isUpdating) return;
                if (_selectedTheme == value) return;

                _selectedTheme = value;

                // Await the theme change operation
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    IsPageBusy = true; // Show spinner
                    IsUpdating = true;

                    await Task.Delay(500);
                    await ChangeThemeAsync(_selectedTheme);
                });
            }
        }

        public bool IsUpdating
        {
            get => isUpdating;
            set
            {
                isUpdating = value;
                OnPropertyChanged();
            }
        }

        public bool HideViewedItems
        {
            get => _hideViewedItems;
            set
            {
                if (_hideViewedItems != value)
                {
                    _hideViewedItems = value;
                    OnPropertyChanged();
                    Task.Run(() => SettingsHelper.SaveShouldShowAlreadyReadItems(value));
                }
            }
        }

        public int SortBySelectedIndex
        {
            get => _sortBySelectedIndex;
            set
            {
                if (_sortBySelectedIndex != value)
                {
                    _sortBySelectedIndex = value;
                    OnPropertyChanged();
                    var sortAttrib = (MainListSortDescriptorModel.SortByAttribute)Enum.ToObject(typeof(MainListSortDescriptorModel.SortByAttribute), value);
                    var sortInfo = SettingsHelper.GetSortDescriptorBySelectedItem(sortAttrib);
                    if (sortInfo != null)
                    {
                        Task.Run(() => SettingsHelper.SaveSortDescriptor(sortInfo.PropertyName, sortInfo.Direction.ToString()));
                    }
                }
            }
        }

        public float SpeechPitch
        {
            get => _speechPitch;
            set
            {
                if (Math.Abs(_speechPitch - value) < 0.01f) return;
                _speechPitch = value;
                OnPropertyChanged();
                Task.Run(() => SettingsHelper.SaveSpeechPitch(value));
            }
        }

        public string? SelectedVoice
        {
            get => _selectedVoice;
            set
            {
                if (_selectedVoice == value) return;
                _selectedVoice = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Task.Run(() =>
                    {
                        SettingsHelper.SaveSpeechVoice(value);
                        SettingsHelper.ResetSpeechSettings();
                    });
                }
            }
        }

        public async Task LoadVoicesAsync()
        {
            try
            {
                var savedVoice = await Task.Run(() => SettingsHelper.GetSpeechVoice());
                var locales = await TextToSpeech.GetLocalesAsync();
                var english = locales
                    .Where(l => l.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(l => l.Name)
                    .Select(l => l.Name)
                    .Distinct()
                    .ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AvailableVoices.Clear();
                    AvailableVoices.Add("System Default");
                    foreach (var v in english)
                        AvailableVoices.Add(v);

                    _selectedVoice = AvailableVoices.Contains(savedVoice) ? savedVoice : "System Default";
                    OnPropertyChanged(nameof(SelectedVoice));
                });
            }
            catch { }
        }

        public async Task LoadAsync()
        {
            try
            {
                var hideRead = await Task.Run(() => SettingsHelper.ShouldShowAlreadyReadItem());
                var sortDescriptor = await Task.Run(() => SettingsHelper.GetCurrentSortDescriptor());
                var sortIndex = Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)),
                    SettingsHelper.GetSortAttributeBySelected(sortDescriptor));
                var pitch = await Task.Run(() => SettingsHelper.GetSpeechPitch());
                var streak = await Task.Run(() => SharedServices.WikiAppController.GetStreak());
                var fontSize = await AppSettingsService.GetParagraphFontSizeAsync();

                var noAds = SharedServiceCore.NoAdsService?.IsNoAdsEnabled ?? false;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _hideViewedItems = hideRead;
                    OnPropertyChanged(nameof(HideViewedItems));
                    _sortBySelectedIndex = sortIndex;
                    OnPropertyChanged(nameof(SortBySelectedIndex));
                    _speechPitch = pitch;
                    OnPropertyChanged(nameof(SpeechPitch));
                    NoAdsEnabled = noAds;
                    CurrentStreak = streak.CurrentStreak;
                    BestStreak = streak.BestStreak;
                    _paragraphFontSize = fontSize;
                    OnPropertyChanged(nameof(ParagraphFontSize));
                });
            }
            catch { }
        }

        public string? SelectedFontFamily        {
            get
            {
                if (_selectedFontFamily == null)
                {
                    AppSettingsService.GetAppFontFamilyAsync().ContinueWith(task =>
                    {
                        _selectedFontFamily = task.Result;
                        MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(SelectedFontFamily)));
                    });
                }
                return _selectedFontFamily;
            }
            set
            {
                if (value == null) return;
                if (isUpdating) return;
                if (_selectedFontFamily == value) return;

                _selectedFontFamily = value;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    IsPageBusy = true;
                    IsUpdating = true;

                    await Task.Delay(300);
                    await ChangeFontFamilyAsync(_selectedFontFamily);
                });
            }
        }

        public SettingsViewModel(WikiExtractor.Maui.App.Services.IThemeHandler themeHandler, IErrorHandlingService errorHandlingService)
        {
            _themeHandler = themeHandler;
            _errorHandlingService = errorHandlingService;

            Themes = new ObservableCollection<string>(Enum.GetNames(typeof(WikiExtractor.Maui.App.Services.AppThemes)));

            // Get registered fonts from platform-specific implementation
            var registeredFonts = SharedServiceCore.AppInformation?.GetRegisteredFontFamilies() ?? new List<string> { "Calibri" };
            FontFamilies = new ObservableCollection<string>(registeredFonts);

            SortByCollection = new ObservableCollection<SfSegmentItem>
            {
                new SfSegmentItem { Text = "Default" },
                new SfSegmentItem { Text = "A-Z" },
                new SfSegmentItem { Text = "Z-A" },
                new SfSegmentItem { Text = "Read" },
                new SfSegmentItem { Text = "UnRead" },
                new SfSegmentItem { Text = "Random" }
            };

            RemoveAdsCommand = new Command(async () => await OnRemoveAdsAsync());
            RestoreAdsCommand = new Command(async () => await OnRestoreAdsAsync());
            ShareAppCommand = new Command(async () => await OnShareAppAsync());
            RateAppCommand = new Command(async () => await OnRateAppAsync());
            SendFeedbackCommand = new Command(async () => await OnSendFeedbackAsync());
        }

        // Default constructor for XAML instantiation
        public SettingsViewModel()
        {
            _themeHandler = SharedServiceCore.ThemeHandler;
            _errorHandlingService = SharedServiceCore.ErrorHandlingService;

            Themes = new ObservableCollection<string>(Enum.GetNames(typeof(WikiExtractor.Maui.App.Services.AppThemes)));

            // Get registered fonts from platform-specific implementation
            var registeredFonts = SharedServiceCore.AppInformation?.GetRegisteredFontFamilies() ?? new List<string> { "Calibri" };
            FontFamilies = new ObservableCollection<string>(registeredFonts);

            SortByCollection = new ObservableCollection<SfSegmentItem>
            {
                new SfSegmentItem { Text = "Default" },
                new SfSegmentItem { Text = "A-Z" },
                new SfSegmentItem { Text = "Z-A" },
                new SfSegmentItem { Text = "Read" },
                new SfSegmentItem { Text = "UnRead" },
                new SfSegmentItem { Text = "Random" }
            };

            RemoveAdsCommand = new Command(async () => await OnRemoveAdsAsync());
            RestoreAdsCommand = new Command(async () => await OnRestoreAdsAsync());
            ShareAppCommand = new Command(async () => await OnShareAppAsync());
            RateAppCommand = new Command(async () => await OnRateAppAsync());
            SendFeedbackCommand = new Command(async () => await OnSendFeedbackAsync());
        }

        private async Task ChangeThemeAsync(string selectedTheme)
        {
            try
            {
                var appTheme = WikiExtractor.Maui.App.Services.EnumHelper<WikiExtractor.Maui.App.Services.AppThemes>.FromString(selectedTheme);
                await SaveAndApplyApplicationThemeAsync(appTheme);

                OnPropertyChanged(nameof(SelectedTheme)); // Notify UI of the change
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleException(ex); // Handle any errors
            }
            finally
            {
                IsUpdating = false;
                IsPageBusy = false; // Hide spinner
            }
        }

        private async Task SaveAndApplyApplicationThemeAsync(WikiExtractor.Maui.App.Services.AppThemes theme)
        {
            await WikiExtractor.Maui.App.Services.SharedServiceCore.SaveData(new WikiExtractor.Maui.App.Services.ThemeSelect { Theme = theme });
            AppSettingsService.SetThemeBackgroundColor(theme);
            await ApplyApplicationThemeAsync(theme);
        }

        private async Task ApplyApplicationThemeAsync(WikiExtractor.Maui.App.Services.AppThemes theme)
        {
            await MainThread.InvokeOnMainThreadAsync(() => _themeHandler.LoadDefaultStyle(theme));
        }

        private async Task ChangeFontFamilyAsync(string fontFamily)
        {
            try
            {
                await AppSettingsService.SetAppFontFamilyAsync(fontFamily);
                await ApplyFontFamilyAsync(fontFamily);
                OnPropertyChanged(nameof(SelectedFontFamily));
            }
            catch (Exception ex)
            {
                _errorHandlingService?.HandleException(ex);
            }
            finally
            {
                IsUpdating = false;
                IsPageBusy = false;
            }
        }

        private async Task ApplyFontFamilyAsync(string fontFamily)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current?.Resources != null)
                {
                    Application.Current.Resources["DefaultFontFamily"] = fontFamily;
                }
            });
        }

        private async Task OnRemoveAdsAsync()
        {
            if (NoAdsBusy) return;
            var noAdsService = SharedServiceCore.NoAdsService;
            if (noAdsService == null) return;
            var productId = SharedServiceCore.AppInformation?.NoAdsProductId;
            if (string.IsNullOrEmpty(productId)) return;

            NoAdsBusy = true;
            try
            {
                var result = await noAdsService.PurchaseNoAdsAsync(productId);
                switch (result)
                {
                    case NoAdsPurchaseResult.Purchased:
                    case NoAdsPurchaseResult.AlreadyOwned:
                        NoAdsEnabled = true;
                        ApplyNoAdsToAdManager();
                        break;
                    case NoAdsPurchaseResult.Cancelled:
                        break;
                    case NoAdsPurchaseResult.Failed:
                        await Application.Current!.MainPage!.DisplayAlert("Purchase Failed", "Unable to complete the purchase. Please try again.", "OK");
                        break;
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService?.HandleException(ex);
            }
            finally
            {
                NoAdsBusy = false;
            }
        }

        private async Task OnRestoreAdsAsync()
        {
            if (NoAdsBusy) return;
            var noAdsService = SharedServiceCore.NoAdsService;
            if (noAdsService == null) return;
            var productId = SharedServiceCore.AppInformation?.NoAdsProductId;
            if (string.IsNullOrEmpty(productId)) return;

            NoAdsBusy = true;
            try
            {
                var restored = await noAdsService.RestoreNoAdsAsync(productId);
                if (restored)
                {
                    NoAdsEnabled = true;
                    ApplyNoAdsToAdManager();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Restore", "No previous purchase found.", "OK");
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService?.HandleException(ex);
            }
            finally
            {
                NoAdsBusy = false;
            }
        }

        private static void ApplyNoAdsToAdManager()
        {
            SharedServiceCore.DisableAds();
        }

        private async Task OnShareAppAsync()
        {
            try
            {
                var link = SharedServiceCore.AppInformation?.AppShareLink ?? "https://www.yoursimpleapps.com";
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = link,
                    Title = "Check out this app!"
                });
            }
            catch (Exception ex)
            {
                _errorHandlingService?.HandleException(ex);
            }
        }

        private async Task OnRateAppAsync()
        {
            try
            {
                var link = SharedServiceCore.AppInformation?.RateAppLink;
                if (string.IsNullOrEmpty(link)) return;

                var canOpen = await Launcher.Default.CanOpenAsync(link);
                if (!canOpen)
                {
                    // Fall back to the web share link (works on emulators / devices without the store app)
                    var webLink = SharedServiceCore.AppInformation?.AppShareLink;
                    if (!string.IsNullOrEmpty(webLink))
                        await Launcher.Default.OpenAsync(webLink);
                    return;
                }

                await Launcher.Default.OpenAsync(link);
            }
            catch (Exception ex)
            {
                _errorHandlingService?.HandleException(ex);
            }
        }

        private async Task OnSendFeedbackAsync()
        {
            try
            {
                var email = SharedServiceCore.AppInformation?.FeedbackEmail ?? "support@yoursimpleapps.com";
                var subject = Uri.EscapeDataString($"Feedback - {AppInfo.Current.Name}");
                await Launcher.Default.OpenAsync($"mailto:{email}?subject={subject}");
            }
            catch (Exception ex)
            {
                _errorHandlingService?.HandleException(ex);
            }
        }
    }
}