using System.Collections.ObjectModel;
using System.Reflection;
using Pj.Library;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Implementation of the IThemeHandler interface for handling theme operations.
    /// </summary>
    public class ThemeHandler : IThemeHandler
    {
        private const string ResourcePrefix = "WikiApp";

        /// <summary>
        /// Gets the current theme from storage.
        /// </summary>
        /// <returns>The current theme or null if no theme is stored.</returns>
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

        /// <summary>
        /// Loads the default style or the previously selected theme.
        /// </summary>
        public void LoadDefaultStyle()
        {
            AppThemes? currentTheme = null;
            Task.Run(async () => currentTheme = await GetCurrentThemeAsync()).Wait();
            currentTheme ??= SharedServiceCore.DefaultAppTheme;
            LoadDefaultStyle(currentTheme.Value);
        }

        /// <summary>
        /// Loads a specific theme style.
        /// </summary>
        /// <param name="appTheme">The theme to load.</param>
        public void LoadDefaultStyle(AppThemes appTheme)
        {
            try
            {
                string themeFile;

                switch (appTheme)
                {
                    case AppThemes.Dark:
                        themeFile = "Theme.Dark.xaml";
                        break;
                    case AppThemes.Light:
                        themeFile = "Theme.Light.xaml";
                        break;
                    case AppThemes.Forest:
                        themeFile = "Theme.Forest.xaml";
                        break;
                    //case AppThemes.Warm:
                    //    themeFile = "Theme.Warm.xaml";
                    //    break;
                    default:
                        throw new ArgumentException("Unsupported theme");
                }

                ClearAllResources("WikiApp"); // Clear old WikiApp resources

                var buttonStyles = LoadResourceDictionary("Theme.CommonButtonStyles.xaml");
                var commonStyles = LoadResourceDictionary("Theme.CommonStyles.xaml");
                // var commonDataGridStyles = LoadResourceDictionary("Theme.CommonDataGridStyles.xaml");

                // Add only non-null resource dictionaries
                if (buttonStyles != null)
                {
                    Application.Current?.Resources.MergedDictionaries.Add(buttonStyles);
                }
                
                if (commonStyles != null)
                {
                    Application.Current?.Resources.MergedDictionaries.Add(commonStyles);
                }
                // if (commonDataGridStyles != null)
                // {
                //     Application.Current?.Resources.MergedDictionaries.Add(commonDataGridStyles);
                // }

                // Load the theme-specific styles from WikiExtractor.Maui.App library
                var themeStyles = LoadResourceDictionary(themeFile);
                if (themeStyles != null)
                {
                    Application.Current?.Resources.MergedDictionaries.Add(themeStyles);
                }

                UpdateResources("WikiApp");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown from the style provider {ex}");
            }
        }

        /// <summary>
        /// Gets a collection of brushes for chart colors.
        /// </summary>
        /// <returns>A collection of brushes for chart colors.</returns>
        public ObservableCollection<Brush> GetChartColors()
        {
            var chartColors = new ObservableCollection<Brush>();

            // Add chart colors from resources
            if (Application.Current?.Resources.TryGetValue($"{ResourcePrefix}ChartColor1", out var color1) == true && color1 != null)
                chartColors.Add((Brush)color1);
            if (Application.Current?.Resources.TryGetValue($"{ResourcePrefix}ChartColor2", out var color2) == true && color2 != null)
                chartColors.Add((Brush)color2);
            if (Application.Current?.Resources.TryGetValue($"{ResourcePrefix}ChartColor3", out var color3) == true && color3 != null)
                chartColors.Add((Brush)color3);
            if (Application.Current?.Resources.TryGetValue($"{ResourcePrefix}ChartColor4", out var color4) == true && color4 != null)
                chartColors.Add((Brush)color4);
            if (Application.Current?.Resources.TryGetValue($"{ResourcePrefix}ChartColor5", out var color5) == true && color5 != null)
                chartColors.Add((Brush)color5);

            return chartColors;
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
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an embedded resource as text.
        /// </summary>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="resourcePath">The path to the resource.</param>
        /// <returns>The resource as text.</returns>
        private string GetEmbeddedResourceAsText(Assembly assembly, string resourcePath)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Resource not found: {resourcePath}");
                    return null;
                }

                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading embedded resource: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clears all resources with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix of resources to clear.</param>
        private void ClearAllResources(string prefix)
        {
            if (Application.Current?.Resources == null)
                return;

            var keysToRemove = new List<string>();

            foreach (var key in Application.Current.Resources.Keys)
            {
                if (key is string stringKey && stringKey.StartsWith(prefix))
                {
                    keysToRemove.Add(stringKey);
                }
            }

            foreach (var key in keysToRemove)
            {
                Application.Current.Resources.Remove(key);
            }

            // Remove any existing theme dictionaries
            var dictionariesToRemove = new List<ResourceDictionary>();
            foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (dictionary.Keys.OfType<string>().Any(k => k.StartsWith(prefix)))
                {
                    dictionariesToRemove.Add(dictionary);
                }
            }

            foreach (var dictionary in dictionariesToRemove)
            {
                Application.Current.Resources.MergedDictionaries.Remove(dictionary);
            }
        }

        static void UpdateResources(string prefix)
        {
            try
            {
                var resourceKeys = Application.Current?.Resources.Keys.Cast<string>().Where(k => k.StartsWith(prefix)).ToList() ?? new List<string>();
                var mergedDictionaryKeys = Application.Current?.Resources.MergedDictionaries.SelectMany(f => f.Keys.Cast<string>()).Where(k => k.StartsWith(prefix)).ToList() ?? new List<string>();

                var keys = resourceKeys.Union(mergedDictionaryKeys).ToList();

                foreach (var key in keys)
                {
                    var temp = Application.Current.Resources[key];
                    Application.Current.Resources[key] = null;
                    Application.Current.Resources[key] = temp;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}