using System;
using System.Collections.Generic;
using System.Text;
namespace WikiExtractor.Maui.App.Exts
{
    public class DeviceDetails
    {
        public static string GetDeviceModel()
        {
            return DeviceInfo.Model;
        }

        public static string GetOperatingSystemVersion()
        {
            return DeviceInfo.VersionString;
        }

        public static double GetScreenWidth()
        {
            return DeviceDisplay.MainDisplayInfo.Width;
        }

        public static double GetScreenHeight()
        {
            return DeviceDisplay.MainDisplayInfo.Height;
        }

        public static NetworkAccess GetNetworkAccess()
        {
            return Connectivity.NetworkAccess;
        }

        public static IDictionary<string, string> GenerateDeviceInformation()
        {
            return new Dictionary<string, string>
            {
                { "Device Model", GetDeviceModel() },
                { "Device Operating System Version", GetOperatingSystemVersion() },
                { "Device Screen Width", GetScreenWidth().ToString() },
                { "Device Screen Height", GetScreenHeight().ToString() },
                { "Device Network Access", GetNetworkAccess().ToString() }
            };
        }
        public static IDictionary<string, string> GenerateMetaInformation(Dictionary<string, string> additionalInformation)
        {
            if (additionalInformation != null)
            {
                foreach (var item in GenerateDeviceInformation())
                {
                    if (additionalInformation.ContainsKey(item.Key) == false)
                    {
                        additionalInformation.Add(item.Key, item.Value);
                    }
                }
                return additionalInformation;
            }
            else
            {
                return GenerateDeviceInformation();
            }
        }

    }
}