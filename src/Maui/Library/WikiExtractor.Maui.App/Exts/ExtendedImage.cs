using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Exts
{
    public class ExtendedImage : Image
    {
        private CancellationTokenSource _loadCts;
        private int _generation;

        public ICommand RetryCommand { get; }

        public ExtendedImage()
        {
            RetryCommand = new Command(Retry);
        }

        #region CustomSource

        public static readonly BindableProperty CustomSourceProperty =
            BindableProperty.Create(
                nameof(CustomSource),
                typeof(string),
                typeof(ExtendedImage),
                default(string),
                propertyChanged: OnCustomSourceChanged);

        private static void OnCustomSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (ExtendedImage)bindable;
            // Skip restart when MAUI re-binds the same URL (e.g. first CollectionView cell during initial layout)
            if (string.Equals(oldValue as string, newValue as string, StringComparison.Ordinal)) return;
            var gen = Interlocked.Increment(ref ctrl._generation);
            ctrl.ApplySource(newValue as string, gen);
        }

        public string CustomSource
        {
            get => (string)GetValue(CustomSourceProperty);
            set => SetValue(CustomSourceProperty, value);
        }

        #endregion

        #region PageCancellationTokenSource

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

        #region IsShowingPlaceholder

        public static readonly BindableProperty IsShowingPlaceholderProperty =
            BindableProperty.Create(
                nameof(IsShowingPlaceholder),
                typeof(bool),
                typeof(ExtendedImage),
                false);

        public bool IsShowingPlaceholder
        {
            get => (bool)GetValue(IsShowingPlaceholderProperty);
            private set => SetValue(IsShowingPlaceholderProperty, value);
        }

        #endregion

        #region IsImageLoading

        public static readonly BindableProperty IsImageLoadingProperty =
            BindableProperty.Create(
                nameof(IsImageLoading),
                typeof(bool),
                typeof(ExtendedImage),
                false);

        public bool IsImageLoading
        {
            get => (bool)GetValue(IsImageLoadingProperty);
            private set => SetValue(IsImageLoadingProperty, value);
        }

        #endregion

        #region ImageWidth / ImageHeight

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

        public void Retry()
        {
            var url = CustomSource;
            if (string.IsNullOrEmpty(url)) return;
            ImagePipeline.Instance.Invalidate(url);
            IsShowingPlaceholder = false;
            var gen = Interlocked.Increment(ref _generation);
            ApplySource(url, gen);
        }

        private async void ApplySource(string url, int gen)
        {
            // async void — must catch everything to prevent app crash
            try
            {
                var oldCts = _loadCts;
                _loadCts = new CancellationTokenSource();
                var token = _loadCts.Token;
                oldCts?.Cancel();
                oldCts?.Dispose();

                base.Source = ImagePipeline.Instance.Placeholder;
                IsShowingPlaceholder = false;
                IsImageLoading = true;

                if (string.IsNullOrEmpty(url))
                {
                    IsImageLoading = false;
                    return;
                }

                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var fileSrc = File.Exists(url) ? ImageSource.FromFile(url) : ImagePipeline.Instance.Placeholder;
                    base.Source = fileSrc;
                    IsShowingPlaceholder = ReferenceEquals(fileSrc, ImagePipeline.Instance.Placeholder);
                    IsImageLoading = false;
                    return;
                }

                // Reading .Token on a disposed CancellationTokenSource throws ObjectDisposedException —
                // guard so a page tear-down race doesn't crash the app.
                CancellationTokenSource linked = null;
                try
                {
                    var pageCts = PageCancellationTokenSource;
                    linked = pageCts != null
                        ? CancellationTokenSource.CreateLinkedTokenSource(token, pageCts.Token)
                        : CancellationTokenSource.CreateLinkedTokenSource(token);
                }
                catch (ObjectDisposedException)
                {
                    // Page already torn down — treat as cancelled
                    IsImageLoading = false;
                    return;
                }

                using (linked)
                {
                    var src = await ImagePipeline.Instance.GetAsync(url, linked.Token);

                    if (linked.IsCancellationRequested)
                    {
                        IsImageLoading = false;
                        return;
                    }

                    // Show retry icon immediately on first failure; stop spinner — background retries are silent
                    if (ReferenceEquals(src, ImagePipeline.Instance.Placeholder))
                    {
                        Application.Current?.Dispatcher.Dispatch(() =>
                        {
                            if (gen == _generation && CustomSource == url)
                            {
                                IsShowingPlaceholder = true;
                                IsImageLoading = false;
                            }
                        });

                        int[] retryDelaysMs = { 3000, 8000, 20000 };
                        foreach (var delayMs in retryDelaysMs)
                        {
                            try { await Task.Delay(delayMs, linked.Token); }
                            catch (OperationCanceledException)
                            {
                                // Cancelled during delay — spinner already hidden above
                                return;
                            }

                            if (linked.IsCancellationRequested) return;

                            src = await ImagePipeline.Instance.GetAsync(url, linked.Token);

                            if (linked.IsCancellationRequested) return;

                            if (!ReferenceEquals(src, ImagePipeline.Instance.Placeholder)) break;
                        }
                    }

                    Application.Current?.Dispatcher.Dispatch(() =>
                    {
                        if (gen == _generation && CustomSource == url)
                        {
                            base.Source = src;
                            IsShowingPlaceholder = ReferenceEquals(src, ImagePipeline.Instance.Placeholder);
                            IsImageLoading = false;
                        }
                    });
                }
            }
            catch (Exception)
            {
                // Swallow all unexpected errors — ensure spinner is always cleared
                try
                {
                    IsImageLoading = false;
                    IsShowingPlaceholder = true;
                }
                catch { }
            }
        }
    }
}
