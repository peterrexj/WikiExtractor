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

        public static string SyncFusionLicense => "yMqIYrHN0hK0qOeiL6UKQVE3vIKBAye7kGsEiZjqZzZu5OqUauT2RgGMbPNoEMCQWvKdqGG+lw02JB2WY8I8HGrJ+mc3M5cSRjBorRGjn0GDN/7bX6Id7eNygLmnrTNd";
    }
}