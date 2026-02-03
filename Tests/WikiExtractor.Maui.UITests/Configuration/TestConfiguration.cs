using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace WikiExtractor.Maui.UITests.Configuration;

public class TestConfiguration
{
    private static TestConfiguration? _instance;
    private static readonly object _lock = new object();
    private readonly IConfiguration _configuration;

    private TestConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static TestConfiguration Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TestConfiguration();
                    }
                }
            }
            return _instance;
        }
    }

    public string AppiumServerUrl => _configuration["AppiumServer:Url"] ?? "http://localhost:4723";
    public int CommandTimeout => int.Parse(_configuration["AppiumServer:CommandTimeout"] ?? "120");

    public AppiumSettings GetAndroidSettings()
    {
        return new AppiumSettings
        {
            PlatformName = _configuration["Android:PlatformName"] ?? "Android",
            AutomationName = _configuration["Android:AutomationName"] ?? "UIAutomator2",
            DeviceName = _configuration["Android:DeviceName"] ?? "Android Emulator",
            PlatformVersion = _configuration["Android:PlatformVersion"] ?? "13.0",
            AppPackage = _configuration["Android:AppPackage"] ?? "",
            AppActivity = _configuration["Android:AppActivity"] ?? "",
            AppPath = _configuration["Android:AppPath"] ?? "",
            NewCommandTimeout = int.Parse(_configuration["Android:NewCommandTimeout"] ?? "3000")
        };
    }

    public AppiumSettings GetIOSSettings()
    {
        return new AppiumSettings
        {
            PlatformName = _configuration["iOS:PlatformName"] ?? "iOS",
            AutomationName = _configuration["iOS:AutomationName"] ?? "XCUITest",
            DeviceName = _configuration["iOS:DeviceName"] ?? "iPhone 15",
            PlatformVersion = _configuration["iOS:PlatformVersion"] ?? "17.2",
            BundleId = _configuration["iOS:BundleId"] ?? "",
            AppPath = _configuration["iOS:AppPath"] ?? "",
            NewCommandTimeout = int.Parse(_configuration["iOS:NewCommandTimeout"] ?? "3000"),
            UsePrebuiltWDA = bool.Parse(_configuration["iOS:UsePrebuiltWDA"] ?? "true"),
            WDALaunchTimeout = int.Parse(_configuration["iOS:WDALaunchTimeout"] ?? "60000"),
            WDAConnectionTimeout = int.Parse(_configuration["iOS:WDAConnectionTimeout"] ?? "120000")
        };
    }

    public string ScreenshotPath => _configuration["TestSettings:ScreenshotPath"] ?? "Screenshots";
    public int ImplicitWaitTimeout => int.Parse(_configuration["TestSettings:ImplicitWaitTimeout"] ?? "10");
    public bool TakeScreenshotOnFailure => bool.Parse(_configuration["TestSettings:TakeScreenshotOnFailure"] ?? "true");
}

public class AppiumSettings
{
    public string PlatformName { get; set; } = string.Empty;
    public string AutomationName { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string PlatformVersion { get; set; } = string.Empty;
    public string AppPackage { get; set; } = string.Empty;
    public string AppActivity { get; set; } = string.Empty;
    public string BundleId { get; set; } = string.Empty;
    public string AppPath { get; set; } = string.Empty;
    public int NewCommandTimeout { get; set; }
    public bool UsePrebuiltWDA { get; set; }
    public int WDALaunchTimeout { get; set; }
    public int WDAConnectionTimeout { get; set; }
}
