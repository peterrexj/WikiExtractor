using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using WikiExtractor.Exts;

namespace WikiExtractor.Maui.App.Services
{
    public sealed class ImagePipeline
    {
        public static readonly ImagePipeline Instance = new();

        private const int MemoryCacheCapacity = 80;
        private readonly Dictionary<string, (ImageSource src, LinkedListNode<string> node)> _memCache = new();
        private readonly LinkedList<string> _lruOrder = new();
        private readonly object _memLock = new();

        private readonly ConcurrentDictionary<string, Task<ImageSource>> _inFlight = new();

        private static readonly HttpClient _http = new()
        {
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestHeaders = { { "User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1" } }
        };

        private ImagePipeline() { }

        private static readonly ImageSource _placeholder = ImageSource.FromFile("no_image_available.png");
        public ImageSource Placeholder => _placeholder;

        public Task<ImageSource> GetAsync(string url, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(url)) return Task.FromResult(Placeholder);

            lock (_memLock)
            {
                if (_memCache.TryGetValue(url, out var entry))
                {
                    _lruOrder.Remove(entry.node);
                    _lruOrder.AddFirst(entry.node);
                    return Task.FromResult(entry.src);
                }
            }

            return _inFlight.GetOrAdd(url, _ => FetchAsync(url, ct))
                            .ContinueWith(t =>
                            {
                                _inFlight.TryRemove(url, out _);
                                return t.Status == TaskStatus.RanToCompletion ? t.Result : Placeholder;
                            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        // Backoff delays for successive 429 responses (ms): 3s, 8s, 20s, 45s
        private static readonly int[] _rateLimitBackoffMs = { 3000, 8000, 20000, 45000 };

        private async Task<ImageSource> FetchAsync(string url, CancellationToken ct)
        {
            try
            {
                var diskPath = DiskPath(url);

                if (File.Exists(diskPath))
                {
                    var src = ImageSource.FromFile(diskPath);
                    AddToMemory(url, src);
                    return src;
                }

                byte[] bytes = null;
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    HttpResponseMessage response;
                    try
                    {
                        response = await _http.GetAsync(url, HttpCompletionOption.ResponseContentRead, ct);
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        // Network error — one silent retry after 2s, then give up
                        if (attempt == 0) { await Task.Delay(2000, ct); continue; }
                        Debug.WriteLine($"[ImagePipeline] Network error: {ex.Message}  {url}");
                        break;
                    }

                    using (response)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            bytes = await response.Content.ReadAsByteArrayAsync(ct);
                            break;
                        }

                        if ((int)response.StatusCode == 429)
                        {
                            if (attempt >= _rateLimitBackoffMs.Length)
                            {
                                Debug.WriteLine($"[ImagePipeline] 429 — exhausted retries: {url}");
                                break;
                            }

                            // Honour Retry-After if Wikipedia provides it, else use our schedule
                            int waitMs = _rateLimitBackoffMs[attempt];
                            if (response.Headers.RetryAfter?.Delta is TimeSpan delta)
                                waitMs = (int)Math.Clamp(delta.TotalMilliseconds, 1000, 60_000);

                            Debug.WriteLine($"[ImagePipeline] 429 attempt {attempt + 1}/5 — waiting {waitMs}ms: {url}");
                            await Task.Delay(waitMs, ct);
                            continue;
                        }

                        // Other HTTP error (404, 403, etc.) — no point retrying
                        Debug.WriteLine($"[ImagePipeline] HTTP {(int)response.StatusCode}: {url}");
                        break;
                    }
                }

                if (bytes == null) return Placeholder;

                await File.WriteAllBytesAsync(diskPath, bytes, ct);
                var result = ImageSource.FromFile(diskPath);
                AddToMemory(url, result);
                return result;
            }
            catch (OperationCanceledException) { return Placeholder; }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ImagePipeline] {url}: {ex.Message}");
                return Placeholder;
            }
        }

        private void AddToMemory(string url, ImageSource src)
        {
            lock (_memLock)
            {
                if (_memCache.ContainsKey(url)) return;
                if (_memCache.Count >= MemoryCacheCapacity)
                {
                    var evict = _lruOrder.Last.Value;
                    _lruOrder.RemoveLast();
                    _memCache.Remove(evict);
                }
                var node = _lruOrder.AddFirst(url);
                _memCache[url] = (src, node);
            }
        }

        public static string DiskPath(string url)
        {
            try
            {
                var fileName = Path.GetFileName(new Uri(url).LocalPath);
                if (string.IsNullOrEmpty(fileName))
                    fileName = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(url))) + ".jpg";
                return Path.Combine(ConfigData.LocalStorageCacheFolderPath, fileName);
            }
            catch
            {
                return Path.Combine(ConfigData.LocalStorageCacheFolderPath,
                    Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(url))) + ".jpg");
            }
        }

        public void Invalidate(string url)
        {
            lock (_memLock)
            {
                if (_memCache.TryGetValue(url, out var entry))
                {
                    _lruOrder.Remove(entry.node);
                    _memCache.Remove(url);
                }
            }
            var p = DiskPath(url);
            if (File.Exists(p)) File.Delete(p);
        }
    }
}
