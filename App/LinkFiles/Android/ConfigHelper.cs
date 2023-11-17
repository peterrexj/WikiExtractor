using Android.App;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Wiki.Droid
{
    internal static class ConfigHelper
    {
        public static ConcurrentDictionary<string, string> Configs { get; set; }
        public static string SyncFusionLicense => Configs?.FirstOrDefault(f => f.Key == "SyncFusionLicense").Value ?? "";
        public static void LoadConfig()
        {
            
            string content;
            using (var streamReader = new StreamReader(Application.Context.Assets.Open("CommonConfigs.json")))
            {
                content = streamReader.ReadToEnd();
            }
            Configs = Pj.Library.JsonHelper.ConvertComplexJsonDataToDictionary(content);
        }
    }
}