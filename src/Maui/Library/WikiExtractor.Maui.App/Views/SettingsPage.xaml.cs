using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.ViewModels;
using Syncfusion.Maui.Buttons;
using System;
using System.Threading;
using System.Threading.Tasks;
using Pj.Library;

namespace WikiExtractor.Maui.App.Views
{
    public partial class SettingsPage : ContentPage
    {
        private CancellationTokenSource _pitchDebounce;
        private CancellationTokenSource _fontSizeDebounce;

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (BindingContext is SettingsViewModel vm)
                {
                    await vm.LoadAsync();
                    await vm.LoadVoicesAsync();
                }
                ApplySwitchThemeColors();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private void ApplySwitchThemeColors()
        {
            try
            {
                var settings = hideViewedSwitch.SwitchSettings;
                settings.SetDynamicResource(Syncfusion.Maui.Buttons.SwitchSettings.TrackBackgroundProperty, "WikiAppSwitchTrackColorOn");
                settings.SetDynamicResource(Syncfusion.Maui.Buttons.SwitchSettings.ThumbBackgroundProperty, "WikiAppSwitchThumbColorOn");
            }
            catch { }
        }

        public SettingsPage()
        {
            InitializeComponent();

            try
            {
                var themeHandler = GetThemeHandlerService();
                var errorHandlingService = GetErrorHandlingService();
                BindingContext = new SettingsViewModel(themeHandler, errorHandlingService);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Failed to initialize SettingsPage with services: {ex.Message}");
                BindingContext = new SettingsViewModel(null, null);
            }

            try
            {
                var name = AppInfo.Current.Name;
                var version = AppInfo.Current.VersionString;
                lblVersion.Text = $"{name} v{version}";
            }
            catch
            {
                lblVersion.Text = string.Empty;
            }
        }

        private IThemeHandler GetThemeHandlerService()
        {
            try
            {
                var customService = SharedServiceCore.ThemeHandler;
                if (customService != null) return customService;
                return new ThemeHandler();
            }
            catch
            {
                return new ThemeHandler();
            }
        }

        private IErrorHandlingService GetErrorHandlingService()
        {
            try { return ServiceLocator.GetService<IErrorHandlingService>(); }
            catch { return null; }
        }

        private async void HideViewedSwitch_StateChanged(object sender, SwitchStateChangedEventArgs e)
        {
            await Task.CompletedTask;
        }

        private async void SfSegmentSortOrder_SelectionChanged(object sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
        {
            await Task.CompletedTask;
        }

        private void SliderSpeechPitch_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _pitchDebounce?.Cancel();
            _pitchDebounce?.Dispose();
            _pitchDebounce = new CancellationTokenSource();
            var token = _pitchDebounce.Token;
            Task.Delay(300, token).ContinueWith(t =>
            {
                try
                {
                    if (!t.IsCanceled && BindingContext is SettingsViewModel vm)
                        vm.SpeechPitch = (float)e.NewValue;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SliderFontSize_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _fontSizeDebounce?.Cancel();
            _fontSizeDebounce?.Dispose();
            _fontSizeDebounce = new CancellationTokenSource();
            var token = _fontSizeDebounce.Token;
            Task.Delay(200, token).ContinueWith(t =>
            {
                try
                {
                    if (!t.IsCanceled && BindingContext is SettingsViewModel vm)
                        vm.ParagraphFontSize = e.NewValue;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
