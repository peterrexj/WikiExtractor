using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace GeneralInformation
{
    internal static class ConfigHelperUwp
    {
        public static ConcurrentDictionary<string, string> Configs { get; set; }
        public static string SyncFusionLicense => Configs?.FirstOrDefault(f => f.Key == "SyncFusionLicense").Value ?? "";
        public static void LoadConfig()
        {
            
            string content;
            using (var streamReader = new StreamReader(File.OpenRead(
                        Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "CommonConfigs.json"))))
            {
                content = streamReader.ReadToEnd();
            }
            Configs = Pj.Library.JsonHelper.ConvertComplexJsonDataToDictionary(content);
        }
    }
}