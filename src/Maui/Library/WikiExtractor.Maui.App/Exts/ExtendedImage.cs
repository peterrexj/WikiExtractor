using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using WikiExtractor.Exts;

namespace WikiExtractor.Maui.App.Exts
{
    public class ExtendedImage : Image
    {
        // Shared across all instances — avoids socket exhaustion from per-download HttpClient
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        // Tracks the in-flight load so a rapid binding update cancels the previous one
        private CancellationTokenSource _loadCts;

        #region LocalFileName — filename only, resolved to the app's local storage folder

        public static readonly BindableProperty LocalFileNameProperty =
            BindableProperty.Create(
                nameof(LocalFileName),
                typeof(string),
                typeof(ExtendedImage),
                default(string));

        public string LocalFileName
        {
            get => (string)GetValue(LocalFileNameProperty);
            set => SetValue(LocalFileNameProperty, value);
        }

        // Resolves LocalFileName to a full path using the same folder as the rest of the app.
        // Returns null when LocalFileName is not set.
        private string ResolveLocalPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            return Path.Combine(ConfigData.LocalStorageCacheFolderPath, fileName);
        }

        #endregion

        #region CustomSource — accepts a local file path string or a URL string

        public static readonly BindableProperty CustomSourceProperty =
            BindableProperty.Create(
                nameof(CustomSource),
                typeof(string),
                typeof(ExtendedImage),
                default(string),
                propertyChanged: OnCustomSourcePropertyChanged);

        private static void OnCustomSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ExtendedImage)bindable;
            var path = newValue as string;
            control.ApplySource(path);
        }

        public string CustomSource
        {
            get => (string)GetValue(CustomSourceProperty);
            set => SetValue(CustomSourceProperty, value);
        }

        #endregion

        #region PageCancellationTokenSource — linked when the host page navigates away

        public static readonly BindableProperty PageCancellationTokenSourceProperty =
            BindableProperty.Create(
                nameof(PageCancellationTokenSource),
                typeof(CancellationTokenSource),
                typeof(ExtendedImage),
                default(CancellationTokenSource));

        public CancellationTokenSource PageCancellationTokenSource
        {
            get => (CancellationTokenSource)GetValue(PageCancellationTokenSourceProperty);
            set => SetValue(PageCancellationTokenSourceProperty, value);
        }

        #endregion

        #region ImageWidth / ImageHeight — metadata for external callers, not used internally

        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(ExtendedImage), default(int));

        public int ImageWidth
        {
            get => (int)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(ExtendedImage), default(int));

        public int ImageHeight
        {
            get => (int)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        #endregion

        // Entry point for both the bindable property change and the post-download refresh.
        // Cancels any in-flight load before starting a new one.
        private void ApplySource(string path)
        {
            // Cancel previous load (rapid binding updates, question change in quiz)
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = new CancellationTokenSource();
            var token = _loadCts.Token;

            _ = LoadAsync(path, token);
        }

        // Called from PersonaDetailPage after a background download completes to refresh the image.
        public void RefreshFromLocalFile()
        {
            var localPath = ResolveLocalPath(LocalFileName);
            if (localPath != null)
                ApplySource(localPath);
        }

        private async Task LoadAsync(string path, CancellationToken token)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    SetSourceOnMainThread(ImageSource.FromFile("no_image_available.png"), token);
                    return;
                }

                // Local file — check disk first, then show
                if (!path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    SetSourceOnMainThread(
                        File.Exists(path)
                            ? ImageSource.FromFile(path)
                            : ImageSource.FromFile("no_image_available.png"),
                        token);
                    return;
                }

                // Remote URL — resolve to local cache path via LocalFileName
                var localPath = ResolveLocalPath(LocalFileName);
                if (string.IsNullOrEmpty(localPath))
                {
                    // Derive filename from URL when LocalFileName is not bound
                    var derived = Path.GetFileName(new Uri(path).LocalPath);
                    if (!string.IsNullOrEmpty(derived))
                        localPath = Path.Combine(ConfigData.LocalStorageCacheFolderPath, derived);
                }

                if (string.IsNullOrEmpty(localPath))
                {
                    SetSourceOnMainThread(ImageSource.FromFile("no_image_available.png"), token);
                    return;
                }

                // Serve from cache when available
                if (File.Exists(localPath))
                {
                    SetSourceOnMainThread(ImageSource.FromFile(localPath), token);
                    return;
                }

                // Download — respect both the per-load token and the page-level token
                using var linked = PageCancellationTokenSource != null
                    ? CancellationTokenSource.CreateLinkedTokenSource(token, PageCancellationTokenSource.Token)
                    : CancellationTokenSource.CreateLinkedTokenSource(token);

                byte[] imageBytes = null;
                const int maxAttempts = 2;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        imageBytes = await _httpClient.GetByteArrayAsync(path, linked.Token);
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception) when (attempt < maxAttempts)
                    {
                        await Task.Delay(1000, linked.Token);
                    }
                }

                if (imageBytes == null || linked.Token.IsCancellationRequested) return;

                await File.WriteAllBytesAsync(localPath, imageBytes, linked.Token);
                if (linked.Token.IsCancellationRequested) return;

                SetSourceOnMainThread(ImageSource.FromFile(localPath), token);
            }
            catch (OperationCanceledException)
            {
                // Navigation away or rapid update — silently discard
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExtendedImage] LoadAsync failed for '{path}': {ex.Message}");
                SetSourceOnMainThread(ImageSource.FromFile("no_image_available.png"), token);
            }
        }

        private void SetSourceOnMainThread(ImageSource source, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                if (!token.IsCancellationRequested)
                    base.Source = source;
            });
        }
    }
}
