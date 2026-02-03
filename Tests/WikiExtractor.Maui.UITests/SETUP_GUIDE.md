# Complete Setup Guide for MAUI UI Automation Tests

This guide provides step-by-step instructions to set up and run automated UI tests for the WikiExtractor MAUI application.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Installation Steps](#installation-steps)
3. [Android Setup](#android-setup)
4. [iOS Setup](#ios-setup-macos-only)
5. [Appium Setup](#appium-setup)
6. [Project Configuration](#project-configuration)
7. [Building the MAUI App](#building-the-maui-app)
8. [Running the Tests](#running-the-tests)
9. [Running Tests from IDE](#running-tests-from-ide)
10. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

| Software | Purpose | Download Link |
|----------|---------|---------------|
| **.NET 8 SDK** | Build and run MAUI apps | https://dotnet.microsoft.com/download/dotnet/8.0 |
| **Node.js (LTS)** | Run Appium server | https://nodejs.org/ |
| **Visual Studio 2022** or **JetBrains Rider** | IDE (optional but recommended) | https://visualstudio.microsoft.com/<br>https://www.jetbrains.com/rider/ |
| **Android Studio** | Android development (for Android tests) | https://developer.android.com/studio |
| **Xcode** | iOS development (macOS only, for iOS tests) | https://apps.apple.com/app/xcode/id497799835 |

### Verify .NET Installation

```bash
dotnet --version
# Should output: 8.0.x or higher
```

### Verify Node.js Installation

```bash
node --version
npm --version
# Node: v18.x or higher recommended
# npm: v9.x or higher
```

---

## Installation Steps

### Step 1: Install Appium

Open a terminal/command prompt and run:

```bash
npm install -g appium
```

Verify installation:
```bash
appium --version
# Should output: 2.x.x
```

### Step 2: Install Appium Drivers

**For Android Testing:**
```bash
appium driver install uiautomator2
```

**For iOS Testing (macOS only):**
```bash
appium driver install xcuitest
```

**Verify drivers:**
```bash
appium driver list
```

You should see installed drivers marked with `✓ INSTALLED`.

---

## Android Setup

### 1. Install Android Studio

Download and install Android Studio from: https://developer.android.com/studio

During installation, ensure these components are selected:
- Android SDK
- Android SDK Platform
- Android Virtual Device (AVD)

### 2. Configure Environment Variables

**Windows:**
```powershell
# Open System Properties > Advanced > Environment Variables
# Add new System Variable:
ANDROID_HOME=C:\Users\<YourUsername>\AppData\Local\Android\Sdk

# Add to Path:
%ANDROID_HOME%\platform-tools
%ANDROID_HOME%\tools
%ANDROID_HOME%\emulator
```

**macOS/Linux:**
```bash
# Add to ~/.bashrc or ~/.zshrc
export ANDROID_HOME=$HOME/Library/Android/sdk
export PATH=$PATH:$ANDROID_HOME/platform-tools
export PATH=$PATH:$ANDROID_HOME/tools
export PATH=$PATH:$ANDROID_HOME/emulator
```

Apply changes:
```bash
source ~/.bashrc  # or source ~/.zshrc
```

### 3. Verify Android Setup

```bash
adb --version
# Should display ADB version

adb devices
# Should list connected devices/emulators
```

### 4. Create Android Virtual Device (Emulator)

1. Open Android Studio
2. Go to **Tools** > **Device Manager**
3. Click **Create Device**
4. Select a device (e.g., Pixel 5)
5. Download and select a system image (e.g., Android 13.0 - API 33)
6. Click **Finish**

**Start Emulator from Command Line:**
```bash
# List available emulators
emulator -list-avds

# Start an emulator
emulator -avd <emulator_name>
```

### 5. Enable Developer Options on Real Device (Optional)

If using a physical Android device:
1. Go to **Settings** > **About Phone**
2. Tap **Build Number** 7 times
3. Go back to **Settings** > **System** > **Developer Options**
4. Enable **USB Debugging**
5. Connect device via USB and authorize the computer

---

## iOS Setup (macOS Only)

### 1. Install Xcode

Download from Mac App Store: https://apps.apple.com/app/xcode/id497799835

**Minimum version**: Xcode 14.0 or higher

### 2. Install Xcode Command Line Tools

```bash
xcode-select --install
```

### 3. Accept Xcode License

```bash
sudo xcodebuild -license accept
```

### 4. Install Additional Tools

**Carthage (recommended):**
```bash
brew install carthage
```

**iOS Deploy (for real devices):**
```bash
npm install -g ios-deploy
```

### 5. Configure Simulators

**List available simulators:**
```bash
xcrun simctl list devices
```

**Boot a simulator:**
```bash
xcrun simctl boot "iPhone 15"
```

Or open Simulator.app:
```bash
open -a Simulator
```

### 6. Real Device Setup (Optional)

1. Connect iOS device via USB
2. Open Xcode
3. Go to **Window** > **Devices and Simulators**
4. Select your device and click **Trust**
5. Enter device passcode when prompted

**Enable UI Automation:**
1. On device: **Settings** > **Developer**
2. Enable **UI Automation**

---

## Appium Setup

### 1. Start Appium Server

Open a terminal and run:

```bash
appium
```

You should see output like:
```
[Appium] Welcome to Appium v2.x.x
[Appium] Appium REST http interface listener started on 0.0.0.0:4723
```

**Keep this terminal running** while executing tests.

**Alternative: Start with custom configuration:**
```bash
appium --port 4723 --base-path / --allow-cors
```

### 2. Verify Appium is Running

Open another terminal:
```bash
curl http://localhost:4723/status
```

Should return JSON with server status.

---

## Project Configuration

### 1. Navigate to Test Project

```bash
cd c:\Git\peterrexj\WikiExtractor\Tests\WikiExtractor.Maui.UITests
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Build Test Project

```bash
dotnet build
```

### 4. Configure App Settings

Edit `appsettings.json` file:

**For Android:**
```json
{
  "Android": {
    "AppPath": "C:/path/to/your/app.apk",
    "AppPackage": "com.yourcompany.wikiextractor",
    "AppActivity": "crc64...MainActivity",
    "PlatformVersion": "13.0",
    "DeviceName": "Android Emulator"
  }
}
```

**For iOS:**
```json
{
  "iOS": {
    "AppPath": "/path/to/your/app.app",
    "BundleId": "com.yourcompany.wikiextractor",
    "PlatformVersion": "17.2",
    "DeviceName": "iPhone 15"
  }
}
```

---

## Building the MAUI App

### For Android

**Build APK:**
```bash
cd c:\Git\peterrexj\WikiExtractor\src

# Build in Release mode
dotnet build -f net8.0-android -c Release

# Or publish to generate APK
dotnet publish -f net8.0-android -c Release
```

**Locate the APK:**
```
bin/Release/net8.0-android/publish/com.yourcompany.wikiextractor-Signed.apk
```

**Find App Package and Activity:**
```bash
# Using aapt (Android Asset Packaging Tool)
aapt dump badging path/to/app.apk | grep package
aapt dump badging path/to/app.apk | grep launchable-activity

# Or if app is installed on device:
adb shell pm list packages | grep wiki
adb shell dumpsys package com.yourpackage | grep -A 1 MAIN
```

### For iOS (macOS Only)

**Build APP:**
```bash
cd /path/to/WikiExtractor/src

# Build for simulator
dotnet build -f net8.0-ios -c Release /p:RuntimeIdentifier=iossimulator-x64

# Build for device (requires provisioning profile)
dotnet build -f net8.0-ios -c Release /p:RuntimeIdentifier=ios-arm64
```

**Locate the APP:**
```
bin/Release/net8.0-ios/iossimulator-x64/YourApp.app
```

**Find Bundle ID:**
```bash
# From Info.plist
/usr/libexec/PlistBuddy -c "Print CFBundleIdentifier" YourApp.app/Info.plist
```

### Update appsettings.json with Paths

After building, update `appsettings.json` with the actual paths to your built app files.

---

## Running the Tests

### Prerequisites Checklist

Before running tests, ensure:
- [ ] Appium server is running (`appium` command in terminal)
- [ ] Emulator/Simulator is running or real device is connected
- [ ] MAUI app is built (APK/APP file exists)
- [ ] `appsettings.json` is configured with correct paths
- [ ] For Android: `adb devices` shows your device
- [ ] For iOS: Simulator is booted or device is connected

### Run All Tests

**For Android:**
```bash
cd c:\Git\peterrexj\WikiExtractor\Tests\WikiExtractor.Maui.UITests

# Set platform environment variable
set TEST_PLATFORM=Android
dotnet test
```

**For iOS (macOS):**
```bash
cd /path/to/WikiExtractor/Tests/WikiExtractor.Maui.UITests

# Set platform environment variable
export TEST_PLATFORM=iOS
dotnet test
```

### Run Specific Tests

**Run single test:**
```bash
dotnet test --filter "Test_LaunchAndCloseApp"
```

**Run by category:**
```bash
dotnet test --filter "Category=Smoke"
```

**Run with detailed output:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Test Output

- **Test Results**: Console output
- **Screenshots**: `Screenshots/Android/` or `Screenshots/iOS/`
- **Test Reports**: `TestResults/` folder

---

## Running Tests from IDE

### Visual Studio 2022

1. **Open Solution:**
   - Open `WikiExtractor.Maui.UITests.csproj` in Visual Studio

2. **Configure Test Environment:**
   - Go to **Test** > **Test Settings**
   - Add environment variable:
     - Name: `TEST_PLATFORM`
     - Value: `Android` or `iOS`

3. **View Tests:**
   - Open **Test Explorer** (Test > Test Explorer)
   - Tests should appear automatically

4. **Run Tests:**
   - Right-click a test and select **Run**
   - Or click **Run All** button

5. **View Results:**
   - Check Test Explorer for pass/fail status
   - Screenshots in project's `Screenshots` folder

### JetBrains Rider

1. **Open Solution:**
   - Open `WikiExtractor.Maui.UITests.csproj` in Rider

2. **Configure Test Environment:**
   - Go to **Run** > **Edit Configurations**
   - Select test configuration
   - Add environment variable:
     - Name: `TEST_PLATFORM`
     - Value: `Android` or `iOS`

3. **View Tests:**
   - Open **Unit Tests** window (View > Tool Windows > Unit Tests)
   - Tests auto-discovered by NUnit

4. **Run Tests:**
   - Right-click test and select **Run**
   - Or use keyboard shortcut (Ctrl+T, R)

5. **Debug Tests:**
   - Right-click and select **Debug**
   - Set breakpoints in test code

### Important IDE Notes

⚠️ **Prerequisites still required when running from IDE:**
- Appium server must be running in a separate terminal
- Emulator/device must be running
- App must be built and configured in `appsettings.json`

If tests fail, check Appium server logs in the terminal.

---

## Troubleshooting

### Appium Server Not Running

**Error:** `Connection refused` or `Unable to connect to Appium`

**Solution:**
```bash
# Start Appium server
appium

# Check if running
curl http://localhost:4723/status
```

### Android Device Not Found

**Error:** `No devices found` or `Device not connected`

**Solution:**
```bash
# Check connected devices
adb devices

# If empty, start emulator
emulator -list-avds
emulator -avd <name>

# Or reconnect USB device
adb kill-server
adb start-server
```

### iOS Simulator Not Booting

**Error:** `Simulator not available`

**Solution:**
```bash
# List simulators
xcrun simctl list devices

# Boot simulator
xcrun simctl boot "iPhone 15"

# Or open Simulator app
open -a Simulator
```

### App Not Installing

**Error:** `Failed to install app`

**Android:**
```bash
# Manually install to verify APK is valid
adb install -r path/to/app.apk

# Check logcat for errors
adb logcat | grep -i error
```

**iOS:**
```bash
# Verify app bundle
codesign -dv --verbose=4 path/to/app.app

# Check bundle ID
/usr/libexec/PlistBuddy -c "Print CFBundleIdentifier" app.app/Info.plist
```

### WebDriverAgent Issues (iOS)

**Error:** `WebDriverAgent not running`

**Solution:**
```bash
# Reinstall xcuitest driver
appium driver uninstall xcuitest
appium driver install xcuitest

# Accept Xcode license
sudo xcodebuild -license accept
```

### Package/Activity Not Found (Android)

**Solution:**
```bash
# Find correct package name
adb shell pm list packages | grep -i wiki

# Find main activity
adb shell dumpsys package com.yourpackage | grep -A 1 "android.intent.action.MAIN"

# Or use aapt
aapt dump badging app.apk | grep package
aapt dump badging app.apk | grep launchable-activity
```

### Test Timeout

**Error:** `Test exceeded timeout`

**Solution:**
- Increase timeout in `appsettings.json`:
```json
{
  "AppiumServer": {
    "CommandTimeout": 300
  },
  "Android": {
    "NewCommandTimeout": 3000
  }
}
```

### Permission Denied Errors

**Android:**
```bash
# Grant permissions to app
adb shell pm grant com.yourpackage android.permission.WRITE_EXTERNAL_STORAGE
```

**macOS/Linux:**
```bash
# Make sure files are executable
chmod +x path/to/file
```

### Appium Version Conflicts

**Solution:**
```bash
# Check version
appium --version

# Update to latest
npm update -g appium

# Update drivers
appium driver update uiautomator2
appium driver update xcuitest
```

### .NET SDK Not Found

**Solution:**
```bash
# Verify installation
dotnet --version

# Install/update .NET SDK
# Download from: https://dotnet.microsoft.com/download
```

### Screenshots Not Saved

**Solution:**
- Check `Screenshots` folder exists and is writable
- Verify `TestSettings:ScreenshotPath` in `appsettings.json`
- Check test output for screenshot errors

---

## Useful Commands Reference

### Appium
```bash
appium                          # Start server
appium --version               # Check version
appium driver list             # List drivers
appium driver install <name>   # Install driver
appium driver update <name>    # Update driver
```

### Android (ADB)
```bash
adb devices                    # List devices
adb install app.apk           # Install app
adb uninstall <package>       # Uninstall app
adb shell pm list packages    # List installed apps
adb logcat                    # View logs
adb shell dumpsys package     # App info
emulator -list-avds           # List emulators
emulator -avd <name>          # Start emulator
```

### iOS
```bash
xcrun simctl list devices              # List simulators
xcrun simctl boot "iPhone 15"          # Boot simulator
xcrun simctl install booted app.app    # Install app
xcrun simctl uninstall booted <bundle> # Uninstall app
xcrun simctl shutdown all              # Shutdown all
ios-deploy --detect                    # Detect real devices
```

### .NET
```bash
dotnet build                   # Build project
dotnet test                    # Run tests
dotnet test --filter <name>   # Run specific test
dotnet clean                   # Clean build
```

---

## Additional Resources

### Documentation Links
- **Appium**: https://appium.io/docs/en/latest/
- **Appium Desktop**: https://github.com/appium/appium-desktop (GUI for Appium)
- **.NET MAUI**: https://learn.microsoft.com/dotnet/maui/
- **NUnit**: https://docs.nunit.org/
- **Android Debug Bridge**: https://developer.android.com/tools/adb
- **Xcode**: https://developer.apple.com/xcode/

### Community Support
- **Appium Forum**: https://discuss.appium.io/
- **Stack Overflow**: Tag `appium`, `maui`, or `.net-maui`

---

## Quick Start Checklist

Use this checklist to verify your setup:

- [ ] Node.js installed (`node --version`)
- [ ] .NET 8 SDK installed (`dotnet --version`)
- [ ] Appium installed (`appium --version`)
- [ ] Appium drivers installed (`appium driver list`)
- [ ] Android SDK configured (for Android tests)
- [ ] Xcode installed (for iOS tests, macOS only)
- [ ] Environment variables set (ANDROID_HOME, etc.)
- [ ] Emulator/Simulator or device ready
- [ ] MAUI app built (APK or APP file)
- [ ] `appsettings.json` configured
- [ ] Test project builds successfully (`dotnet build`)
- [ ] Appium server running
- [ ] First test executes successfully

---

**For questions or issues, check the Troubleshooting section or refer to the main README.md**
