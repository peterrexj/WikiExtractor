# Developer Setup Guide — WikiExtractor MAUI Apps

Covers setup for all four apps: **Maui.Popes**, **Maui.Saints**, **Maui.Countries**, **Maui.WorldLeaders**.

---

## Version Reference

| Component | Version |
|---|---|
| .NET SDK | **10.0.302** (pinned in `global.json`) |
| MAUI Workload | **10.0.20** (installed) |
| MAUI NuGet (app csproj) | **10.0.90** (overrides workload) |
| Target framework — Android | `net10.0-android36.0` |
| Target framework — iOS | `net10.0-ios26.5` |
| Library target framework | `net9.0-android36.0` / `net9.0-ios18.0` |
| Android API level | 36 (min supported: 21 / Android 5.0) |
| iOS minimum deployment | 15.0 |
| Xcode (Mac only) | **26.5** (Build 17F42) |
| Java (Android builds) | **21** (OpenJDK 21) |

---

## Mac Setup

### 1. Install .NET SDK 10.0.302

Download from [https://dot.net](https://dot.net) and install the **SDK** (not the runtime-only installer).

Verify:

```bash
dotnet --version
# expected: 10.0.302
```

The repo pins to this exact version via `global.json`. Using a different SDK version will cause build errors.

### 2. Install MAUI Workload

```bash
sudo dotnet workload install maui
```

To update an existing install to the latest patch:

```bash
sudo dotnet workload update
```

Verify:

```bash
dotnet workload list
# maui   10.0.20/10.0.100   SDK 10.0.300
```

> **Note:** The installed workload version is **10.0.20** but the apps override MAUI NuGet packages to **10.0.90** via `<MauiVersion>10.0.90</MauiVersion>` in each app `.csproj`. This version bump is required for iOS 26 compatibility — **10.0.20 crashes on iOS 26 with a `UIVisualEffectView` SIGSEGV**. The NuGet override takes precedence automatically; no workload reinstall is needed.

### 3. Install Xcode 26.5

- Install **Xcode 26.5** (Build 17F42) from the Mac App Store or the [Apple Developer portal](https://developer.apple.com/download/).
- After install, accept the license:

```bash
sudo xcodebuild -license accept
```

- Install command-line tools:

```bash
xcode-select --install
```

Verify:

```bash
xcodebuild -version
# Xcode 26.5
# Build version 17F42
```

> The project sets `<ValidateXcodeVersion>false</ValidateXcodeVersion>` in each iOS csproj, which suppresses the MAUI workload's Xcode version check. This is required because the installed MAUI workload (10.0.20) predates Xcode 26 and would otherwise refuse to build. The NuGet packages at 10.0.90 are fully compatible.

### 4. Install Java 21 (Android builds on Mac)

The Android run scripts explicitly require Java 21 at `/opt/homebrew/opt/openjdk@21`.

```bash
brew install openjdk@21
```

Add to your shell profile (`~/.zshrc` or `~/.bashrc`):

```bash
export JAVA_HOME=/opt/homebrew/opt/openjdk@21
export PATH="$JAVA_HOME/bin:$PATH"
```

Reload:

```bash
source ~/.zshrc
java -version
# openjdk version "21.0.x"
```

> Java 21 is required for the Android manifest merger (d8/r8). Java 17 may work but is untested. Java 23+ breaks the Android build tools.

### 5. Install Android SDK (Mac)

The Android SDK is not bundled with the .NET workload. Install via [Android Studio](https://developer.android.com/studio) (recommended) or the SDK command-line tools.

Required SDK components — open Android Studio → SDK Manager and install:

| Component | Required version |
|---|---|
| Android SDK Platform | **API 36** |
| Android SDK Build-Tools | **36.x** |
| Android SDK Platform-Tools | Latest |
| Android Emulator | Latest |
| Android System Image | `API 36` — `Google APIs ARM 64 v8a` (Apple Silicon) |

Set the environment variable (add to `~/.zshrc`):

```bash
export ANDROID_HOME="$HOME/Library/Android/sdk"
export PATH="$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator:$PATH"
```

Verify:

```bash
adb version
# Android Debug Bridge version 1.x
```

### 6. Create Android Virtual Device (AVD)

The Android run scripts expect these AVD names by default:

| Type | AVD name |
|---|---|
| Phone (default) | `Medium_Phone_API_36.1` |
| Tablet | `Medium_Tablet_API_36.1` |

Create them via **Android Studio → Device Manager → Create Virtual Device**:

1. Select hardware profile: `Medium Phone` → click Next
2. Select system image: `API Level 36`, `Google APIs`, `ARM 64 v8a` → Download if needed
3. Name the AVD exactly `Medium_Phone_API_36.1` → Finish
4. Repeat with `Medium Tablet` → name `Medium_Tablet_API_36.1`

Alternatively create via command line:

```bash
$ANDROID_HOME/cmdline-tools/latest/bin/avdmanager create avd \
  -n "Medium_Phone_API_36.1" \
  -k "system-images;android-36;google_apis;arm64-v8a" \
  -d "medium_phone"
```

### 7. Create iOS Simulator

The iOS run scripts target **iPhone 17 Pro** on **iOS 26.5**. Create it if it does not already exist:

```bash
xcrun simctl create "iPhone 17 Pro" \
  "com.apple.CoreSimulator.SimDeviceType.iPhone-17-Pro" \
  "com.apple.CoreSimulator.SimRuntime.iOS-26-5"
```

Verify the simulator appears:

```bash
xcrun simctl list devices available | awk '/-- iOS 26\.5 --/{f=1;next} /^--/{f=0} f' | grep "iPhone 17 Pro"
```

### 8. Syncfusion License

Syncfusion controls require a valid license key. The key is registered in `App.xaml.cs` of each app:

```csharp
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR_KEY");
```

Each developer must obtain their own key from [https://www.syncfusion.com](https://www.syncfusion.com) (a community license is free for small teams/individuals). Without a valid key the app will launch but display a license watermark over the UI.

### 9. Clone and Restore

```bash
git clone <repo-url>
cd WikiExtractor

# Restore all NuGet packages
dotnet restore src/Maui/Apps/Maui.Popes/Maui.Popes.csproj
# Repeat for Saints, Countries, WorldLeaders as needed
```

---

## Windows Setup

iOS builds are **not possible on Windows** — iOS compilation and simulator requires macOS and Xcode. Windows can build and run the Android target only.

### 1. Install .NET SDK 10.0.302

Download from [https://dot.net](https://dot.net) — install the **SDK** installer.

Verify in a new terminal (PowerShell or CMD):

```powershell
dotnet --version
# 10.0.302
```

### 2. Install MAUI Workload

```powershell
dotnet workload install maui-android
```

Installing `maui-android` only avoids pulling iOS/Mac Catalyst tooling that cannot be used on Windows anyway.

Verify:

```powershell
dotnet workload list
# maui-android   10.0.20/10.0.100
```

### 3. Install Java 21

Download OpenJDK 21 from [https://adoptium.net](https://adoptium.net) (Temurin distribution recommended).

During installation, select the option to **set JAVA_HOME** automatically.

Verify in a new terminal:

```powershell
java -version
# openjdk version "21.x.x"
$env:JAVA_HOME
# C:\Program Files\Eclipse Adoptium\jdk-21.x.x-hotspot (or similar)
```

If `JAVA_HOME` is not set by the installer, set it manually:

```powershell
[System.Environment]::SetEnvironmentVariable("JAVA_HOME", "C:\Program Files\Eclipse Adoptium\jdk-21.0.9.9-hotspot", "Machine")
```

### 4. Install Android SDK

Install via [Android Studio for Windows](https://developer.android.com/studio).

Open Android Studio → SDK Manager → install:

| Component | Required version |
|---|---|
| Android SDK Platform | **API 36** |
| Android SDK Build-Tools | **36.x** |
| Android SDK Platform-Tools | Latest |
| Android Emulator | Latest |
| Android System Image | `API 36` — `Google APIs x86_64` (Intel/AMD) or `ARM 64 v8a` (Snapdragon X / ARM Windows) |

Set environment variables (System Properties → Advanced → Environment Variables):

```
ANDROID_HOME = C:\Users\<you>\AppData\Local\Android\Sdk
ANDROID_SDK_ROOT = C:\Users\<you>\AppData\Local\Android\Sdk
```

Add to `PATH`:

```
%ANDROID_HOME%\platform-tools
%ANDROID_HOME%\emulator
```

### 5. Create Android Virtual Device (AVD)

Same as Mac — use Android Studio Device Manager to create:

| Type | AVD name |
|---|---|
| Phone (default) | `Medium_Phone_API_36.1` |
| Tablet | `Medium_Tablet_API_36.1` |

Select `x86_64` system image for best emulator performance on Intel/AMD machines. ARM64 image if on a Snapdragon X machine.

### 6. Enable PowerShell Script Execution

The Android run scripts are `.ps1` files. By default Windows blocks unsigned scripts. Run once as Administrator:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### 7. Syncfusion License

Same as Mac — update `App.xaml.cs` in each app with your key before building.

### 8. Clone and Restore

```powershell
git clone <repo-url>
cd WikiExtractor
dotnet restore src\Maui\Apps\Maui.Popes\Maui.Popes.csproj
```

---

## Running the Apps

### iOS (Mac only)

Each app has its own shell script. Run from the app's directory:

```bash
cd src/Maui/Apps/Maui.Popes
./run-ios-popes.sh                              # iPhone 17 Pro (default)
./run-ios-popes.sh --ipad                       # iPad Pro 13-inch (M5)
./run-ios-popes.sh --device "iPhone 16"         # any named simulator
./run-ios-popes.sh --clean                      # clean build first
```

| App | Script |
|---|---|
| Popes | `run-ios-popes.sh` |
| Saints | `run-ios-saints.sh` |
| Countries | `run-ios-countries.sh` |
| WorldLeaders | `run-ios-worldleaders.sh` |

The scripts will:
1. Find the iOS 26.5 simulator matching the device name (iOS version section is filtered by `awk` to avoid ambiguous matches with older runtimes).
2. Boot the simulator if not already running.
3. Build with `dotnet build -f net10.0-ios26.5 -c Debug`.
4. Install the `.app` bundle via `xcrun simctl install`.
5. Terminate any existing instance and launch the new one.

### Android (Mac and Windows)

Each app has its own PowerShell script. On Mac, run with `pwsh`; on Windows, run with `pwsh` or the standard PowerShell terminal.

```powershell
cd src/Maui/Apps/Maui.Popes
pwsh ./run-android-popes.ps1                    # Medium_Phone_API_36.1 (default)
pwsh ./run-android-popes.ps1 -Tablet            # Medium_Tablet_API_36.1 (Saints/Countries/WorldLeaders only)
pwsh ./run-android-popes.ps1 -Avd "Pixel_9_API_36"  # any named AVD
```

| App | Script |
|---|---|
| Popes | `run-android-popes.ps1` |
| Saints | `run-android-saints.ps1` |
| Countries | `run-android-countries.ps1` |
| WorldLeaders | `run-android-worldleaders.ps1` |

The scripts will:
1. Locate the Android SDK (checks `ANDROID_HOME`, `ANDROID_SDK_ROOT`, then common platform paths).
2. Resolve Java 21 at `/opt/homebrew/opt/openjdk@21` (Mac) or from `JAVA_HOME` (Windows).
3. Start the emulator if not already running and wait for full boot.
4. Build with `dotnet build -f net10.0-android36.0 -c Debug`.
5. Find the signed APK, force-stop any running instance, and install with `adb install -r`.
6. Detect the `MainActivity` from `pm dump` and launch with `adb shell am start`.

---

## Project Structure Notes

### Dual target-framework split

| Scope | Frameworks |
|---|---|
| App projects (Popes, Saints, Countries, WorldLeaders) | `net10.0-android36.0`, `net10.0-ios26.5` |
| Library projects (WikiExtractor.Maui.App, PjAds.Maui) | `net9.0-android36.0`, `net9.0-ios18.0` |

The libraries have not yet been migrated to net10. They are consumed by the net10 app projects without conflict because .NET 10 can reference .NET 9 libraries.

### MAUI version override pattern

`Directory.Build.props` at the repo root sets `<MauiVersion>9.0.100</MauiVersion>` for the library projects. Each app `.csproj` overrides this with its own `<PropertyGroup>` entry:

```xml
<MauiVersion>10.0.90</MauiVersion>
```

This override is what pins the apps to the iOS 26-compatible MAUI NuGet packages while leaving library projects unaffected.

### AndroidX package overrides

MAUI 10.0.90 requires higher AndroidX versions than the defaults. Each app `.csproj` explicitly pins:

```xml
<PackageReference Include="Xamarin.AndroidX.AppCompat"   Version="1.7.1.1" />
<PackageReference Include="Xamarin.AndroidX.Activity"    Version="1.10.1.3" />
<PackageReference Include="Xamarin.AndroidX.SavedState"  Version="1.3.1.1" />
```

Do not downgrade these — MAUI 10.0.90 will produce NU1605 restore errors.

### `ValidateXcodeVersion`

All four iOS csproj files set:

```xml
<ValidateXcodeVersion>false</ValidateXcodeVersion>
```

This is needed because the installed MAUI workload manifest (10.0.20) does not recognise Xcode 26 as a supported version. The `false` flag bypasses that check. The MAUI NuGet packages at 10.0.90 have full Xcode 26 support.

---

## Android Bundle IDs

| App | Android Bundle ID | iOS Bundle ID |
|---|---|---|
| Popes | `com.pj.popesofchurch` | `com.pj.popesofchurch` |
| Saints | `com.pj.christiancatholicsaints` | `com.pj.ChristianCatholicSaints` |
| Countries | `com.pj.countries` | `com.pj.countriesofworld` |
| WorldLeaders | `com.pj.worldleadershub` | `com.pj.worldleadershub` |

> Note: Saints and Countries have **different** bundle IDs on Android vs iOS — this is intentional for store listings but means the run scripts must use the correct platform-specific ID.

---

## Release Builds

### Android

Release APKs are built as AAB (Android App Bundle) format. The keystore lives at:

```
Resources/droidCerts/catholicsaints.keystore
```

Build:

```bash
dotnet build Maui.Popes.csproj -f net10.0-android36.0 -c Release
```

Signing credentials are embedded in the Release `PropertyGroup` of each `.csproj`.

### iOS

iOS release targets `ios-arm64` (device, not simulator). Requires:
- An Apple Developer account
- A valid distribution certificate (`Apple Distribution: Peter Joseph (5PNCUV7LZ5)`)
- Automatic provisioning (`CodesignProvision=Automatic:Distribution`)

Build:

```bash
dotnet build Maui.Popes.csproj -f net10.0-ios26.5 -c Release
```

`<ArchiveOnBuild>true</ArchiveOnBuild>` is set, so the Release build produces an `.xcarchive` suitable for App Store upload.

---

## Troubleshooting

### iOS simulator: `Unable to lookup in current state: Shutdown`

The simulator is not booted. The run script boots it automatically, but if running manually:

```bash
xcrun simctl boot <SIMULATOR_ID>
open -a Simulator
```

### iOS: App launches then crashes immediately (SIGSEGV in UIVisualEffectView)

This is the iOS 26 "Liquid Glass" crash. It means the app was built against MAUI 10.0.20 (the workload default). Confirm `<MauiVersion>10.0.90</MauiVersion>` is present in the app `.csproj` and rebuild with `--no-incremental` or `--clean`.

### Android: `Could not determine MainActivity`

The run script uses `adb shell pm dump <bundle-id>` to find the launch activity. This fails if the bundle ID in the `.ps1` script does not match what was installed. Verify the `$BundleId` variable matches the `ApplicationId` in the `.csproj` exactly (including case).

### Android build: `NU1605 — downgrade detected`

MAUI 10.0.90 requires AndroidX versions higher than the MAUI workload defaults. Ensure all three packages are pinned at the correct versions in the app `.csproj` (see AndroidX section above). Running `dotnet restore --force` after updating clears the resolved dependency cache.

### `JAVA_HOME` not found (Mac)

The Android run scripts check `/opt/homebrew/opt/openjdk@21` first. If Java is installed elsewhere:

```bash
export JAVA_HOME=/path/to/your/jdk21
```

Or install via Homebrew: `brew install openjdk@21`.

### `dotnet workload update` prompts for a password

This requires elevated permissions. Run it directly in a terminal with `sudo`:

```bash
sudo dotnet workload update
```

It cannot be run from within an IDE terminal that does not have a TTY for password input.

### Syncfusion watermark visible in the app

The Syncfusion license key in `App.xaml.cs` is either missing or does not match the installed version (33.2.6). Obtain a key that covers version 33.x from the Syncfusion portal and update `RegisterLicense(...)`.
