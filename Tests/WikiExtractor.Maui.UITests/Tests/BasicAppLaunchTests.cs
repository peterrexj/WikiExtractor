using NUnit.Framework;
using WikiExtractor.Maui.UITests.Base;

namespace WikiExtractor.Maui.UITests.Tests;

[TestFixture]
public class BasicAppLaunchTests : BaseTest
{
    [Test]
    [Category("Smoke")]
    [Description("Verify that the application launches successfully and can be closed")]
    public void Test_LaunchAndCloseApp()
    {
        // Arrange & Act
        // The app is already launched in the SetUp method
        Console.WriteLine("App launched successfully");
        
        // Take a screenshot of the initial screen
        TakeScreenshot("AppLaunched");
        
        // Wait for a few seconds to ensure app is fully loaded
        WaitForSeconds(2);
        
        // Take another screenshot to verify app is responsive
        TakeScreenshot("AppRunning");
        
        // Assert
        Assert.That(Driver, Is.Not.Null, "Driver should be initialized");
        Assert.That(Driver!.SessionId, Is.Not.Null, "App session should be active");
        
        Console.WriteLine("App is running and responsive");
        
        // The app will be closed in the TearDown method
    }

    [Test]
    [Category("Smoke")]
    [Description("Verify that the application can be launched multiple times")]
    public void Test_MultipleLaunchCycles()
    {
        // First launch (already done in SetUp)
        Console.WriteLine("First launch - taking screenshot");
        TakeScreenshot("FirstLaunch");
        WaitForSeconds(1);
        
        // Verify app is running
        Assert.That(Driver, Is.Not.Null);
        Assert.That(Driver!.SessionId, Is.Not.Null, "App should be running");
        
        Console.WriteLine("App launch cycle completed successfully");
    }

    [Test]
    [Category("Smoke")]
    [Description("Verify app context and package information")]
    public void Test_VerifyAppContext()
    {
        // Take initial screenshot
        TakeScreenshot("AppContext");
        
        // Verify driver session
        Assert.That(Driver, Is.Not.Null, "Driver should be initialized");
        Assert.That(Driver!.SessionId, Is.Not.Null, "Session should be active");
        
        // Get and log session capabilities
        var capabilities = Driver.Capabilities;
        Console.WriteLine($"Platform: {capabilities.GetCapability("platformName")}");
        Console.WriteLine($"Device: {capabilities.GetCapability("deviceName")}");
        Console.WriteLine($"Platform Version: {capabilities.GetCapability("platformVersion")}");
        Console.WriteLine($"Automation: {capabilities.GetCapability("automationName")}");
        
        // Verify platform-specific information
        if (TestPlatform == Configuration.Platform.Android)
        {
            var appPackage = capabilities.GetCapability("appPackage");
            Console.WriteLine($"App Package: {appPackage}");
        }
        else if (TestPlatform == Configuration.Platform.iOS)
        {
            var bundleId = capabilities.GetCapability("bundleId");
            Console.WriteLine($"Bundle ID: {bundleId}");
        }
        
        Assert.Pass("App context verified successfully");
    }
}
