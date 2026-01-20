using Microsoft.Maui.Handlers;
using PjAds.Maui.Models;
using PjAds.Maui.Services;

namespace PjAds.Maui.Controls
{
    /// <summary>
    /// Cross-platform banner ad view for MAUI
    /// </summary>
    public class BannerAdView : View
    {
        /// <summary>
        /// Bindable property for AdUnitId
        /// </summary>
        public static readonly BindableProperty AdUnitIdProperty =
            BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(BannerAdView), string.Empty);

        /// <summary>
        /// Bindable property for AdSize
        /// </summary>
        public static readonly BindableProperty AdSizeProperty =
            BindableProperty.Create(nameof(AdSize), typeof(AdSize), typeof(BannerAdView), AdSize.Banner);

        /// <summary>
        /// Bindable property for BannerType
        /// </summary>
        public static readonly BindableProperty BannerTypeProperty =
            BindableProperty.Create(nameof(BannerType), typeof(BannerType), typeof(BannerAdView), BannerType.Regular);

        /// <summary>
        /// Gets or sets the ad unit ID
        /// </summary>
        public string AdUnitId
        {
            get => (string)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }

        /// <summary>
        /// Gets or sets the ad size
        /// </summary>
        public AdSize AdSize
        {
            get => (AdSize)GetValue(AdSizeProperty);
            set => SetValue(AdSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the banner type
        /// </summary>
        public BannerType BannerType
        {
            get => (BannerType)GetValue(BannerTypeProperty);
            set => SetValue(BannerTypeProperty, value);
        }

        /// <summary>
        /// Event fired when the ad is loaded
        /// </summary>
        public event EventHandler<AdLoadedEventArgs>? AdLoaded;

        /// <summary>
        /// Event fired when the ad fails to load
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;

        /// <summary>
        /// Event fired when the ad is clicked
        /// </summary>
        public event EventHandler<AdClickedEventArgs>? AdClicked;

        /// <summary>
        /// Event fired when an ad impression is recorded
        /// </summary>
        public event EventHandler<AdImpressionEventArgs>? AdImpression;

        /// <summary>
        /// Internal method to raise AdLoaded event
        /// </summary>
        internal void OnAdLoaded(AdLoadedEventArgs args) => AdLoaded?.Invoke(this, args);

        /// <summary>
        /// Internal method to raise AdFailedToLoad event
        /// </summary>
        internal void OnAdFailedToLoad(AdFailedToLoadEventArgs args) => AdFailedToLoad?.Invoke(this, args);

        /// <summary>
        /// Internal method to raise AdClicked event
        /// </summary>
        internal void OnAdClicked(AdClickedEventArgs args) => AdClicked?.Invoke(this, args);

        /// <summary>
        /// Internal method to raise AdImpression event
        /// </summary>
        internal void OnAdImpression(AdImpressionEventArgs args) => AdImpression?.Invoke(this, args);
    }

    /// <summary>
    /// Handler for BannerAdView
    /// </summary>
#if IOS
    public partial class BannerAdViewHandler : ViewHandler<BannerAdView, UIKit.UIView>
#elif ANDROID
    public partial class BannerAdViewHandler : ViewHandler<BannerAdView, Android.Views.View>
#else
    public partial class BannerAdViewHandler : ViewHandler<BannerAdView, object>
#endif
    {
        /// <summary>
        /// Property mapper for BannerAdView
        /// </summary>
        public static IPropertyMapper<BannerAdView, BannerAdViewHandler> Mapper = new PropertyMapper<BannerAdView, BannerAdViewHandler>(ViewMapper)
        {
            [nameof(BannerAdView.AdUnitId)] = MapAdUnitId,
            [nameof(BannerAdView.AdSize)] = MapAdSize,
            [nameof(BannerAdView.BannerType)] = MapBannerType,
        };

        /// <summary>
        /// Command mapper for BannerAdView
        /// </summary>
        public static CommandMapper<BannerAdView, BannerAdViewHandler> CommandMapper = new(ViewCommandMapper)
        {
        };

        public BannerAdViewHandler() : base(Mapper, CommandMapper)
        {
        }

        private static void MapAdUnitId(BannerAdViewHandler handler, BannerAdView view)
        {
            handler.UpdateAdUnitId();
        }

        private static void MapAdSize(BannerAdViewHandler handler, BannerAdView view)
        {
            handler.UpdateAdSize();
        }

        private static void MapBannerType(BannerAdViewHandler handler, BannerAdView view)
        {
            handler.UpdateBannerType();
        }

        // These tell the compiler that the implementation exists in the platform files
        partial void UpdateAdUnitId();
        partial void UpdateAdSize();
        partial void UpdateBannerType();

#if !IOS && !ANDROID
        protected override object CreatePlatformView()
        {
            // Return a placeholder object for unsupported platforms
            return new object();
        }

        protected virtual void UpdateAdUnitId() { }
        protected virtual void UpdateAdSize() { }
        protected virtual void UpdateBannerType() { }
#endif
    }
}