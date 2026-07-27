using Pj.Library;
using System.Collections.ObjectModel;
using System.Reflection;
using WikiExtractor.Maui.App.Models;

namespace WikiExtractor.Maui.App.Services
{
    public class ThemeHandler : IThemeHandler
    {
        private const string ResourcePrefix = "WikiApp";
        public async Task<AppThemes?> GetCurrentThemeAsync()
        {
            try
            {
                var data = await SharedServiceCore.LoadDataFile<ThemeSelect>();
                return data?.Theme == null ? null : EnumHelper<AppThemes>.FromString(data.Theme.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current theme: {ex.Message}");
            }
            return null;
        }
        public void LoadDefaultStyle()
        {
            // Avoid .Wait() on the main thread — it deadlocks on iOS when the awaited
            // work tries to marshal back to the main thread (e.g. SecureStorage).
            // Fall back to the default theme synchronously; the saved preference is
            // a nice-to-have that can be applied later.
            var currentTheme = SharedServiceCore.DefaultAppTheme;
            LoadDefaultStyle(currentTheme);

            // Apply the user's saved theme asynchronously after the UI is up
            Task.Run(async () =>
            {
                var saved = await GetCurrentThemeAsync();
                if (saved.HasValue && saved.Value != currentTheme)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => LoadDefaultStyle(saved.Value));
                }
            });

