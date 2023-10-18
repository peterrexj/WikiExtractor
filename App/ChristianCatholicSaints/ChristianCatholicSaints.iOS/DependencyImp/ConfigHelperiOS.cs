using Foundation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace ChristianCatholicSaints.iOS.DependencyImp
{
    internal static class ConfigHelperiOS
    {
        public static ConcurrentDictionary<string, string> Configs { get; set; }

        public static string SyncFusionLicense => "Ngo9BigBOggjHTQxAR8/V1NHaF5cXmpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWH9feHRcR2lYWEdzW0M=";
    }
}