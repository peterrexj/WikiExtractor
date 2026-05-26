# .NET 10 Upgrade Plan

**When to do this**: After .NET 10 goes GA — expected November 2026.  
**Why**: Unlocks `Plugin.Firebase.Crashlytics` 4.0.0 on iOS, which resolves the
`GULNetworkInfo` symbol conflict that blocks Crashlytics from working on iOS today.

---

## Prerequisites

Before touching any code:

1. Install the .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
2. Install the latest Xcode (whatever ships alongside iOS 26 SDK)
3. Verify `dotnet --list-sdks` shows `10.x.x` alongside `9.x.x`
4. Update Visual Studio for Mac / Rider / VS Code MAUI extension to a version
   that supports .NET 10 MAUI workloads
5. Install the .NET 10 MAUI workload:
   ```
   dotnet workload install maui
   ```

---

## Step 1 — Update Target Frameworks in all 8 projects

Change every occurrence of the TFM strings. The pattern is consistent across all projects.

### TFM substitution table

| Old | New |
|-----|-----|
| `net9.0-android36.0` | `net10.0-android36.0` (or newer Android TFM if changed) |
| `net9.0-ios18.0` | `net10.0-ios18.0` (minimum iOS SDK; keep 18 unless you want to require 26) |
| `net9.0-maccatalyst18.0` | `net10.0-maccatalyst18.0` |
| `net9.0-maccatalyst` | `net10.0-maccatalyst` |
| `net9.0` (plain) | `net10.0` |

> **Note**: `net10.0-ios18.0` means your app runs on iOS 18+, built with the .NET 10
> toolchain. You do NOT need to change the iOS deployment target just because you're
> on .NET 10.

### Files to update

**App projects** (4 files — identical change in each):
- `src/Maui/Apps/Maui.Countries/Maui.Countries.csproj`
- `src/Maui/Apps/Maui.Saints/Maui.Saints.csproj`
- `src/Maui/Apps/Maui.WorldLeaders/Maui.WorldLeaders.csproj`
- `src/Maui/Apps/Maui.Popes/Maui.Popes.csproj`

Change in each:
```xml
<!-- Before -->
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">
    net9.0-android36.0;net9.0-ios18.0
</TargetFrameworks>
<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('windows'))">
    net9.0-android36.0;net9.0-ios18.0
</TargetFrameworks>

<!-- After -->
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">
    net10.0-android36.0;net10.0-ios18.0
</TargetFrameworks>
<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('windows'))">
    net10.0-android36.0;net10.0-ios18.0
</TargetFrameworks>
```

Also remove `<UseXcode16_3ForIOS>true</UseXcode16_3ForIOS>` from each — that was
a workaround for net9 tooling; .NET 10 uses Xcode natively.

**Shared library** (`WikiExtractor.Maui.App/WikiExtractor.Maui.App.csproj`):
```xml
<!-- Before -->
<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('windows'))">
    net9.0-android36.0;net9.0-ios18.0;net9.0-maccatalyst18.0
</TargetFrameworks>

<!-- After -->
<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('windows'))">
    net10.0-android36.0;net10.0-ios18.0;net10.0-maccatalyst18.0
</TargetFrameworks>
```

Also update the platform-condition strings inside `<ItemGroup Condition="...">`:
```xml
<!-- Every place you see net9.0-android or net9.0-ios in Condition attributes -->
net9.0-android  →  net10.0-android
net9.0-ios      →  net10.0-ios
net9.0-maccatalyst → net10.0-maccatalyst
```

Specific `Condition` attributes to update in `WikiExtractor.Maui.App.csproj`:
- `$(TargetFramework.StartsWith('net9.0-android'))` → `net10.0-android`
- `$(TargetFramework.StartsWith('net9.0-ios'))` → `net10.0-ios`
- `$(TargetFramework.StartsWith('net9.0-maccatalyst'))` → `net10.0-maccatalyst`

**PjAds.Maui library** (`src/Maui/Library/PjAds.Maui/PjAds.Maui.csproj`):
Same TFM swap as the shared library. Also update the `<ItemGroup Condition>` strings
for `net9.0-android` and `net9.0-ios`.

**WikiExtractor.Maui.Core** (`src/Maui/Library/WikiExtractor.Maui.Core/WikiExtractor.Maui.Core.csproj`):
```xml
<!-- Before -->
<TargetFramework>net9.0</TargetFramework>

<!-- After -->
<TargetFramework>net10.0</TargetFramework>
```

**Maui.Samples** (`src/Maui/Apps/Maui.Samples/Maui.Samples.csproj`):
```xml
<!-- Before -->
<TargetFrameworks>net9.0-android36.0;net9.0-ios18.0;net9.0-maccatalyst</TargetFrameworks>

<!-- After -->
<TargetFrameworks>net10.0-android36.0;net10.0-ios18.0;net10.0-maccatalyst</TargetFrameworks>
```

---

## Step 2 — Update NuGet package versions

### Packages that need a version bump

| Package | Current | Target | Affected projects |
|---------|---------|--------|-------------------|
| `Plugin.Firebase.Crashlytics` | 3.1.1 | **4.0.0** | All 5 (4 apps + WikiExtractor.Maui.App) |
| `Microsoft.Extensions.Logging.Debug` | 9.0.0 | **10.0.0** | All projects that reference it |
| `Microsoft.Maui.Controls` | `$(MauiVersion)` | `$(MauiVersion)` | No change needed — resolves from SDK |
| `CommunityToolkit.Maui` | 9.0.0 | Check for 10.x release | All 4 app projects |

