using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Models
{
    /// <summary>
    /// Model for configuring the Loading Facts Control
    /// </summary>
    public class LoadingFactsModel
    {
        /// <summary>
        /// List of quiz facts to display in rotation
        /// </summary>
        public List<QuizFactViewModel> Facts { get; set; } = new();

        /// <summary>
        /// Duration in milliseconds to display each fact
        /// </summary>
        public int FactDisplayDurationMs { get; set; } = 3000;

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
        /// Whether to show rotating facts or just a simple spinner with loading text (lite mode)
        /// </summary>
        public bool ShowFacts { get; set; } = true;

        /// <summary>
        /// Custom loading text to display in lite mode (when ShowFacts is false)
        /// </summary>
        public string LoadingText { get; set; } = "Loading...";

        /// <summary>
        /// Optional master ID to filter facts for a specific entity
        /// </summary>
        public int? MasterId { get; set; }

        /// <summary>
        /// Number of facts to fetch and display
        /// </summary>
        public int FactCount { get; set; } = 5;
    }
}
