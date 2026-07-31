using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using WikiExtractor.Maui.App.Controls;

namespace WikiExtractor.Maui.App.Platforms.Android
{
    public class SwipeCollectionViewHandler : CollectionViewHandler
    {
        // CreatePlatformView must return a MauiRecyclerView<,,> subclass, not a plain RecyclerView.
        // The base ConnectHandler casts to IMauiRecyclerView<TItemsView> to call SetUpNewElement;
        // a plain RecyclerView fails that cast silently (null-safe ?.call) → no adapter, empty list.
        //
        // SwipeableRecyclerView uses the exact TAdapter/TItemsViewSource types produced by the
        // handler chain (ReorderableItemsViewAdapter / IGroupableItemsViewSource), so CreateAdapter
        // passes as a method group with no runtime cast.
        protected override RecyclerView CreatePlatformView()
            => new SwipeableRecyclerView(Context!, GetItemsLayout, CreateAdapter);

        protected override void ConnectHandler(RecyclerView platformView)
        {
            base.ConnectHandler(platformView); // IMauiRecyclerView cast succeeds → SetUpNewElement runs
            if (platformView is SwipeableRecyclerView srv && VirtualView is SwipeCollectionView scv)
                srv.VirtualSwipeView = scv;
        }

        protected override void DisconnectHandler(RecyclerView platformView)
        {
            if (platformView is SwipeableRecyclerView srv)
                srv.VirtualSwipeView = null;
            base.DisconnectHandler(platformView);
        }
    }

