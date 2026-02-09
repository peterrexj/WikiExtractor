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
        private Timer? _factRotationTimer;
        private int _currentFactIndex = 0;
        private bool _isDisposed = false;
        private readonly List<QuizFactViewModel> _displayedFacts = new(); // Track facts actually shown to user
        private readonly object _displayedFactsLock = new(); // Thread safety for displayed facts list

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

        private List<QuizFactViewModel> _facts = new();
        public List<QuizFactViewModel> Facts
        {
            get => _facts;
            set
            {
                _facts = value;
                OnPropertyChanged();
            }
        }

        private QuizFactViewModel? _currentFact;
        public QuizFactViewModel? CurrentFact
        {
            get => _currentFact;
            set
            {
                var oldFact = _currentFact;
                _currentFact = value;
                
                // Track newly displayed fact (thread-safe)
                if (value != null && value != oldFact)
                {
                    lock (_displayedFactsLock)
                    {
                        if (!_displayedFacts.Contains(value))
                        {
                            _displayedFacts.Add(value);
                            System.Diagnostics.Debug.WriteLine($"[FactTracking] Added fact to displayed list. Total displayed: {_displayedFacts.Count}");
                        }
                    }
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
        /// Initializes the facts from the model and starts the rotation timer
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

            // Get first fact immediately from cache (instant if pre-loaded)
            // Pass empty exclusion list for first fact
            var sessionExclusions = GetSessionExclusionList();
            var initialFacts = FactCacheService.Instance.GetFacts(1, Model.MasterId, sessionExclusions);
            
            if (initialFacts != null && initialFacts.Any())
            {
                // Cache has facts - display immediately
                CurrentFact = initialFacts[0];
            }
            else
            {
                // Cache not ready yet - load synchronously to avoid delay
                // This should be fast since cache is pre-populated on app start
                Task.Run(async () =>
                {
                    try
                    {
                        var sessionExclusions = GetSessionExclusionList();
                        var facts = await FactCacheService.Instance.GetFactsAsync(1, Model.MasterId, sessionExclusions);
                        
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            if (facts != null && facts.Any())
                            {
                                CurrentFact = facts[0];
                            }
                            else
                            {
                                // Fallback: show a generic message only if no facts available
                                CurrentFact = new QuizFactViewModel
                                {
                                    FactText = "Did you know? This app contains fascinating information!",
                                    MasterName = "",
                                    MasterImagePath = "",
                                    MasterId = Model.MasterId ?? 0
                                };
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.InitializeFacts");
                        
                        // Show fallback on error
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            CurrentFact = new QuizFactViewModel
                            {
                                FactText = "Did you know? This app contains fascinating information!",
                                MasterName = "",
                                MasterImagePath = "",
                                MasterId = Model.MasterId ?? 0
                            };
                        });
                    }
                });
            }

            // Start rotation timer - will continuously pull from cache
            StartFactRotation();
        }

        /// <summary>
        /// Starts the timer that rotates through facts
        /// </summary>
        private void StartFactRotation()
        {
            if (Model == null) return;

            _factRotationTimer?.Dispose();
            _factRotationTimer = new Timer(Model.FactDisplayDurationMs);
            _factRotationTimer.Elapsed += OnFactRotationTimerElapsed;
            _factRotationTimer.AutoReset = true;
            _factRotationTimer.Start();
            
            System.Diagnostics.Debug.WriteLine($"[FactRotation] Timer started with interval: {Model.FactDisplayDurationMs}ms");
        }

        /// <summary>
        /// Timer callback to rotate to the next fact - continuously loads new facts from cache
        /// </summary>
        private void OnFactRotationTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (Model == null) return;

                // Get next fact from cache (instant if cache is populated)
                // Exclude facts already displayed in this session
                var sessionExclusions = GetSessionExclusionList();
                var nextFacts = FactCacheService.Instance.GetFacts(1, Model.MasterId, sessionExclusions);
                
                System.Diagnostics.Debug.WriteLine($"[FactRotation] Timer elapsed. Retrieved {nextFacts?.Count ?? 0} facts. Cache size: {FactCacheService.Instance.CacheSize}");
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (nextFacts != null && nextFacts.Any())
                        {
                            // Update to new fact
                            var newFact = nextFacts[0];
                            var factTextLength = newFact.FactText?.Length ?? 0;
                            var previewLength = Math.Min(50, factTextLength);
                            var preview = factTextLength > 0 ? newFact.FactText?.Substring(0, previewLength) : "";
                            System.Diagnostics.Debug.WriteLine($"[FactRotation] Updating to new fact: {preview}...");
                            CurrentFact = newFact;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[FactRotation] No new facts available. Keeping current fact.");
                        }
                        // If no new facts available, keep showing current fact (don't show loading message)
                        // The cache will refresh in background and new facts will appear on next timer tick
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.OnFactRotationTimerElapsed.MainThread");
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.OnFactRotationTimerElapsed");
            }
        }

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
        /// Stops the fact rotation and hides the control
        /// </summary>
        public void Hide()
        {
            try
            {
                StopFactRotation();
                IsVisible = false;

                // Mark only the facts that were actually displayed in the UI (thread-safe)
                List<QuizFactViewModel>? factsToMark = null;
                
                if (Model?.AutoMarkFactsAsShown == true)
                {
                    lock (_displayedFactsLock)
                    {
                        if (_displayedFacts.Any())
                        {
                            factsToMark = new List<QuizFactViewModel>(_displayedFacts);
                            _displayedFacts.Clear(); // Clear immediately while we have the lock
                            System.Diagnostics.Debug.WriteLine($"[FactTracking] Queued {factsToMark.Count} displayed facts to mark as shown");
                        }
                    }
                }
                
                // Mark facts in background without blocking UI (using FactCacheService)
                if (factsToMark != null && factsToMark.Any())
                {
                    FactCacheService.Instance.MarkFactsAsShown(factsToMark);
                }
                
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

        /// <summary>
        /// Stops the fact rotation timer
        /// </summary>
        private void StopFactRotation()
        {
            try
            {
                if (_factRotationTimer != null)
                {
                    _factRotationTimer.Stop();
                    _factRotationTimer.Elapsed -= OnFactRotationTimerElapsed;
                    _factRotationTimer.Dispose();
                    _factRotationTimer = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CatchException(ex, "LoadingFactsControlViewModel.StopFactRotation");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the list of fact keys already displayed in this loading session
        /// </summary>
        private List<(int MasterId, string MetadataKey)> GetSessionExclusionList()
        {
            lock (_displayedFactsLock)
            {
                return _displayedFacts
                    .Select(f => (f.MasterId, f.MetadataKey))
                    .ToList();
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                StopFactRotation();
                _isDisposed = true;
            }
        }
    }
}
