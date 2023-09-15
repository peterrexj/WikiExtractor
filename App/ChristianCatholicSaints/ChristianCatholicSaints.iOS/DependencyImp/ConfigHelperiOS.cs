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

        public static string SyncFusionLicense => "yMqIYrHN0hK0qOeiL6UKQTMEZt5VOAfo9RUFQMnQe26vhkLiMHkhpACIUObDx5t8i35SUk+BWMWO3D5D2C8M2peJ+lrDxmD9fT7VKLEyI5KkDhUfVqI7r1K8XcdTd5iT";
    }
}