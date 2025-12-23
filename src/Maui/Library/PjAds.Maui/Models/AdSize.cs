namespace PjAds.Maui.Models
{
    /// <summary>
    /// Represents different ad sizes for banner ads
    /// </summary>
    public enum AdSize
    {
        /// <summary>
        /// Standard banner (320x50)
        /// </summary>
        Banner,
        
        /// <summary>
        /// Large banner (320x100)
        /// </summary>
        LargeBanner,
        
        /// <summary>
        /// Medium rectangle (300x250)
        /// </summary>
        MediumRectangle,
        
        /// <summary>
        /// Full banner (468x60)
        /// </summary>
        FullBanner,
        
        /// <summary>
        /// Leaderboard (728x90)
        /// </summary>
        Leaderboard,
        
        /// <summary>
        /// Smart banner (screen width x 32|50|90)
        /// </summary>
        SmartBanner
    }
}