            // Load saved font family asynchronously so it doesn't block the caller
            _ = LoadSavedFontFamilyAsync();
        }
        public void LoadDefaultStyle(AppThemes appTheme)
        {
            if (!MainThread.IsMainThread)
            {
                MainThread.BeginInvokeOnMainThread(() => LoadDefaultStyle(appTheme));
                return;
            }

            try
            {
                if (Application.Current?.Resources == null) return;

                string themeFile = appTheme switch
                {
                    AppThemes.Dark    => "Theme_Dark.xaml",
                    AppThemes.Light   => "Theme_Light.xaml",
                    AppThemes.Forest  => "Theme_Forest.xaml",
                    AppThemes.Candy   => "Theme_Candy.xaml",
                    AppThemes.Sunset  => "Theme_Sunset.xaml",
                    AppThemes.Ocean   => "Theme_Ocean.xaml",
                    _                 => "Theme_Dark.xaml",
                };

                ClearAllResources("WikiApp");
                var commonStyles         = LoadResourceDictionary("Theme_Styles_Common.xaml");
                var commonStylesWiki     = LoadResourceDictionary("Theme_Styles_WikiPages.xaml");
                var commonStylesSettings = LoadResourceDictionary("Theme_Styles_Settings.xaml");
                var commonStylesQuiz     = LoadResourceDictionary("Theme_Styles_Quiz.xaml");
                var themeStyles          = LoadResourceDictionary(themeFile);
                if (commonStyles         != null) Application.Current.Resources.MergedDictionaries.Add(commonStyles);
                if (commonStylesWiki     != null) Application.Current.Resources.MergedDictionaries.Add(commonStylesWiki);
                if (commonStylesSettings != null) Application.Current.Resources.MergedDictionaries.Add(commonStylesSettings);
                if (commonStylesQuiz     != null) Application.Current.Resources.MergedDictionaries.Add(commonStylesQuiz);
                if (themeStyles          != null) Application.Current.Resources.MergedDictionaries.Add(themeStyles);
                // Promote merged keys into the top-level dictionary so DynamicResource bindings
                // re-evaluate. Set each key once — no null bounce needed.
                foreach (var dict in Application.Current.Resources.MergedDictionaries)
                    foreach (var kvp in dict)
                        if (kvp.Key is string k && k.StartsWith("WikiApp"))
                            Application.Current.Resources[k] = kvp.Value;

                // Status bar color — run after resources are applied so the color is available
                if (Application.Current.Resources.TryGetValue("WikiAppDefaultBackgroundColor", out var bgObj) && bgObj is Color bgColor)
                {
                    var isLightBg = (bgColor.Red * 0.299f + bgColor.Green * 0.587f + bgColor.Blue * 0.114f) > 0.5f;
                    SharedServiceCore.AppEnvironment?.SetStatusBarColor(bgColor, isLightBg);
                }

                _ = LoadSavedFontFamilyAsync();
                InitializeQuizColorsBackground();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ThemeHandler] LoadDefaultStyle error: {ex.Message}");
            }
        }
        private TaskCompletionSource<ObservableCollection<Brush>> _chartColorsTcs = new();
        private TaskCompletionSource<QuizThemeData> _themeDataTcs = new();
        public async Task<ObservableCollection<Brush>> GetChartColorsAsync() => await _chartColorsTcs.Task;
        public async Task<QuizThemeData> GetThemeDataAsync() => await _themeDataTcs.Task;
        public void InitializeQuizColorsBackground()
        {
            // 1. If the previous task is already finished, reset it for the new theme
            if (_chartColorsTcs.Task.IsCompleted)
            {
                _chartColorsTcs = new TaskCompletionSource<ObservableCollection<Brush>>();
                _themeDataTcs = new TaskCompletionSource<QuizThemeData>();
            }

            Task.Run(async () =>
            {
                try
                {
                    ObservableCollection<Brush>? colors = null;
                    QuizThemeData? data = null;
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        var appResources = Application.Current?.Resources;
                        var p = ResourcePrefix;
                        if (appResources != null && appResources.ContainsKey($"{ResourcePrefix}QuizCorrectAnswerColor"))
                        {
                            data = new QuizThemeData
                            {
                                CorrectColor = (Color)appResources[$"{p}QuizCorrectAnswerColor"],
                                WrongColor = (Color)appResources[$"{p}QuizWrongAnswerColor"],
                                DefaultBackColor = (Color)appResources[$"{p}QuizAnswerDefaultBackColor"],
                                SelectionBackColor = (Color)appResources[$"{p}QuizAnswerSelectionBackColor"]
                            };
                            colors = [new SolidColorBrush(data.CorrectColor), new SolidColorBrush(data.WrongColor), new SolidColorBrush(data.DefaultBackColor)];
                        }
                    });
                    data ??= new QuizThemeData { CorrectColor = Color.FromArgb("#2ECC71"), WrongColor = Color.FromArgb("#E74C3C"), DefaultBackColor = Color.FromArgb("#F5F5F5"), SelectionBackColor = Color.FromArgb("#D6EAF8") };
                    colors ??= [new SolidColorBrush(data.CorrectColor), new SolidColorBrush(data.WrongColor), new SolidColorBrush(data.DefaultBackColor)];
                    _themeDataTcs.TrySetResult(data);
                    _chartColorsTcs.TrySetResult(colors);
                }
                catch (Exception ex) { _chartColorsTcs.TrySetException(ex); }
            });
        }
        private ResourceDictionary? LoadResourceDictionary(string resourcePath)
        {
            try
            {
                var assembly = typeof(ThemeHandler).Assembly;
                var streamName = $"WikiExtractor.Maui.App.Resources.Styles.{resourcePath}";
                using var stream = assembly.GetManifestResourceStream(streamName);
                if (stream == null) return null;
                using var reader = new StreamReader(stream);
                var xaml = reader.ReadToEnd();
                var dict = new ResourceDictionary();
                dict.LoadFromXaml(xaml);
                return dict;
            }
            catch { return null; }
        }
        private void ClearAllResources(string prefix)
        {
            if (Application.Current?.Resources == null) return;
            // Exclude user preferences that use the WikiApp prefix but are not theme resources
            var excluded = new HashSet<string> { "WikiAppParagraphFontSize" };
            var keysToRemove = Application.Current.Resources.Keys.OfType<string>().Where(k => k.StartsWith(prefix) && !excluded.Contains(k)).ToList();
            foreach (var key in keysToRemove) Application.Current.Resources.Remove(key);
            var dictionariesToRemove = Application.Current.Resources.MergedDictionaries.Where(d => d.Keys.OfType<string>().Any(k => k.StartsWith(prefix))).ToList();
            foreach (var dictionary in dictionariesToRemove) Application.Current.Resources.MergedDictionaries.Remove(dictionary);
        }
        private async Task LoadSavedFontFamilyAsync()
        {
            try
            {
                if (Application.Current?.Resources == null) return;

                var fontFamily = await AppSettingsService.GetAppFontFamilyAsync();
                var fontSize = await AppSettingsService.GetParagraphFontSizeAsync();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (Application.Current?.Resources == null) return;
                    if (!string.IsNullOrEmpty(fontFamily))
                        Application.Current.Resources["DefaultFontFamily"] = fontFamily;
                    Application.Current.Resources["WikiAppParagraphFontSize"] = fontSize;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading saved font family: {ex.Message}");
            }
        }
    }
}