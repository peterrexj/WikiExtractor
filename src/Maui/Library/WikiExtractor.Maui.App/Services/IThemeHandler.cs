using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using WikiExtractor.Maui.App.Models;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Interface for handling theme operations in the application.
    /// </summary>
    public interface IThemeHandler
    {
        /// <summary>
        /// Gets the current theme from storage.
        /// </summary>
        /// <returns>The current theme or null if no theme is stored.</returns>
        Task<AppThemes?> GetCurrentThemeAsync();

        /// <summary>
        /// Loads the default style or the previously selected theme.
        /// </summary>
        void LoadDefaultStyle();
        //Task LoadDefaultStyleAsync();

        /// <summary>
        /// Loads a specific theme style.
        /// </summary>
        /// <param name="appTheme">The theme to load.</param>
        void LoadDefaultStyle(AppThemes appTheme);
        //void LoadDefaultStyleAsync(AppThemes appTheme);

        void InitializeQuizColorsBackground();
        //void InitializeQuizColorsBackgroundAsync();

        Task<ObservableCollection<Brush>> GetChartColorsAsync();
        Task<QuizThemeData> GetThemeDataAsync();
    }
}