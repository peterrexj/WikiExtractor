using CoreGraphics;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Handlers.Items;
using UIKit;
using WikiExtractor.Maui.App.Controls;

namespace WikiExtractor.Maui.App.Platforms.iOS
{
    public class SwipeCollectionViewHandler : CollectionViewHandler
    {
        private IDisposable? _swipeDelegate;
        private UIGestureRecognizer? _panRecognizer;

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);

            if (VirtualView is SwipeCollectionView scv)
            {
                // Controller?.CollectionView is the normal path (MAUI 7–10+).
                // Fall back to platformView itself in case the internal property is ever renamed —
                // the platform view of a CollectionViewHandler on iOS is always a UICollectionView.
                var collectionView = Controller?.CollectionView
                    ?? platformView as UICollectionView;
                if (collectionView != null)
                {
                    var d = new SwipePanDelegate(collectionView, scv);
                    _swipeDelegate = d;
                    _panRecognizer = d.PanRecognizer;
                    collectionView.AddGestureRecognizer(_panRecognizer);
                }
            }
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            if (_panRecognizer != null)
            {
                Controller?.CollectionView?.RemoveGestureRecognizer(_panRecognizer);
                _panRecognizer = null;
            }
            _swipeDelegate?.Dispose();
            _swipeDelegate = null;
            base.DisconnectHandler(platformView);
        }
    }

    // Drives swipe-to-act on iOS using a UIPanGestureRecognizer added to the UICollectionView.
    // shouldRecognizeSimultaneouslyWith returns true so the built-in scroll gesture and our
    // horizontal pan can both track the same touch — scroll coexists with swipe naturally.
    //
    // [Preserve] is required: the linker sees [Export] methods as unreachable from C# and strips
    // them in release builds, silently breaking gesture delegation at runtime.
    [Foundation.Preserve(AllMembers = true)]
    file class SwipePanDelegate : NSObject, IUIGestureRecognizerDelegate
    {
        private readonly UICollectionView _collectionView;
        private readonly SwipeCollectionView _virtualView;

        // Track per-cell state: indexPath → (startX, committed)
        private NSIndexPath? _activeIndexPath;
        private nfloat _startX;
        private bool _committed;
        private bool? _horizontal;

        public UIPanGestureRecognizer PanRecognizer { get; }

        public SwipePanDelegate(UICollectionView cv, SwipeCollectionView scv)
        {
            _collectionView = cv;
            _virtualView = scv;

            PanRecognizer = new UIPanGestureRecognizer(OnPan)
            {
                MaximumNumberOfTouches = 1,
                Delegate = this
            };
        }

        // Allow simultaneous recognition with the UICollectionView's built-in scroll gesture.
        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
            => true;

        // Only begin if horizontal movement is dominant — prevents stealing vertical scrolls.
        [Export("gestureRecognizerShouldBegin:")]
        public bool ShouldBegin(UIGestureRecognizer recognizer)
        {
            if (recognizer is not UIPanGestureRecognizer pan) return true;
            var velocity = pan.VelocityInView(_collectionView);
            return Math.Abs(velocity.X) > Math.Abs(velocity.Y);
        }

        private void OnPan(UIPanGestureRecognizer pan)
        {
            var location = pan.LocationInView(_collectionView);
            var translation = pan.TranslationInView(_collectionView);

            switch (pan.State)
            {
                case UIGestureRecognizerState.Began:
                {
                    _activeIndexPath = _collectionView.IndexPathForItemAtPoint(location);
                    if (_activeIndexPath == null) return;
                    _startX = translation.X;
                    _committed = false;
                    _horizontal = null;
                    break;
                }

                case UIGestureRecognizerState.Changed:
                {
                    if (_activeIndexPath == null) return;

                    var totalX = (float)(translation.X - _startX);

                    // Lock direction on first 8pt of movement
                    if (_horizontal == null)
                    {
                        var absX = Math.Abs(translation.X);
                        var absY = Math.Abs(translation.Y);
                        if (absX < 8 && absY < 8) return;
                        _horizontal = absX >= absY;
                    }

                    if (_horizontal == false) return; // vertical — let scroll handle it

                    // Only allow rightward drag
                    if (totalX < 0) totalX = 0;

                    var (ctx, mauiView) = GetContextAndView(_activeIndexPath);
                    if (ctx == null) return;

                    MainThread.BeginInvokeOnMainThread(() => _virtualView.NotifyProgress(ctx, totalX, mauiView));

                    if (!_committed && totalX >= _virtualView.SwipeThreshold)
                    {
                        _committed = true;
                        MainThread.BeginInvokeOnMainThread(() => _virtualView.NotifySwiped(ctx, SwipeDirection.Right, mauiView));
                    }
                    break;
                }

                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                {
                    if (_activeIndexPath == null) return;
                    var (ctx, mauiView) = GetContextAndView(_activeIndexPath);
                    if (ctx != null && !_committed)
                        MainThread.BeginInvokeOnMainThread(() => _virtualView.NotifyCancelled(ctx, 0, mauiView));
                    _activeIndexPath = null;
                    _committed = false;
                    _horizontal = null;
                    break;
                }
            }
        }

        private (object? ctx, Microsoft.Maui.Controls.View? mauiView) GetContextAndView(NSIndexPath indexPath)
        {
            var cell = _collectionView.CellForItem(indexPath);
            if (cell == null) return (null, null);
            return FindMauiView(cell.ContentView);
        }

        private static (object? ctx, Microsoft.Maui.Controls.View? mauiView) FindMauiView(UIView? view)
        {
            if (view == null) return (null, null);

            if (view is ICrossPlatformLayoutBacking backing &&
                backing.CrossPlatformLayout is BindableObject bo &&
                bo.BindingContext != null &&
                bo is Microsoft.Maui.Controls.View mv)
                return (bo.BindingContext, mv);

            foreach (var sub in view.Subviews)
            {
                var result = FindMauiView(sub);
                if (result.ctx != null) return result;
            }

            return (null, null);
        }
    }
}
