using PjAds.Maui.Services;

namespace PjAds.Maui.Models
{
    /// <summary>
    /// Event arguments for ad events
    /// </summary>
    public class AdEventArgs : EventArgs
    {
        public string? Message { get; set; }
        public Exception? Exception { get; set; }
        public string? AdUnitId { get; set; }

        public AdEventArgs() { }

        public AdEventArgs(string message)
        {
            Message = message;
        }

        public AdEventArgs(Exception exception)
        {
            Exception = exception;
            Message = exception.Message;
        }

        public AdEventArgs(string adUnitId, string message)
        {
            AdUnitId = adUnitId;
            Message = message;
        }
    }

    /// <summary>
    /// Event arguments for ad load events
    /// </summary>
    public class AdLoadedEventArgs : AdEventArgs
    {
        public BannerType BannerType { get; set; }
        
        public AdLoadedEventArgs(string adUnitId) : base(adUnitId, "Ad loaded successfully") { }
        
        public AdLoadedEventArgs(string adUnitId, BannerType bannerType) : base(adUnitId, "Ad loaded successfully")
        {
            BannerType = bannerType;
        }
    }

    /// <summary>
    /// Event arguments for ad failed to load events
    /// </summary>
    public class AdFailedToLoadEventArgs : AdEventArgs
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage => Message ?? "Unknown error";

        public AdFailedToLoadEventArgs(string adUnitId, int errorCode, string message)
            : base(adUnitId, message)
        {
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Event arguments for ad clicked events
    /// </summary>
    public class AdClickedEventArgs : AdEventArgs
    {
        public BannerType BannerType { get; set; }
        
        public AdClickedEventArgs(string adUnitId) : base(adUnitId, "Ad clicked") { }
        
        public AdClickedEventArgs(string adUnitId, BannerType bannerType) : base(adUnitId, "Ad clicked")
        {
            BannerType = bannerType;
        }
    }

    /// <summary>
    /// Event arguments for ad impression events
    /// </summary>
    public class AdImpressionEventArgs : AdEventArgs
    {
        public AdImpressionEventArgs(string adUnitId) : base(adUnitId, "Ad impression recorded") { }
    }

    /// <summary>
    /// Event arguments for interstitial ad closed events
    /// </summary>
    public class InterstitialAdClosedEventArgs : AdEventArgs
    {
        public InterstitialAdClosedEventArgs(string adUnitId) : base(adUnitId, "Interstitial ad closed") { }
    }

    /// <summary>
    /// Event arguments for interstitial ad opened events
    /// </summary>
    public class InterstitialAdOpenedEventArgs : AdEventArgs
    {
        public InterstitialAdOpenedEventArgs(string adUnitId) : base(adUnitId, "Interstitial ad opened") { }
    }

    /// <summary>
    /// Event arguments for interstitial ad showed events
    /// </summary>
    public class InterstitialAdShowedEventArgs : AdEventArgs
    {
        public InterstitialAdShowedEventArgs(string adUnitId) : base(adUnitId, "Interstitial ad showed") { }
    }
}