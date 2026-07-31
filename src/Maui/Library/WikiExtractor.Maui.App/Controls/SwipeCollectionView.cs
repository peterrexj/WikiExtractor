namespace WikiExtractor.Maui.App.Controls
{
    public class ItemSwipedEventArgs : EventArgs
    {
        public object BindingContext { get; }
        public SwipeDirection Direction { get; }
        // The MAUI root view of the item cell — avoids walking the logical tree to find it
        public View? ItemView { get; }
        public ItemSwipedEventArgs(object bindingContext, SwipeDirection direction, View? itemView = null)
            => (BindingContext, Direction, ItemView) = (bindingContext, direction, itemView);
    }

    public class ItemSwipeProgressEventArgs : EventArgs
    {
        public object BindingContext { get; }
        public float TranslationX { get; }
        // The MAUI root view of the item cell
        public View? ItemView { get; }
        public ItemSwipeProgressEventArgs(object bindingContext, float translationX, View? itemView = null)
            => (BindingContext, TranslationX, ItemView) = (bindingContext, translationX, itemView);
    }

    /// <summary>
    /// CollectionView subclass whose Android handler attaches an ItemTouchHelper
    /// so swipe-to-act and vertical scroll coexist natively without any gesture conflicts.
    /// </summary>
    public class SwipeCollectionView : CollectionView
    {
        /// Fires continuously while the user is dragging an item (TranslationX in dp).
        public event EventHandler<ItemSwipeProgressEventArgs>? ItemSwipeProgress;

        /// Fires when the user releases and the drag has exceeded SwipeThreshold dp.
        public event EventHandler<ItemSwipedEventArgs>? ItemSwiped;

        /// Fires when the user releases without reaching SwipeThreshold.
        public event EventHandler<ItemSwipeProgressEventArgs>? ItemSwipeCancelled;

        /// Minimum horizontal drag distance in dp to count as a committed swipe.
        public float SwipeThreshold { get; set; } = 120f;

        internal void NotifyProgress(object ctx, float tx, View? itemView = null)
            => ItemSwipeProgress?.Invoke(this, new ItemSwipeProgressEventArgs(ctx, tx, itemView));

        internal void NotifySwiped(object ctx, SwipeDirection dir, View? itemView = null)
            => ItemSwiped?.Invoke(this, new ItemSwipedEventArgs(ctx, dir, itemView));

        internal void NotifyCancelled(object ctx, float tx, View? itemView = null)
            => ItemSwipeCancelled?.Invoke(this, new ItemSwipeProgressEventArgs(ctx, tx, itemView));
    }
}
