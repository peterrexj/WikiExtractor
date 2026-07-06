using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Exts
{
    public class ExtendedImage : Image
    {
        private CancellationTokenSource _loadCts;
        private int _generation;

        #region CustomSource

        public static readonly BindableProperty CustomSourceProperty =
            BindableProperty.Create(
                nameof(CustomSource),
                typeof(string),
                typeof(ExtendedImage),
                default(string),
                propertyChanged: OnCustomSourcePropertyChanged);

        private static void OnCustomSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (ExtendedImage)bindable;
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

        private async void ApplySource(string url, int gen)
        {
            // Cancel any previous load for this cell
            var oldCts = _loadCts;
            _loadCts = new CancellationTokenSource();
            var token = _loadCts.Token;
            oldCts?.Cancel();
            oldCts?.Dispose();

            base.Source = ImagePipeline.Instance.Placeholder;

            if (string.IsNullOrEmpty(url)) return;

            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                base.Source = File.Exists(url)
                    ? ImageSource.FromFile(url)
                    : ImagePipeline.Instance.Placeholder;
                return;
            }

            using var linked = PageCancellationTokenSource != null
                ? CancellationTokenSource.CreateLinkedTokenSource(token, PageCancellationTokenSource.Token)
                : CancellationTokenSource.CreateLinkedTokenSource(token);

            var src = await ImagePipeline.Instance.GetAsync(url, linked.Token);

            // Guard: cell may have been recycled to a different item while we were waiting
            if (gen != _generation || linked.IsCancellationRequested) return;

            Application.Current?.Dispatcher.Dispatch(() =>
            {
                // Re-check inside dispatch — generation may have changed again on the UI thread
                if (gen == _generation && CustomSource == url)
                    base.Source = src;
            });
        }
    }
}
