using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using Maui.Wiki.Services;
using WikiExtractor.Maui.App.Models;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.ViewModels;
using Timer = System.Timers.Timer;

namespace WikiExtractor.Maui.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Loading Facts Control
    /// Manages fact rotation, timing, and display logic
    /// </summary>
    public class LoadingFactsControlViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _isDisposed = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        private LoadingFactsModel? _model;
        public LoadingFactsModel? Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
                if (value != null)
                {
                    InitializeFacts();
                }
            }
        }

        // Removed Facts collection - no longer needed since we show only one fact

        private QuizFactViewModel? _currentFact;
        public QuizFactViewModel? CurrentFact
        {
            get => _currentFact;
            set
            {
                _currentFact = value;
                
                // Mark fact as shown immediately when displayed
                if (value != null && Model?.AutoMarkFactsAsShown == true)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            SharedServices.QuizController.MarkFactAsShown(value.MasterId, value.MetadataKey);
                            System.Diagnostics.Debug.WriteLine($"[Facts] Marked as shown immediately: MasterId={value.MasterId}, Key={value.MetadataKey}");
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.CurrentFact.MarkAsShown");
                        }
                    });
                }
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCurrentFact));
                OnPropertyChanged(nameof(CurrentFactText));
                OnPropertyChanged(nameof(CurrentMasterImage));
                OnPropertyChanged(nameof(ShowImage));
            }
        }

        public bool HasCurrentFact => CurrentFact != null;
        public string CurrentFactText => CurrentFact?.FactText ?? Model?.LoadingText ?? "Loading interesting facts...";
        public string CurrentMasterImage => CurrentFact?.MasterImagePath ?? "NoImageAvailable.png";
        public bool ShowImage => (Model?.ShowMasterImage ?? true) && HasCurrentFact;

        private string _loadingText = "Loading...";
        public string LoadingText
        {
            get => _loadingText;
            set
            {
                _loadingText = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public LoadingFactsControlViewModel()
        {
        }

        /// <summary>
        /// Loads a single fact to display (no rotation)
        /// </summary>
        private void InitializeFacts()
        {
            if (Model == null) return;

            // Lite mode: Just show loading text without facts
            if (!Model.ShowFacts)
            {
                LoadingText = Model.LoadingText ?? "Loading...";
                CurrentFact = null;
                return;
            }

            // Load one fact in background to avoid blocking UI
            Task.Run(async () =>
            {
                try
                {
                    // Get one random unshown fact (any master, no filtering)
                    var facts = await Task.Run(() => SharedServices.QuizController.GetQuizFacts(1, masterId: null));
                    
                    if (facts != null && facts.Any())
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            CurrentFact = facts[0];
                            System.Diagnostics.Debug.WriteLine($"[Facts] Loaded fact: {facts[0].FactText?.Substring(0, Math.Min(50, facts[0].FactText.Length))}...");
                        });
                    }
                    else
                    {
                        // No facts available - reset all and try again
                        System.Diagnostics.Debug.WriteLine($"[Facts] No unshown facts available. Resetting all facts...");
                        SharedServices.QuizController.ResetShownFacts();
                        
                        // Try again after reset
                        facts = await Task.Run(() => SharedServices.QuizController.GetQuizFacts(1, masterId: null));
                        
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            if (facts != null && facts.Any())
                            {
                                CurrentFact = facts[0];
                                System.Diagnostics.Debug.WriteLine($"[Facts] Loaded fact after reset: {facts[0].FactText?.Substring(0, Math.Min(50, facts[0].FactText.Length))}...");
                            }
                            else
                            {
                                // Fallback if still no facts
                                CurrentFact = new QuizFactViewModel
                                {
                                    FactText = "Did you know? This app contains fascinating information!",
                                    MasterName = "",
                                    MasterImagePath = "",
                                    MasterId = 0
                                };
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.InitializeFacts");
                    
                    // Show fallback on error
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CurrentFact = new QuizFactViewModel
                        {
                            FactText = "Did you know? This app contains fascinating information!",
                            MasterName = "",
                            MasterImagePath = "",
                            MasterId = 0
                        };
                    });
                }
            });
        }

        // Rotation removed - no longer needed

        /// <summary>
        /// Shows the loading facts control with the specified configuration
        /// </summary>
        /// <param name="model">Configuration model for loading facts</param>
        public void Show(LoadingFactsModel model)
        {
            try
            {
                IsVisible = true;
                Model = model;
            }
            catch (Exception ex)
            {
                ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.Show");
            }
        }

        /// <summary>
        /// Hides the control
        /// </summary>
        public void Hide()
        {
            try
            {
                IsVisible = false;
                
                // Invoke completion callback on main thread
                if (Model?.OnLoadComplete != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            Model?.OnLoadComplete?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.Hide.OnLoadComplete");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.Hide");
            }
        }

        // StopFactRotation removed - no longer needed

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
            }
        }
    }
}