> **Note**: `Xamarin.AndroidX.*` and `Xamarin.GooglePlayServices.Ads` and
> `Xamarin.Google.iOS.MobileAds` are independent of .NET version — leave them
> as-is unless the build flags a conflict after upgrading.

### Firebase Crashlytics — re-enable on iOS

In all 4 app `.csproj` files, change the Condition from Android-only back to both platforms:
```xml
<!-- Before (current — Android only) -->
<PackageReference Include="Plugin.Firebase.Crashlytics" Version="3.1.1"
    Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'" />

<!-- After (.NET 10) -->
<PackageReference Include="Plugin.Firebase.Crashlytics" Version="4.0.0" />
```

In `WikiExtractor.Maui.App.csproj`, restore the iOS condition:
```xml
<!-- Before (current — Android only) -->
<PackageReference Include="Plugin.Firebase.Crashlytics" Version="3.1.1"
    Condition="$(TargetFramework.StartsWith('net10.0-android'))" />

<!-- After (.NET 10) -->
<PackageReference Include="Plugin.Firebase.Crashlytics" Version="4.0.0"
    Condition="$(TargetFramework.StartsWith('net10.0-android')) OR $(TargetFramework.StartsWith('net10.0-ios'))" />
```

---

## Step 3 — Re-enable Firebase on iOS in AppDelegate.cs (all 4 apps)

Add the using back and restore `CrossFirebase.Initialize()` in all 4 files:

- `src/Maui/Apps/Maui.Countries/Platforms/iOS/AppDelegate.cs`
- `src/Maui/Apps/Maui.Saints/Platforms/iOS/AppDelegate.cs`
- `src/Maui/Apps/Maui.WorldLeaders/Platforms/iOS/AppDelegate.cs`
- `src/Maui/Apps/Maui.Popes/Platforms/iOS/AppDelegate.cs`

In each file, add the using back:
```csharp
using Plugin.Firebase.Core.Platforms.iOS;
```

And restore the call at the top of `CreateMauiApp()`:
```csharp
protected override MauiApp CreateMauiApp()
{
    CrossFirebase.Initialize();   // <-- restore this
    MobileAds.SharedInstance.Start(...);
    ...
}
```

> Keep the try/catch wrapper around `CrossFirebase.Initialize()` until
> `GoogleService-Info.plist` has been added for each app (see `FIREBASE_SETUP.md`).

---

## Step 4 — Re-enable Crashlytics for iOS in ExceptionHandler.cs

File: `src/Maui/Library/WikiExtractor.Maui.App/Exts/ExceptionHandler.cs`

```csharp
// Before (current — Android only)
#if ANDROID && !DEBUG

// After (.NET 10 — both platforms)
#if (ANDROID || IOS) && !DEBUG
```

---

## Step 5 — Android MainActivity.cs (verify — no change expected)

The Android `CrossFirebase.Initialize(this)` calls in all 4 `MainActivity.cs` files
work correctly today and require no change. Verify they still compile after the TFM bump.

Files (no expected change needed):
- `src/Maui/Apps/Maui.Countries/Platforms/Android/MainActivity.cs`
- `src/Maui/Apps/Maui.Saints/Platforms/Android/MainActivity.cs`
- `src/Maui/Apps/Maui.WorldLeaders/Platforms/Android/MainActivity.cs`
- `src/Maui/Apps/Maui.Popes/Platforms/Android/MainActivity.cs`

---

## Step 6 — Add Firebase config files (if not done yet)

This is required for Crashlytics to actually report crashes — on both platforms.
See `FIREBASE_SETUP.md` for full instructions.

Quick reference for file placement:

| App | Android path | iOS path |
|-----|-------------|----------|
| Countries | `Maui.Countries/Platforms/Android/google-services.json` | `Maui.Countries/Platforms/iOS/GoogleService-Info.plist` |
| Saints | `Maui.Saints/Platforms/Android/google-services.json` | `Maui.Saints/Platforms/iOS/GoogleService-Info.plist` |
| WorldLeaders | `Maui.WorldLeaders/Platforms/Android/google-services.json` | `Maui.WorldLeaders/Platforms/iOS/GoogleService-Info.plist` |
| Popes | `Maui.Popes/Platforms/Android/google-services.json` | `Maui.Popes/Platforms/iOS/GoogleService-Info.plist` |

Build actions:
- `google-services.json` → **GoogleServicesJson**
- `GoogleService-Info.plist` → **BundleResource**

---

## Step 7 — Build and verify

```bash
# Clean first — TFM changes require a full clean
dotnet clean

# Restore packages
dotnet restore

# Build Android for one app to verify
dotnet build src/Maui/Apps/Maui.Saints/Maui.Saints.csproj \
    -f net10.0-android36.0 -c Debug

# Build iOS for one app to verify
dotnet build src/Maui/Apps/Maui.Saints/Maui.Saints.csproj \
    -f net10.0-ios18.0 -c Debug
```

Expected: 0 errors. The `NU1608` warnings about Lifecycle.Runtime may reappear —
these are version constraint warnings, not errors, and can be resolved by bumping
those AndroidX packages to whatever version the transitive tree now requires.

---

## Rollback plan

If .NET 10 introduces a blocking issue, reverting is straightforward:
1. Change all `net10.0-*` TFMs back to `net9.0-*`
2. Revert Firebase Crashlytics back to 3.1.1, Android-only condition
3. Revert `ExceptionHandler.cs` `#if` back to `ANDROID && !DEBUG`
4. Remove `CrossFirebase.Initialize()` from iOS AppDelegates
5. `dotnet restore && dotnet build`

Git commit the net9 state before starting the upgrade so you have a clean revert point.
