# Firebase Crashlytics Setup Guide

The code wiring is already done. This guide covers the Firebase Console steps and config file placement needed to activate crash reporting.

---

## Prerequisites

- A Google account
- Access to [console.firebase.google.com](https://console.firebase.google.com)

---

## Step 1 — Create a Firebase Project

1. Go to [console.firebase.google.com](https://console.firebase.google.com)
2. Click **Add project**
3. Name it (e.g. `WikiExtractor Apps`) — one project can host all 4 apps
4. Enable or disable Google Analytics (recommended: enable, it's free)
5. Click **Create project**

---

## Step 2 — Register Each App

Repeat this for all 4 apps. In the Firebase project overview, click **Add app**.

### Countries Insights

**Android:**
1. Click the Android icon
2. Android package name: `com.pj.countriesofworld`
3. App nickname: `Countries Insights`
4. Click **Register app**
5. Download `google-services.json`
6. Place it at: `src/Maui/Apps/Maui.Countries/Platforms/Android/google-services.json`
7. Click through the remaining steps (the SDK is already added)

**iOS:**
1. Click **Add app** again, then the iOS icon
2. iOS bundle ID: `com.pj.countriesofworld`
3. App nickname: `Countries Insights`
4. Click **Register app**
5. Download `GoogleService-Info.plist`
6. Place it at: `src/Maui/Apps/Maui.Countries/Platforms/iOS/GoogleService-Info.plist`
7. Click through the remaining steps

---

### Catholic Saints

**Android:**
- Package name: `com.peterrexj.christiancatholicsaints`
- Place `google-services.json` at: `src/Maui/Apps/Maui.Saints/Platforms/Android/google-services.json`

**iOS:**
- Bundle ID: `com.peterrexj.christiancatholicsaints`
- Place `GoogleService-Info.plist` at: `src/Maui/Apps/Maui.Saints/Platforms/iOS/GoogleService-Info.plist`

---

### World Leaders Hub

**Android:**
- Package name: `com.pj.worldleadershub`
- Place `google-services.json` at: `src/Maui/Apps/Maui.WorldLeaders/Platforms/Android/google-services.json`

**iOS:**
- Bundle ID: `com.pj.worldleadershub`
- Place `GoogleService-Info.plist` at: `src/Maui/Apps/Maui.WorldLeaders/Platforms/iOS/GoogleService-Info.plist`

---

### Popes

**Android:**
- Package name: `com.peterrexj.popesofchurch`
- Place `google-services.json` at: `src/Maui/Apps/Maui.Popes/Platforms/Android/google-services.json`

**iOS:**
- Bundle ID: `com.peterrexj.popesofchurch`
- Place `GoogleService-Info.plist` at: `src/Maui/Apps/Maui.Popes/Platforms/iOS/GoogleService-Info.plist`

---

## Step 3 — Enable Crashlytics in Firebase Console

For each app registered above:

1. In the Firebase project, go to **Crashlytics** (left sidebar under Release & Monitor)
2. Click **Enable Crashlytics**
3. Select each app from the dropdown and enable it

---

## Step 4 — Set the Correct Build Actions in Visual Studio / Rider

After placing the config files, verify the build actions are set correctly.

### Android — `google-services.json`

In your IDE, right-click each `google-services.json` file → Properties → Build Action:

```
GoogleServicesJson
```

Or verify in the csproj that it reads:

```xml
<GoogleServicesJson Include="Platforms\Android\google-services.json" />
```

If the IDE doesn't set this automatically, add it manually to the csproj inside an `<ItemGroup>`.

### iOS — `GoogleService-Info.plist`

Right-click each `GoogleService-Info.plist` → Properties → Build Action:

```
BundleResource
```

Or in csproj:

```xml
<BundleResource Include="Platforms\iOS\GoogleService-Info.plist" />
```

---

## Step 5 — Build and Test

1. Build in **Release** configuration (Crashlytics calls are excluded in Debug builds by design)
2. Run the app on a real device
3. Trigger a test crash — the easiest way is to call this somewhere temporary:
   ```csharp
   Plugin.Firebase.Crashlytics.CrossFirebaseCrashlytics.Current.TestIt();
   ```
4. Restart the app after the crash
5. Wait ~5 minutes, then check **Crashlytics** in the Firebase Console — the crash should appear

---

## Step 6 — Keep Config Files Out of Git (Recommended)

The config files contain app-specific secrets. Add them to `.gitignore`:

```
# Firebase config files
**/Platforms/Android/google-services.json
**/Platforms/iOS/GoogleService-Info.plist
```

Store them securely (e.g. in a password manager or CI/CD secret store) and distribute to team members out of band.

---

## Summary of File Locations

| App | Android config | iOS config |
|---|---|---|
| Countries | `Maui.Countries/Platforms/Android/google-services.json` | `Maui.Countries/Platforms/iOS/GoogleService-Info.plist` |
| Saints | `Maui.Saints/Platforms/Android/google-services.json` | `Maui.Saints/Platforms/iOS/GoogleService-Info.plist` |
| WorldLeaders | `Maui.WorldLeaders/Platforms/Android/google-services.json` | `Maui.WorldLeaders/Platforms/iOS/GoogleService-Info.plist` |
| Popes | `Maui.Popes/Platforms/Android/google-services.json` | `Maui.Popes/Platforms/iOS/GoogleService-Info.plist` |

---

## How Crashes Are Reported

Once enabled, any exception passed to `ExceptionHandler.CaptureException()` will be recorded to Crashlytics in Release builds. This includes:

- Unhandled exceptions caught by `AppDomain.CurrentDomain.UnhandledException`
- Unobserved task exceptions caught by `TaskScheduler.UnobservedTaskException`
- Native iOS exceptions caught by `MarshalManagedException` / `MarshalObjectiveCException`
- All manually caught exceptions throughout the app
