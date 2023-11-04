using System.Collections.Concurrent;

namespace Wiki.iOS
{
    internal static class ConfigHelper
    {
        public static ConcurrentDictionary<string, string> Configs { get; set; }

        public static string SyncFusionLicense => "Ngo9BigBOggjHTQxAR8/V1NHaF5cXmpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWH9feHRcR2lYWEdzW0M=";
    }
}