    // Subclasses MauiRecyclerView so IMauiRecyclerView<TItemsView> cast in ConnectHandler succeeds.
    //
    // Swipe detection is in DispatchTouchEvent, which fires before FLAG_DISALLOW_INTERCEPT is checked.
    // MAUI cells call requestDisallowInterceptTouchEvent(true) on ACTION_DOWN, which would block
    // any approach based on onInterceptTouchEvent (including ItemTouchHelper). DispatchTouchEvent
    // is unconditional. The list is vertical-only so horizontal movement is unambiguous.
    //
    // Upgrade safety notes:
    // - MauiRecyclerView constructor (Context, Func<IItemsLayout>, Func<TAdapter>) has been stable
    //   since MAUI 7. If it ever changes, we get a compile error, not a silent runtime failure.
    // - The adapter delegate matches the exact type returned by the handler chain
    //   (ReorderableItemsViewAdapter / IGroupableItemsViewSource), so no runtime cast is needed.
    //   If MAUI ever changes the adapter hierarchy this becomes a compile error, not a silent failure.
    // - ICrossPlatformLayoutBacking: if removed in a future MAUI version, FindMauiView falls back to
    //   recursing children and still finds the BindableObject — swipe degrades gracefully, not broken.
    // - DispatchTouchEvent override: observe-only (never swallows events), so MauiRecyclerView's
    //   own ParentScrollGestureDispatcher and IsEnabled logic are fully preserved.
    internal class SwipeableRecyclerView
        : MauiRecyclerView<
            ReorderableItemsView,
            ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>,
            IGroupableItemsViewSource>
    {
        internal SwipeCollectionView? VirtualSwipeView { get; set; }

        private float _downX, _downY;
        private bool? _horizontal;           // null=undecided, true=horizontal, false=vertical
        private RecyclerView.ViewHolder? _activeVh;
        private bool _committed;

        private float Dp(float px) => px / Context!.Resources!.DisplayMetrics!.Density;

        public SwipeableRecyclerView(
            global::Android.Content.Context context,
            Func<IItemsLayout> getItemsLayout,
            Func<ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>> getAdapter)
            : base(context, getItemsLayout, getAdapter) { }

        public override bool DispatchTouchEvent(MotionEvent? ev)
        {
            // Observe BEFORE delegating so we capture the raw coordinates on ACTION_DOWN.
            // We never swallow events (always call base), so MauiRecyclerView's own
            // ParentScrollGestureDispatcher and IsEnabled checks are fully preserved.
            if (ev != null && VirtualSwipeView != null)
                ObserveSwipe(ev);
            return base.DispatchTouchEvent(ev);
        }

        private void ObserveSwipe(MotionEvent ev)
        {
            switch (ev.ActionMasked)
            {
                case MotionEventActions.Down:
                {
                    _downX = ev.GetX();
                    _downY = ev.GetY();
                    _horizontal = null;
                    _committed = false;
                    var child = FindChildViewUnder(ev.GetX(), ev.GetY());
                    _activeVh = child != null ? GetChildViewHolder(child) : null;
                    break;
                }

                case MotionEventActions.Move:
                {
                    if (_activeVh == null) break;

                    float dx = ev.GetX() - _downX;
                    float dy = ev.GetY() - _downY;

                    // Lock direction once past dead zone (8 px raw — ~3dp on hdpi)
                    if (_horizontal == null && (Math.Abs(dx) > 8 || Math.Abs(dy) > 8))
                        _horizontal = Math.Abs(dx) > Math.Abs(dy);

                    if (_horizontal != true) break;
                    if (dx < 0) break;  // right-swipe only

                    float dxDp = Dp(dx);
                    var (bo, mauiView) = FindMauiView(_activeVh.ItemView);
                    if (bo?.BindingContext == null) break;

                    var ctx = bo.BindingContext;
                    var mv  = mauiView;
                    var sv  = VirtualSwipeView!;

                    MainThread.BeginInvokeOnMainThread(() => sv.NotifyProgress(ctx, dxDp, mv));

                    if (!_committed && dxDp >= sv.SwipeThreshold)
                    {
                        _committed = true;
                        MainThread.BeginInvokeOnMainThread(() => sv.NotifySwiped(ctx, SwipeDirection.Right, mv));
                    }
                    break;
                }

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                {
                    if (_activeVh != null && _horizontal == true && !_committed)
                    {
                        var (bo, mauiView) = FindMauiView(_activeVh.ItemView);
                        if (bo?.BindingContext != null)
                        {
                            var ctx = bo.BindingContext;
                            var mv  = mauiView;
                            var sv  = VirtualSwipeView!;
                            MainThread.BeginInvokeOnMainThread(() => sv.NotifyCancelled(ctx, 0, mv));
                        }
                    }
                    _activeVh   = null;
                    _horizontal = null;
                    _committed  = false;
                    break;
                }
            }
        }

        // Walk the native view tree to find the MAUI DataTemplate root and its BindingContext.
        //
        // Current MAUI cell structure (stable since MAUI 7, verified on main branch):
        //   ItemContentView (vh.ItemView) — plain ViewGroup, NOT ICrossPlatformLayoutBacking
        //     └── LayoutViewGroup — implements ICrossPlatformLayoutBacking
        //           CrossPlatformLayout = Grid (DataTemplate root; BindingContext = item VM)
        //
        // ICrossPlatformLayoutBacking fallback: if the interface is ever removed or renamed in a
        // future MAUI version, the second branch finds a BindableObject child with a non-null
        // BindingContext, so swipe degrades to finding the right view by context rather than failing.
        private static (BindableObject? bo, Microsoft.Maui.Controls.View? view) FindMauiView(
            global::Android.Views.View? nativeView)
        {
            if (nativeView == null) return (null, null);

            // Primary path: ICrossPlatformLayoutBacking (MAUI 7–10+)
            if (nativeView is ICrossPlatformLayoutBacking backing &&
                backing.CrossPlatformLayout is BindableObject bo &&
                bo.BindingContext != null &&
                bo is Microsoft.Maui.Controls.View mv)
                return (bo, mv);

            // Recurse into children
            if (nativeView is ViewGroup vg)
                for (int i = 0; i < vg.ChildCount; i++)
                {
                    var r = FindMauiView(vg.GetChildAt(i));
                    if (r.bo != null) return r;
                }

            return (null, null);
        }
    }
}
