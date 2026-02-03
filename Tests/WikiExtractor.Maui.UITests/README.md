# WikiExtractor MAUI UI Automation Tests

This project contains automated UI tests for the WikiExtractor MAUI application using Appium framework.

## Overview

The test framework is designed to:
- Test both Android and iOS platforms with the same test code
- Use Appium for cross-platform UI automation
- Take screenshots during test execution
- Support both emulators/simulators and real devices

## Prerequisites

### Required Software
1. **Appium Server** (v2.x)
   ```bash
   npm install -g appium
   ```

2. **Appium Drivers**
   ```bash
   # For Android
   appium driver install uiautomator2
   
   # For iOS
   appium driver install xcuitest
   ```

3. **Android Setup** (for Android testing)
   - Android SDK
   - Android Emulator or real device
   - Enable Developer Options and USB Debugging on device

4. **iOS Setup** (for iOS testing)
   - Xcode (macOS only)
   - iOS Simulator or real device
   - WebDriverAgent (installed automatically by xcuitest driver)

## Configuration

### appsettings.json

Update the configuration file with your app details:

```json
{
  "Android": {
    "AppPath": "path/to/your/app.apk",
    "AppPackage": "com.your.package.name",
    "AppActivity": "crc64...MainActivity"
  },
  "iOS": {
    "AppPath": "path/to/your/app.app",
    "BundleId": "com.your.bundle.id"
  }
}
```

### Finding Android Package and Activity

To find your Android app's package and activity:
```bash
aapt dump badging path/to/your/app.apk | grep package
aapt dump badging path/to/your/app.apk | grep launchable-activity
```

Or if app is installed:
```bash
adb shell pm list packages | grep your.app
adb shell dumpsys package your.package.name | grep -A 1 MAIN
```

## Running Tests

### 1. Start Appium Server
```bash
appium
```

### 2. Build Your MAUI App

**For Android:**
```bash
dotnet build -t:Run -f net8.0-android -c Release
# Or create APK
dotnet publish -f net8.0-android -c Release
```

**For iOS:**
```bash
dotnet build -t:Run -f net8.0-ios -c Release
# Or create IPA
dotnet publish -f net8.0-ios -c Release
```

### 3. Update Configuration

Edit `appsettings.json` with the path to your built app (APK or APP file).

### 4. Run Tests

**Test on Android:**
```bash
# Set environment variable for platform
set TEST_PLATFORM=Android
dotnet test
```

**Test on iOS:**
```bash
# Set environment variable for platform (macOS)
export TEST_PLATFORM=iOS
dotnet test
```

**Run specific test:**
```bash
dotnet test --filter "Test_LaunchAndCloseApp"
```

**Run by category:**
```bash
dotnet test --filter "Category=Smoke"
```

## Project Structure

```
WikiExtractor.Maui.UITests/
├── Base/
│   └── BaseTest.cs              # Base test class with setup/teardown
├── Configuration/
│   ├── Platform.cs              # Platform enum
│   └── TestConfiguration.cs     # Configuration management
├── Drivers/
│   └── AppiumDriverFactory.cs   # Driver creation and initialization
├── Helpers/
│   └── ScreenshotHelper.cs      # Screenshot utilities
├── Tests/
│   └── BasicAppLaunchTests.cs   # Basic smoke tests
├── appsettings.json             # Test configuration
└── WikiExtractor.Maui.UITests.csproj
```

## Test Features

### Current Tests

1. **Test_LaunchAndCloseApp**
   - Launches the app
   - Takes screenshots
   - Verifies app is running
   - Closes the app gracefully

2. **Test_MultipleLaunchCycles**
   - Verifies app can be launched multiple times

3. **Test_VerifyAppContext**
   - Verifies app context and capabilities
   - Logs platform information

### Screenshot Management

Screenshots are automatically saved to:
- `Screenshots/Android/` for Android tests
- `Screenshots/iOS/` for iOS tests

Screenshots are taken:
- During test execution when `TakeScreenshot()` is called
- Automatically when a test fails

## Adding New Tests

1. Create a new test class inheriting from `BaseTest`
2. Use NUnit attributes: `[Test]`, `[Category]`, `[Description]`
3. Access the driver via `Driver` property
4. Use `TakeScreenshot()` to capture screens
5. Use platform-agnostic selectors when possible

Example:
```csharp
[TestFixture]
public class MyNewTests : BaseTest
{
    [Test]
    [Category("Feature")]
    public void Test_MyFeature()
    {
        // Your test code here
        TakeScreenshot("FeatureScreen");
        Assert.Pass();
    }
}
```

## Troubleshooting

### Appium Server Issues
- Ensure Appium server is running on port 4723
- Check drivers are installed: `appium driver list`

### Android Issues
- Verify emulator/device is running: `adb devices`
- Check app is installed: `adb shell pm list packages`
- Enable USB debugging on device

### iOS Issues
- Ensure Xcode is installed (macOS only)
- Accept Xcode license: `sudo xcodebuild -license accept`
- Simulator must be running before test starts
- WebDriverAgent must be properly configured

### Connection Timeouts
- Increase timeout values in `appsettings.json`
- Ensure device/emulator has enough resources

## Next Steps

Future enhancements to add:
- Page Object Model implementation
- Navigation tests between screens
- UI element interaction tests
- Data-driven tests
- Parallel test execution
- CI/CD integration
- Video recording of test execution

## CI/CD Integration

To run in CI/CD pipelines:
1. Install Appium and drivers in CI environment
2. Set up emulators/simulators
3. Set environment variables
4. Run tests with appropriate platform parameter

Example for Azure DevOps:
```yaml
- script: |
    npm install -g appium
    appium driver install uiautomator2
  displayName: 'Install Appium'

- script: |
    dotnet test --logger trx --results-directory $(Build.ArtifactStagingDirectory)/TestResults
  env:
    TEST_PLATFORM: Android
  displayName: 'Run Android Tests'
```

## License

Same as the main WikiExtractor project.
