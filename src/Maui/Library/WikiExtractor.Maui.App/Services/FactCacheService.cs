using System.Collections.Concurrent;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Service that maintains a pre-loaded cache of quiz facts for instant display.
    /// Keeps a "bucket" of facts always ready to avoid loading delays.
    /// </summary>
    public class FactCacheService
    {
        private static readonly Lazy<FactCacheService> _instance = new(() => new FactCacheService());
        public static FactCacheService Instance => _instance.Value;

        private readonly ConcurrentBag<QuizFactViewModel> _factCache = new();
        private readonly ConcurrentQueue<(int MasterId, string MetadataKey)> _displayedFactKeys = new(); // Track shown fact keys to avoid immediate repeats
        private readonly SemaphoreSlim _refreshLock = new(1, 1);
        private bool _isInitialized = false;
        private CancellationTokenSource? _refreshCts;

        // Configuration
        private const int MinimumCacheSize = 30; // Keep at least this many facts cached (increased for continuous display)
        private const int MaximumCacheSize = 100; // Don't exceed this many cached facts (large buffer for long overlays)
        private const int DefaultFetchCount = 40; // Fetch this many facts per refresh (large batch)
        private const int MaxDisplayedFactsTracking = 50; // Track last N facts to avoid immediate repeats (increased to prevent cycling during long overlays)

        private FactCacheService()
        {
        }

        /// <summary>
        /// Initializes the cache by pre-loading facts in the background.
        /// Non-blocking - returns immediately.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            // Start background refresh - don't await to avoid blocking
            Task.Run(async () => 
            {
                try
                {
                    await RefreshCacheAsync();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex, "FactCacheService.Initialize");
                }
            });
        }

        /// <summary>
        /// Waits for the initial cache to be populated (up to 5 seconds).
        /// Use this on app startup to ensure cache is ready before showing UI.
        /// </summary>
        public async Task WaitForInitializationAsync(int timeoutMs = 5000)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            var startTime = DateTime.Now;
            while (_factCache.Count < MinimumCacheSize &&
                   (DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Synchronous wait for cache initialization. Safe to call from constructors.
        /// </summary>
        public void WaitForInitialization(int timeoutMs = 5000)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            var startTime = DateTime.Now;
            while (_factCache.Count < MinimumCacheSize &&
                   (DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Gets facts from the cache immediately. Returns available facts or empty list.
        /// Never blocks the UI thread - always returns quickly.
        /// Facts are NOT removed from cache - they can be retrieved multiple times.
        /// Triggers aggressive refresh when cache gets low.
        /// </summary>
        /// <param name="count">Number of facts to retrieve</param>
        /// <param name="masterId">Optional master ID filter</param>
        /// <param name="excludeFactKeys">Optional list of fact keys to exclude from results (for current session)</param>
        /// <returns>List of cached facts (may be less than requested if cache is low)</returns>
        public List<QuizFactViewModel> GetFacts(int count, int? masterId = null, List<(int MasterId, string MetadataKey)>? excludeFactKeys = null)
        {
            if (count <= 0) return new List<QuizFactViewModel>();

            var result = new List<QuizFactViewModel>();
            var allFacts = _factCache.ToArray(); // Get snapshot without removing from cache

            System.Diagnostics.Debug.WriteLine($"[FactCache] GetFacts called. Count requested: {count}, MasterId: {masterId}, Total cache: {allFacts.Length}");

            // Get set of recently displayed fact keys for fast lookup (includes session exclusions)
            var displayedKeys = _displayedFactKeys.ToHashSet();
            if (excludeFactKeys != null && excludeFactKeys.Any())
            {
                foreach (var key in excludeFactKeys)
                {
                    displayedKeys.Add(key);
                }
            }
            System.Diagnostics.Debug.WriteLine($"[FactCache] Recently displayed facts: {displayedKeys.Count} (session exclusions: {excludeFactKeys?.Count ?? 0})");

            // Filter facts based on criteria
            var availableFacts = allFacts
                .Where(f => !masterId.HasValue || f.MasterId == masterId.Value)
                .Where(f => !displayedKeys.Contains((f.MasterId, f.MetadataKey))) // Avoid recently shown facts
                .ToList();

            System.Diagnostics.Debug.WriteLine($"[FactCache] Available facts after filtering: {availableFacts.Count}");

            // If we filtered out too many, check if we can cycle through displayed facts
            // This allows rotation even when all facts for a masterId have been shown in current session
            if (availableFacts.Count < count && allFacts.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine($"[FactCache] Not enough available facts ({availableFacts.Count}), looking for additional facts to cycle...");
                
                // Get facts for this masterId that haven't been added yet (allow cycling through session exclusions)
                var additionalFacts = allFacts
                    .Where(f => !masterId.HasValue || f.MasterId == masterId.Value)
                    .Where(f => !availableFacts.Contains(f))
                    .Where(f => !_displayedFactKeys.Contains((f.MasterId, f.MetadataKey))) // Still avoid recently shown from other sessions
                    .Take(count - availableFacts.Count)
                    .ToList();
                
                if (additionalFacts.Any())
                {
                    availableFacts.AddRange(additionalFacts);
                    System.Diagnostics.Debug.WriteLine($"[FactCache] Added {additionalFacts.Count} additional facts (excluding recent history). Total available: {availableFacts.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[FactCache] No additional facts available for masterId={masterId}. This might indicate cache needs refresh.");
                }
            }

            // Take requested count
            result = availableFacts.Take(count).ToList();

            // Track displayed facts to avoid immediate repeats
            foreach (var fact in result)
            {
                var factKey = (fact.MasterId, fact.MetadataKey);
                _displayedFactKeys.Enqueue(factKey);
                
                // Keep tracking list bounded
                while (_displayedFactKeys.Count > MaxDisplayedFactsTracking)
                {
                    _displayedFactKeys.TryDequeue(out _);
                }
            }

            System.Diagnostics.Debug.WriteLine($"[FactCache] Returning {result.Count} facts. Displayed tracking queue: {_displayedFactKeys.Count}");

            // Trigger async refresh if cache is getting low (aggressive threshold)
            if (_factCache.Count < MinimumCacheSize)
            {
                System.Diagnostics.Debug.WriteLine($"[FactCache] Cache low ({_factCache.Count} < {MinimumCacheSize}). Triggering refresh.");
                Task.Run(() => RefreshCacheAsync());
            }

            return result;
        }

        /// <summary>
        /// Gets facts asynchronously - will load from database if cache is empty.
        /// This method is safe to call on UI thread but may take time on first call.
        /// </summary>
        public async Task<List<QuizFactViewModel>> GetFactsAsync(int count, int? masterId = null, List<(int MasterId, string MetadataKey)>? excludeFactKeys = null)
        {
            // Try cache first
            var cachedFacts = GetFacts(count, masterId, excludeFactKeys);
            
            // If we got enough from cache, return immediately
            if (cachedFacts.Count >= count || cachedFacts.Count >= MinimumCacheSize)
            {
                return cachedFacts;
            }

            // Cache is empty or low - load from database on background thread
            try
            {
                var facts = await Task.Run(() => 
                    SharedServices.QuizController.GetQuizFacts(count, masterId));

                // Add loaded facts to result
                if (facts != null && facts.Any())
                {
                    cachedFacts.AddRange(facts.Take(count - cachedFacts.Count));
                    
                    // Put remaining facts into cache for later
                    foreach (var fact in facts.Skip(count - cachedFacts.Count))
                    {
                        if (_factCache.Count < MaximumCacheSize)
                        {
                            _factCache.Add(fact);
                        }
                    }
                }

                // Trigger background refresh to keep cache full
                _ = Task.Run(() => RefreshCacheAsync());
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex, "FactCacheService.GetFactsAsync");
            }

            return cachedFacts;
        }

        /// <summary>
        /// Refreshes the cache by loading new facts in the background.
        /// Clears and reloads cache to get fresh facts.
        /// </summary>
        private async Task RefreshCacheAsync()
        {
            // Prevent multiple simultaneous refreshes
            if (!await _refreshLock.WaitAsync(0))
                return;

            try
            {
                // Only refresh if below minimum or cache is empty
                if (_factCache.Count >= MinimumCacheSize)
                    return;

                // Calculate how many facts to fetch
                int factsNeeded = MaximumCacheSize - _factCache.Count;
                if (factsNeeded <= 0) return;

                int factsToFetch = Math.Min(factsNeeded, DefaultFetchCount);

                // Fetch facts on background thread
                var newFacts = await Task.Run(() =>
                    SharedServices.QuizController.GetQuizFacts(factsToFetch, masterId: null));

                if (newFacts != null && newFacts.Any())
                {
                    // Add new facts to cache (avoiding duplicates based on MasterId + MetadataKey combination)
                    var existingKeys = _factCache.Select(f => (f.MasterId, f.MetadataKey)).ToHashSet();
                    
                    foreach (var fact in newFacts)
                    {
                        // Don't exceed maximum cache size
                        if (_factCache.Count >= MaximumCacheSize)
                            break;

                        // Avoid duplicates (same MasterId + MetadataKey combination)
                        var factKey = (fact.MasterId, fact.MetadataKey);
                        if (!existingKeys.Contains(factKey))
                        {
                            _factCache.Add(fact);
                            existingKeys.Add(factKey);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex, "FactCacheService.RefreshCacheAsync");
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        /// <summary>
        /// Marks facts as shown and triggers cache refresh.
        /// Immediately updates internal tracking to prevent returning same facts before DB update completes.
        /// Does NOT remove from cache to allow facts to remain available for other masters.
        /// </summary>
        public void MarkFactsAsShown(List<QuizFactViewModel> facts)
        {
            if (facts == null || !facts.Any()) return;

            System.Diagnostics.Debug.WriteLine($"[FactCache] MarkFactsAsShown called with {facts.Count} facts");

            // IMMEDIATELY update displayed keys tracking (before async DB update)
            // This prevents returning the same facts on next request before DB update completes
            foreach (var fact in facts)
            {
                var factKey = (fact.MasterId, fact.MetadataKey);
                
                // Add to displayed keys tracking (will be excluded from future GetFacts calls)
                if (!_displayedFactKeys.Contains(factKey))
                {
                    _displayedFactKeys.Enqueue(factKey);
                    System.Diagnostics.Debug.WriteLine($"[FactCache] Added to displayed keys: MasterId={fact.MasterId}, Key={fact.MetadataKey}");
                }
                
                // Keep tracking list bounded
                while (_displayedFactKeys.Count > MaxDisplayedFactsTracking)
                {
                    _displayedFactKeys.TryDequeue(out _);
                }
            }

            System.Diagnostics.Debug.WriteLine($"[FactCache] Updated displayed keys. Total tracked: {_displayedFactKeys.Count}");

            // Mark in database and refresh cache in background
            Task.Run(() =>
            {
                foreach (var fact in facts)
                {
                    try
                    {
                        SharedServices.QuizController.MarkFactAsShown(
                            fact.MasterId, 
                            fact.MetadataKey);
                        System.Diagnostics.Debug.WriteLine($"[FactCache] Marked in DB: MasterId={fact.MasterId}, Key={fact.MetadataKey}");
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.CaptureException(ex, "FactCacheService.MarkFactsAsShown");
                    }
                }

                // Refresh cache after marking facts
                RefreshCacheAsync().Wait();
            });
        }

        /// <summary>
        /// Clears the cache (useful for testing or reset scenarios)
        /// </summary>
        public void ClearCache()
        {
            while (_factCache.TryTake(out _)) { }
            while (_displayedFactKeys.TryDequeue(out _)) { }
        }

        /// <summary>
        /// Pre-loads facts for a specific master ID
        /// </summary>
        public void PreloadFactsForMaster(int masterId, int count = 5)
        {
            Task.Run(async () =>
            {
                try
                {
                    var facts = await Task.Run(() => 
                        SharedServices.QuizController.GetQuizFacts(count, masterId));

                    if (facts != null && facts.Any())
                    {
                        foreach (var fact in facts)
                        {
                            if (_factCache.Count < MaximumCacheSize)
                            {
                                _factCache.Add(fact);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.CaptureException(ex, "FactCacheService.PreloadFactsForMaster", 
                        $"MasterId: {masterId}");
                }
            });
        }

        /// <summary>
        /// Gets the current cache size (for monitoring/debugging)
        /// </summary>
        public int CacheSize => _factCache.Count;

        /// <summary>
        /// Forces an immediate cache refresh
        /// </summary>
        public Task ForceRefreshAsync() => RefreshCacheAsync();
    }
}
