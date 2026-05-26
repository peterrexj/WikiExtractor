# Interstitial Ads + Remove Ads IAP — Production Setup Guide

## Overview

The code is fully implemented across all 4 apps (Countries, Saints, WorldLeaders, Popes). This guide covers every manual step required in external consoles and in code before shipping to production.

---

## Part 1 — Google AdMob: Fix Missing Interstitial Ad Units

### Apps with test/placeholder IDs that need real ones

**Saints (Android & iOS)** — currently using Google's public test app IDs (`ca-app-pub-3940256099942544`). You must create a real AdMob app and ad units.

**Popes (Android)** — Banner and QuizBanner are still using test IDs. The interstitial is real. The banners won't earn revenue.

All other apps (Countries, WorldLeaders, Popes iOS) already have real production ad unit IDs.

### Steps

1. Go to [admob.google.com](https://admob.google.com)
2. For each app that needs new ad units, open the app → **Ad units** → **Add ad unit**
3. Create one **Interstitial** ad unit per app that needs it
4. Copy the generated `ca-app-pub-XXXXXXXX/XXXXXXXXXX` ID
5. Update the appropriate `AppInformation.cs` file:

**Saints Android** — `src/Maui/Apps/Maui.Saints/Platforms/Android/DependencyInjection/AppInformation.cs`

Replace ALL four ad IDs — currently all using `ca-app-pub-3940256099942544` test IDs:

```csharp
public string AdsAppId => "ca-app-pub-YOUR_SAINTS_ANDROID_APP_ID";
public string AdsBannerId => "ca-app-pub-YOUR_SAINTS_ANDROID_BANNER_ID";
public string AdsQuizBannerId => "ca-app-pub-YOUR_SAINTS_ANDROID_QUIZ_BANNER_ID";
public string AdsInterstitialId => "ca-app-pub-YOUR_SAINTS_ANDROID_INTERSTITIAL_ID";
```

**Saints iOS** — `src/Maui/Apps/Maui.Saints/Platforms/iOS/DependencyInjection/AppInformation.cs`

Same replacement — currently all test IDs.

**Popes Android** — `src/Maui/Apps/Maui.Popes/Platforms/Android/DependencyInjection/AppInformation.cs`

Replace only the two banner IDs (the interstitial `ca-app-pub-4219645367584712/3071004011` is already real):

```csharp
public string AdsBannerId => "ca-app-pub-4219645367584712/YOUR_POPES_ANDROID_BANNER_ID";
public string AdsQuizBannerId => "ca-app-pub-4219645367584712/YOUR_POPES_ANDROID_QUIZ_BANNER_ID";
```

---

## Part 2 — Google Play Console: Create "Remove Ads" In-App Purchase

Do this for all 4 apps. Each app has its own Play Console entry.

### Bundle IDs

| App | Bundle ID |
|-----|-----------|
| Countries | `com.pj.countriesofworld` |
| Saints | `com.peterrexj.christiancatholicsaints` |
| WorldLeaders | `com.pj.worldleadershub` |
| Popes | `com.peterrexj.popesofchurch` |

### Steps (repeat for each app)

1. Open [play.google.com/console](https://play.google.com/console)
2. Select the app → **Monetize** → **Products** → **In-app products**
3. Click **Create product**
4. Fill in:
   - **Product ID**: `no_ads` _(must match exactly — this is what the code uses)_
   - **Name**: "Remove Ads" (or "No Ads — Premium")
   - **Description**: "Remove all ads from the app permanently."
   - **Status**: Active
   - **Price**: set your price (e.g. $1.99 or $2.99)
5. Click **Save** then **Activate**

> The product ID `no_ads` must be identical across all 4 apps on Google Play. If you ever want per-app product IDs (e.g. `no_ads_countries`), you must also update `NoAdsProductId` in the corresponding `AppInformation.cs` files.

---

## Part 3 — App Store Connect: Create "Remove Ads" In-App Purchase

Do this for all 4 apps.

### Bundle IDs

Same as the table above — use the bundle ID to find the right app in App Store Connect.

### Steps (repeat for each app)

1. Open [appstoreconnect.apple.com](https://appstoreconnect.apple.com)
2. Select the app → **Features** → **In-App Purchases** → **+**
3. Select type: **Non-Consumable** (user buys once, owns forever)
4. Fill in:
   - **Reference Name**: "Remove Ads"
   - **Product ID**: `no_ads` _(must match exactly)_
5. Add a **Localization** (English required):
   - **Display Name**: "Remove Ads"
   - **Description**: "Remove all ads from the app permanently."
6. Set **Price**: choose a price tier (e.g. Tier 2 = $1.99)
7. Under **Review Information**, add a screenshot of the purchase flow (App Review requires this for IAP)
8. Set status to **Ready to Submit**

> Apple requires IAP to be submitted together with an app version the first time. The IAP won't be live until an app update is approved.

---

## Part 4 — Android: Billing Permission

The `BILLING` permission has been explicitly added to all 4 `AndroidManifest.xml` files. No action needed — this is for reference only.

- `src/Maui/Apps/Maui.Countries/Platforms/Android/AndroidManifest.xml` ✓
- `src/Maui/Apps/Maui.Saints/Platforms/Android/AndroidManifest.xml` ✓
- `src/Maui/Apps/Maui.WorldLeaders/Platforms/Android/AndroidManifest.xml` ✓
- `src/Maui/Apps/Maui.Popes/Platforms/Android/AndroidManifest.xml` ✓

```xml
<uses-permission android:name="com.android.vending.BILLING" />
```

---

## Part 5 — iOS: StoreKit Capability

For in-app purchases to work on iOS:

1. In the Apple Developer portal, ensure **In-App Purchase** capability is enabled for each app's App ID under **Identifiers**
2. No special entitlement key is required in the project — StoreKit is available to all apps by default
3. Confirm `Plugin.InAppBilling` is listed in each app's `.csproj` (already added):

```xml
<PackageReference Include="Plugin.InAppBilling" Version="8.0.5" />
```

---

## Part 6 — Restore NuGet Packages

Run this once after the code changes are merged to download `Plugin.InAppBilling`:

```bash
dotnet restore
```

Or target a specific project:

```bash
dotnet restore src/Maui/Library/WikiExtractor.Maui.App/WikiExtractor.Maui.App.csproj
```

---

## Part 7 — Testing Interstitial Ads

### How it works

- `FirstInterstitialAdThreshold = 1` — interstitial fires on the **first** navigation to a detail page or quiz
- `SubsequentInterstitialAdThreshold = 3` — fires every **3rd** navigation after that
- In `DEBUG` builds, `TestMode = true` is set automatically — Google's test ads are shown (no real revenue, no policy risk)

### Android

1. Build in **Debug**
2. Navigate to any item detail page or tap **Take Quiz**
3. Verify the test interstitial appears, then dismissing it completes the navigation

### iOS

Same as Android. Test mode activates automatically in debug builds.

### Release testing

For Android, register your device's advertising ID in AdMob under **Settings → Test devices** to see real ads without generating invalid traffic.

---

## Part 8 — Testing the Remove Ads Purchase

### Android — Internal Testing track

Google Play requires the app to be uploaded before real billing can be tested:

1. Build a release AAB: `dotnet publish -f net9.0-android -c Release`
2. Upload to Play Console → **Testing** → **Internal testing** → create a new release
3. Add your Google account as a tester
4. Install from the Internal Testing link (not sideloaded APK)
5. Set up a **License tester** in Play Console → **Setup** → **License testing** — add your Gmail to get `$0.00` test purchases
6. Open **Settings** in the app → tap **Buy** under Remove Ads
7. Complete the test purchase
8. Verify: relaunch the app — ads should not appear, and "Purchased — Ads are off" should show in Settings

### iOS — Sandbox testers

1. App Store Connect → **Users and Access** → **Sandbox** → **Testers** → add a sandbox Apple ID
2. On your test device, go to **Settings → App Store** → sign out, sign in with the sandbox account
3. Install the app via Xcode or TestFlight
4. Open **Settings** in the app → tap **Buy** — the sandbox purchase completes without a real charge
5. Verify ads are gone after purchase and on the next app launch

### Restore Purchases

1. Uninstall and reinstall the app (clears local SecureStorage entitlement cache)
2. Open **Settings** → tap **Restore**
3. Confirm the entitlement is restored and ads are disabled

---

## Current Ad Unit IDs Reference

### Countries

| Platform | Type | ID |
|----------|------|----|
| Android | App | `ca-app-pub-4219645367584712~3489544050` |
| Android | Banner | `ca-app-pub-4219645367584712/3041169107` |
| Android | Quiz Banner | `ca-app-pub-4219645367584712/2354361942` |
| Android | Interstitial | `ca-app-pub-4219645367584712/4901045689` |
| iOS | App | `ca-app-pub-4219645367584712~1323561667` |
| iOS | Banner | `ca-app-pub-4219645367584712/2073536552` |
| iOS | Quiz Banner | `ca-app-pub-4219645367584712/5146615629` |
| iOS | Interstitial | `ca-app-pub-4219645367584712/4887402158` |

### WorldLeaders

| Platform | Type | ID |
|----------|------|----|
| Android | App | `ca-app-pub-4219645367584712~7724393725` |
| Android | Banner | `ca-app-pub-4219645367584712/1240528817` |
| Android | Quiz Banner | `ca-app-pub-4219645367584712/8157352960` |
| Android | Interstitial | `ca-app-pub-4219645367584712/4014031811` |
| iOS | App | `ca-app-pub-4219645367584712~1266796586` |
| iOS | Banner | `ca-app-pub-4219645367584712/2856862813` |
| iOS | Quiz Banner | `ca-app-pub-4219645367584712/1398942308` |
| iOS | Interstitial | `ca-app-pub-4219645367584712/4668637936` |

### Popes

| Platform | Type | ID |
|----------|------|----|
| Android | App | `ca-app-pub-4219645367584712~1706236868` |
| Android | Banner | `ca-app-pub-3940256099942544/6300978111` ⚠️ test ID — replace |
| Android | Quiz Banner | `ca-app-pub-3940256099942544/6300978111` ⚠️ test ID — replace |
| Android | Interstitial | `ca-app-pub-4219645367584712/3071004011` |
| iOS | App | `ca-app-pub-4219645367584712~8734202306` |
| iOS | Banner | `ca-app-pub-4219645367584712/8224449302` |
| iOS | Quiz Banner | `ca-app-pub-4219645367584712/2322572355` |
| iOS | Interstitial | `ca-app-pub-4219645367584712/1495389423` |

### Saints

| Platform | Type | ID |
|----------|------|----|
| Android | App | `ca-app-pub-3940256099942544~3347511713` ⚠️ test ID — replace |
| Android | Banner | `ca-app-pub-3940256099942544/6300978111` ⚠️ test ID — replace |
| Android | Quiz Banner | `ca-app-pub-3940256099942544/6300978111` ⚠️ test ID — replace |
| Android | Interstitial | `ca-app-pub-3940256099942544/1033173712` ⚠️ test ID — replace |
| iOS | App | `ca-app-pub-3940256099942544~1458002511` ⚠️ test ID — replace |
| iOS | Banner | `ca-app-pub-3940256099942544/2934735716` ⚠️ test ID — replace |
| iOS | Quiz Banner | `ca-app-pub-3940256099942544/2934735716` ⚠️ test ID — replace |
| iOS | Interstitial | `ca-app-pub-3940256099942544/4411468910` ⚠️ test ID — replace |

---

## Part 9 — Startup Entitlement Check (Architecture Note)

The "already purchased" check runs in `App.xaml.cs` → `InitializeAppControllerAsync()` on a background thread, **not** in `MauiProgram.CreateMauiApp()`. This is intentional.

`MauiProgram.CreateMauiApp()` runs during `Application.OnCreate()` before any Android `Activity` exists. Calling `SecureStorage.GetAsync()` there with `.GetAwaiter().GetResult()` deadlocks on Android because the Android Keystore needs the main thread's message pump to complete, which isn't running yet at that point.

The entitlement check in `InitializeAppControllerAsync` follows this order:

1. Check `SecureStorage` for a previously stored entitlement (`LoadLocalEntitlementAsync`) — fast, no billing SDK
2. If entitled: call `SharedServiceCore.DisableAds()` immediately
3. If not entitled: check for a pending purchase (`CheckPendingPurchaseAsync`) — connects to Google Play/StoreKit only if needed

This means there is a brief window at startup (a few hundred milliseconds) where a previously-purchased user could theoretically see an ad before the background check completes. In practice `WaitForInitializationAsync(5000)` runs first and the check completes well before any navigation triggers an ad.

---

| Task | Done |
|------|------|
| AdMob: create real app + ad units for Saints Android | ☐ |
| AdMob: create real app + ad units for Saints iOS | ☐ |
| AdMob: create real banner ad units for Popes Android | ☐ |
| Update `AppInformation.cs` files with real ad unit IDs | ☐ |
| Google Play: create `no_ads` IAP product for Countries | ☐ |
| Google Play: create `no_ads` IAP product for Saints | ☐ |
| Google Play: create `no_ads` IAP product for WorldLeaders | ☐ |
| Google Play: create `no_ads` IAP product for Popes | ☐ |
| App Store Connect: create `no_ads` IAP for Countries | ☐ |
| App Store Connect: create `no_ads` IAP for Saints | ☐ |
| App Store Connect: create `no_ads` IAP for WorldLeaders | ☐ |
| App Store Connect: create `no_ads` IAP for Popes | ☐ |
| Verify `BILLING` permission in Android manifests | ✓ (added to all 4) |
| `dotnet restore` to pull Plugin.InAppBilling 8.0.5 | ☐ |
| Test interstitials in debug builds (both platforms) | ☐ |
| Test purchase flow on Android via Internal Testing track | ☐ |
| Test purchase flow on iOS via Sandbox tester | ☐ |
| Test Restore Purchases on both platforms | ☐ |
| `TestMode` is already `false` for release builds via `#if DEBUG` | ✓ |
