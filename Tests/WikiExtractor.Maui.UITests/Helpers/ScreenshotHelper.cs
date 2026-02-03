using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using WikiExtractor.Maui.UITests.Configuration;
using Platform = WikiExtractor.Maui.UITests.Configuration.Platform;

namespace WikiExtractor.Maui.UITests.Helpers;

public class ScreenshotHelper
{
    private readonly AppiumDriver _driver;
    private readonly Platform _platform;
    private readonly string _screenshotBasePath;

    public ScreenshotHelper(AppiumDriver driver, Platform platform)
    {
        _driver = driver;
        _platform = platform;
        _screenshotBasePath = TestConfiguration.Instance.ScreenshotPath;
        EnsureScreenshotDirectory();
    }

    private void EnsureScreenshotDirectory()
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _screenshotBasePath, _platform.ToString());
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    public void TakeScreenshot(string name, string? testName = null)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var sanitizedName = SanitizeFileName(name);
            var fileName = $"{timestamp}_{sanitizedName}.png";
            
            if (!string.IsNullOrEmpty(testName))
            {
                var sanitizedTestName = SanitizeFileName(testName);
                fileName = $"{timestamp}_{sanitizedTestName}_{sanitizedName}.png";
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _screenshotBasePath, _platform.ToString(), fileName);
            
            var screenshot = _driver.GetScreenshot();
            screenshot.SaveAsFile(fullPath);
            
            Console.WriteLine($"Screenshot saved: {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save screenshot: {ex.Message}");
        }
    }

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    }
}
