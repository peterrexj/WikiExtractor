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
            AppThemes? currentTheme = null;
            Task.Run(async () => currentTheme = await GetCurrentThemeAsync()).Wait();
            currentTheme ??= SharedServiceCore.DefaultAppTheme;
            LoadDefaultStyle(currentTheme.Value);
        }
        public void LoadDefaultStyle(AppThemes appTheme)
        {
            try
            {
                string themeFile;
                switch (appTheme)
                {
                    case AppThemes.Dark: themeFile = "Theme.Dark.xaml"; break;
                    case AppThemes.Light: themeFile = "Theme.Light.xaml"; break;
                    case AppThemes.Forest: themeFile = "Theme.Forest.xaml"; break;
                    default: throw new ArgumentException("Unsupported theme");
                }
                ClearAllResources("WikiApp");
                var buttonStyles = LoadResourceDictionary("Theme.CommonButtonStyles.xaml");
                var commonStyles = LoadResourceDictionary("Theme.Styles.Common.xaml");
                var commonStylesQuiz = LoadResourceDictionary("Theme.Styles.Quiz.xaml");
                if (buttonStyles != null) Application.Current?.Resources.MergedDictionaries.Add(buttonStyles);
                if (commonStyles != null) Application.Current?.Resources.MergedDictionaries.Add(commonStyles);
                if (commonStylesQuiz != null) Application.Current?.Resources.MergedDictionaries.Add(commonStylesQuiz);
                var themeStyles = LoadResourceDictionary(themeFile);
                if (themeStyles != null) Application.Current?.Resources.MergedDictionaries.Add(themeStyles);
                UpdateResources("WikiApp");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown from the style provider {ex}");
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
                var xaml = PjUtility.Runtime.GetAssembly("WikiExtractor.Maui.App").GetEmbeddedResourceAsText($"WikiExtractor.Maui.App.Resources.Styles.{resourcePath}");
                var resourceDictionary = new ResourceDictionary();
                resourceDictionary.LoadFromXaml(xaml);
                return resourceDictionary;
            }
            catch { return null; }
        }
        private string GetEmbeddedResourceAsText(Assembly assembly, string resourcePath)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream == null) return null;
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch { return null; }
        }
        private void ClearAllResources(string prefix)
        {
            if (Application.Current?.Resources == null) return;
            var keysToRemove = Application.Current.Resources.Keys.OfType<string>().Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove) Application.Current.Resources.Remove(key);
            var dictionariesToRemove = Application.Current.Resources.MergedDictionaries.Where(d => d.Keys.OfType<string>().Any(k => k.StartsWith(prefix))).ToList();
            foreach (var dictionary in dictionariesToRemove) Application.Current.Resources.MergedDictionaries.Remove(dictionary);
        }
        static void UpdateResources(string prefix)
        {
            try
            {
                var resKeys = Application.Current?.Resources.Keys.Cast<string>().Where(k => k.StartsWith(prefix)) ?? Enumerable.Empty<string>();
                var mergedKeys = Application.Current?.Resources.MergedDictionaries.SelectMany(f => f.Keys.Cast<string>()).Where(k => k.StartsWith(prefix)) ?? Enumerable.Empty<string>();
                var keys = resKeys.Union(mergedKeys).ToList();
                foreach (var key in keys)
                {
                    var temp = Application.Current.Resources[key];
                    Application.Current.Resources[key] = null;
                    Application.Current.Resources[key] = temp;
                }
            }
            catch { }
        }
    }
}