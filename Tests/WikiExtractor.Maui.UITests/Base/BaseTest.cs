using NUnit.Framework;
using OpenQA.Selenium.Appium;
using WikiExtractor.Maui.UITests.Configuration;
using WikiExtractor.Maui.UITests.Drivers;
using WikiExtractor.Maui.UITests.Helpers;

namespace WikiExtractor.Maui.UITests.Base;

public abstract class BaseTest
{
    protected AppiumDriver? Driver { get; private set; }
    protected Platform TestPlatform { get; private set; }
    protected ScreenshotHelper? ScreenshotHelper { get; private set; }
    
    private AppiumDriverFactory? _driverFactory;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Get platform from environment variable or test parameter
        var platformEnv = Environment.GetEnvironmentVariable("TEST_PLATFORM");
        TestPlatform = string.IsNullOrEmpty(platformEnv) 
            ? Platform.Android // Default to Android
            : Enum.Parse<Platform>(platformEnv, true);

        Console.WriteLine($"Test Platform: {TestPlatform}");
    }

    [SetUp]
    public void SetUp()
    {
        try
        {
            _driverFactory = new AppiumDriverFactory();
            Driver = _driverFactory.CreateDriver(TestPlatform);
            ScreenshotHelper = new ScreenshotHelper(Driver, TestPlatform);
            
            Console.WriteLine($"Driver initialized successfully for {TestPlatform}");
            
            // Give the app some time to launch
            Thread.Sleep(3000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize driver: {ex.Message}");
            throw;
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            // Take screenshot on failure
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                TakeScreenshot($"FAILED_{TestContext.CurrentContext.Test.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to take failure screenshot: {ex.Message}");
        }
        finally
        {
            try
            {
                Driver?.Quit();
                Driver?.Dispose();
                Console.WriteLine("Driver closed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to close driver: {ex.Message}");
            }
        }
    }

    protected void TakeScreenshot(string name)
    {
        try
        {
            ScreenshotHelper?.TakeScreenshot(name, TestContext.CurrentContext.Test.Name);
            Console.WriteLine($"Screenshot taken: {name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to take screenshot: {ex.Message}");
        }
    }

    protected void WaitForSeconds(int seconds)
    {
        Thread.Sleep(seconds * 1000);
    }
}
