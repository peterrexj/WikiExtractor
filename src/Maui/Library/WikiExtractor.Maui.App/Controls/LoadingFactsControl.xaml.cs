using WikiExtractor.Maui.App.ViewModels;
using WikiExtractor.Maui.App.Models;

namespace WikiExtractor.Maui.App.Controls
{
    /// <summary>
    /// A reusable control that displays rotating quiz facts with a loading spinner.
    /// Can be used in any page during data loading operations.
    /// </summary>
    public partial class LoadingFactsControl : ContentView
    {
        public LoadingFactsControlViewModel ViewModel { get; private set; }

        public LoadingFactsControl()
        {
            InitializeComponent();
            ViewModel = new LoadingFactsControlViewModel();
            BindingContext = ViewModel;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
            // Ensure ViewModel is always our custom type
            if (BindingContext is LoadingFactsControlViewModel vm)
            {
                ViewModel = vm;
            }
        }

        /// <summary>
        /// Shows the loading facts control with the specified configuration
        /// </summary>
        /// <param name="model">Configuration model for the loading facts display</param>
        public void Show(LoadingFactsModel model)
        {
            ViewModel?.Show(model);
        }

        /// <summary>
        /// Hides the loading facts control and stops the fact rotation
        /// </summary>
        public void Hide()
        {
            ViewModel?.Hide();
        }

        /// <summary>
        /// Call this method when page loading is complete to hide the control
        /// </summary>
        public void NotifyLoadComplete()
        {
            ViewModel?.Hide();
        }
    }
}