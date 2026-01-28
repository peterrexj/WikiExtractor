using System.Collections.ObjectModel;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Models.Mix;
using Syncfusion.Maui.Buttons;
using WikiExtractor.ViewModels;

namespace Maui.Wiki.ViewModels
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

        public ObservableCollection<string> Themes { get; }
        public ObservableCollection<string> FontFamilies { get; }
        public ObservableCollection<SfSegmentItem> SortByCollection { get; }

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
                        
                        OnPropertyChanged(nameof(SelectedTheme));
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
            get
            {
                if (_hideViewedItems == false)
                {
                    _hideViewedItems = SettingsHelper.ShouldShowAlreadyReadItem();
                }
                return _hideViewedItems;
            }
            set
            {
                if (_hideViewedItems != value)
                {
                    _hideViewedItems = value;
                    OnPropertyChanged();
                    // Save the setting immediately when changed
                    SettingsHelper.SaveShouldShowAlreadyReadItems(value);
                }
            }
        }

        public int SortBySelectedIndex
        {
            get
            {
                if (_sortBySelectedIndex == 0)
                {
                    _sortBySelectedIndex = Array.IndexOf(Enum.GetValues(typeof(MainListSortDescriptorModel.SortByAttribute)),
                        SettingsHelper.GetSortAttributeBySelected(SettingsHelper.GetCurrentSortDescriptor()));
                }
                return _sortBySelectedIndex;
            }
            set
            {
                if (_sortBySelectedIndex != value)
                {
                    _sortBySelectedIndex = value;
                    OnPropertyChanged();
                    // Save the setting immediately when changed
                    var sortAttrib = (MainListSortDescriptorModel.SortByAttribute)Enum.ToObject(typeof(MainListSortDescriptorModel.SortByAttribute), value);
                    var sortInfo = SettingsHelper.GetSortDescriptorBySelectedItem(sortAttrib);
                    if (sortInfo != null)
                    {
                        SettingsHelper.SaveSortDescriptor(sortInfo.PropertyName, sortInfo.Direction.ToString());
                    }
                }
            }
        }

        public string? SelectedFontFamily
        {
            get
            {
                if (_selectedFontFamily == null)
                {
                    AppSettingsService.GetAppFontFamilyAsync().ContinueWith(task =>
                    {
                        _selectedFontFamily = task.Result;
                        OnPropertyChanged(nameof(SelectedFontFamily));
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
    }
}