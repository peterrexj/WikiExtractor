using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using WikiExtractor.Maui.UITests.Configuration;

namespace WikiExtractor.Maui.UITests.Drivers;

public class AppiumDriverFactory
{
    private readonly TestConfiguration _config;

    public AppiumDriverFactory()
    {
        _config = TestConfiguration.Instance;
    }

    public AppiumDriver CreateDriver(Platform platform)
    {
        var serverUri = new Uri(_config.AppiumServerUrl);
        var options = CreateAppiumOptions(platform);

        AppiumDriver driver = platform switch
        {
            Platform.Android => new AndroidDriver(serverUri, options, TimeSpan.FromSeconds(_config.CommandTimeout)),
            Platform.iOS => new IOSDriver(serverUri, options, TimeSpan.FromSeconds(_config.CommandTimeout)),
            _ => throw new ArgumentException($"Unsupported platform: {platform}")
        };

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_config.ImplicitWaitTimeout);
        return driver;
    }

    private AppiumOptions CreateAppiumOptions(Platform platform)
    {
        var options = new AppiumOptions();

        if (platform == Platform.Android)
        {
            var settings = _config.GetAndroidSettings();
            options.PlatformName = settings.PlatformName;
            options.AutomationName = settings.AutomationName;
            options.DeviceName = settings.DeviceName;
            options.PlatformVersion = settings.PlatformVersion;
            options.AddAdditionalAppiumOption("newCommandTimeout", settings.NewCommandTimeout);
            options.AddAdditionalAppiumOption("autoGrantPermissions", true);
            options.AddAdditionalAppiumOption("noReset", false);
            options.AddAdditionalAppiumOption("fullReset", false);

            if (!string.IsNullOrEmpty(settings.AppPath))
            {
                options.App = settings.AppPath;
            }
            else if (!string.IsNullOrEmpty(settings.AppPackage))
            {
                options.AddAdditionalAppiumOption("appPackage", settings.AppPackage);
                options.AddAdditionalAppiumOption("appActivity", settings.AppActivity);
            }
        }
        else if (platform == Platform.iOS)
        {
            var settings = _config.GetIOSSettings();
            options.PlatformName = settings.PlatformName;
            options.AutomationName = settings.AutomationName;
            options.DeviceName = settings.DeviceName;
            options.PlatformVersion = settings.PlatformVersion;
            options.AddAdditionalAppiumOption("newCommandTimeout", settings.NewCommandTimeout);
            options.AddAdditionalAppiumOption("autoAcceptAlerts", true);
            options.AddAdditionalAppiumOption("noReset", false);
            options.AddAdditionalAppiumOption("fullReset", false);
            options.AddAdditionalAppiumOption("usePrebuiltWDA", settings.UsePrebuiltWDA);
            options.AddAdditionalAppiumOption("wdaLaunchTimeout", settings.WDALaunchTimeout);
            options.AddAdditionalAppiumOption("wdaConnectionTimeout", settings.WDAConnectionTimeout);

            if (!string.IsNullOrEmpty(settings.AppPath))
            {
                options.App = settings.AppPath;
            }
            else if (!string.IsNullOrEmpty(settings.BundleId))
            {
                options.AddAdditionalAppiumOption("bundleId", settings.BundleId);
            }
        }

        return options;
    }
}
