using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Models
{
    /// <summary>
    /// Model for configuring the Loading Facts Control (Simplified - one fact per overlay)
    /// </summary>
    public class LoadingFactsModel
    {
        /// <summary>
        /// Whether to show the master's image in a circle
        /// </summary>
        public bool ShowMasterImage { get; set; } = true;

        /// <summary>
        /// Optional callback when page load is complete and control should be hidden
        /// </summary>
        public Action? OnLoadComplete { get; set; }

        /// <summary>
        /// Whether to automatically mark facts as shown when displayed
        /// </summary>
        public bool AutoMarkFactsAsShown { get; set; } = true;

        /// <summary>
        /// Whether to show facts or just a simple spinner with loading text (lite mode)
        /// </summary>
        public bool ShowFacts { get; set; } = true;

        /// <summary>
        /// Custom loading text to display in lite mode (when ShowFacts is false)
        /// </summary>
        public string LoadingText { get; set; } = "Loading...";
    }
}
