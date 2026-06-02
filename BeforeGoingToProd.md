# Before Going to Production

Everything required before shipping any of the 4 apps (Countries, Saints, WorldLeaders, Popes) to the Play Store or App Store.

---

## Bundle IDs Reference

| App | Android package | iOS bundle |
|-----|----------------|------------|
| Countries Insights | `com.pj.countriesofworld` | `com.pj.countriesofworld` |
| Catholic Saints | `com.peterrexj.christiancatholicsaints` | `com.peterrexj.christiancatholicsaints` |
| World Leaders Hub | `com.pj.worldleadershub` | `com.pj.worldleadershub` |
| Popes of Church | `com.peterrexj.popesofchurch` | `com.peterrexj.popesofchurch` |

---

## 1 — Code Placeholders to Replace

### iOS App Store IDs (all 4 apps)

All 4 iOS `AppInformation.cs` files currently have `idYOUR_APP_ID` as a placeholder.
After creating each app in App Store Connect and receiving its numeric App ID:

**Countries iOS** — `src/Maui/Apps/Maui.Countries/Platforms/iOS/DependencyInjection/AppInformation.cs`
```csharp
public string AppShareLink => "https://apps.apple.com/app/id<YOUR_ID>";
public string RateAppLink => "itms-apps://itunes.apple.com/app/id<YOUR_ID>?action=write-review";
```

**Saints iOS** — `src/Maui/Apps/Maui.Saints/Platforms/iOS/DependencyInjection/AppInformation.cs`
Same pattern — replace `idYOUR_APP_ID`.

**WorldLeaders iOS** — `src/Maui/Apps/Maui.WorldLeaders/Platforms/iOS/DependencyInjection/AppInformation.cs`
Same pattern.

**Popes iOS** — `src/Maui/Apps/Maui.Popes/Platforms/iOS/DependencyInjection/AppInformation.cs`
Same pattern.

### Android Share/Rate Links (verify package names)

The Android share/rate links should use the correct bundle IDs above.
Current values in each Android `AppInformation.cs`:

| App | AppShareLink package | Should be |
|-----|---------------------|-----------|
| Countries | `com.pj.countries.wiki` | `com.pj.countriesofworld` |
| Saints | `com.pj.saints.wiki` | `com.peterrexj.christiancatholicsaints` |
| WorldLeaders | `com.pj.worldleaders.wiki` | `com.pj.worldleadershub` |
| Popes | `com.pj.popes.wiki` | `com.peterrexj.popesofchurch` |

Fix in each Android `AppInformation.cs`:
```csharp
// Countries
public string AppShareLink => "https://play.google.com/store/apps/details?id=com.pj.countriesofworld";
public string RateAppLink => "market://details?id=com.pj.countriesofworld";

// Saints
public string AppShareLink => "https://play.google.com/store/apps/details?id=com.peterrexj.christiancatholicsaints";
public string RateAppLink => "market://details?id=com.peterrexj.christiancatholicsaints";

// WorldLeaders
public string AppShareLink => "https://play.google.com/store/apps/details?id=com.pj.worldleadershub";
public string RateAppLink => "market://details?id=com.pj.worldleadershub";

// Popes
public string AppShareLink => "https://play.google.com/store/apps/details?id=com.peterrexj.popesofchurch";
public string RateAppLink => "market://details?id=com.peterrexj.popesofchurch";
```

---

## 2 — AdMob: Fix Test IDs

See `ADS_AND_IAP_SETUP_GUIDE.md` for full instructions. Summary of what needs real IDs:

### Saints (Android + iOS) — ALL IDs are Google test IDs

Go to [admob.google.com](https://admob.google.com), create a new app for each platform, create Banner, Quiz Banner, and Interstitial ad units.

**Saints Android** — `src/Maui/Apps/Maui.Saints/Platforms/Android/DependencyInjection/AppInformation.cs`
```
AdsAppId         ca-app-pub-3940256099942544~3347511713  ⚠ REPLACE
AdsBannerId      ca-app-pub-3940256099942544/6300978111  ⚠ REPLACE
AdsQuizBannerId  ca-app-pub-3940256099942544/6300978111  ⚠ REPLACE
AdsInterstitialId ca-app-pub-3940256099942544/1033173712 ⚠ REPLACE
```

**Saints iOS** — `src/Maui/Apps/Maui.Saints/Platforms/iOS/DependencyInjection/AppInformation.cs`
```
AdsAppId         ca-app-pub-3940256099942544~1458002511  ⚠ REPLACE
AdsBannerId      ca-app-pub-3940256099942544/2934735716  ⚠ REPLACE
AdsQuizBannerId  ca-app-pub-3940256099942544/2934735716  ⚠ REPLACE
AdsInterstitialId ca-app-pub-3940256099942544/4411468910 ⚠ REPLACE
```

### Popes Android — Banner IDs only

**Popes Android** — `src/Maui/Apps/Maui.Popes/Platforms/Android/DependencyInjection/AppInformation.cs`
```
AdsBannerId      ca-app-pub-3940256099942544/6300978111  ⚠ REPLACE
AdsQuizBannerId  ca-app-pub-3940256099942544/6300978111  ⚠ REPLACE
```
(The Popes interstitial `ca-app-pub-4219645367584712/3071004011` is real — keep it.)

### Apps with real IDs already (no action needed)

Countries (Android + iOS), WorldLeaders (Android + iOS), Popes iOS — all production IDs.

---

## 3 — Firebase / Crashlytics

See `src/Maui/FIREBASE_SETUP.md` for full step-by-step. Summary:

### Step 1 — Create Firebase project

One project can host all 4 apps: [console.firebase.google.com](https://console.firebase.google.com)

### Step 2 — Download and place config files

None of these files exist in the repo yet — they must be created per app:

| App | Android path | iOS path |
|-----|-------------|----------|
| Countries | `Maui.Countries/Platforms/Android/google-services.json` | `Maui.Countries/Platforms/iOS/GoogleService-Info.plist` |
| Saints | `Maui.Saints/Platforms/Android/google-services.json` | `Maui.Saints/Platforms/iOS/GoogleService-Info.plist` |
| WorldLeaders | `Maui.WorldLeaders/Platforms/Android/google-services.json` | `Maui.WorldLeaders/Platforms/iOS/GoogleService-Info.plist` |
| Popes | `Maui.Popes/Platforms/Android/google-services.json` | `Maui.Popes/Platforms/iOS/GoogleService-Info.plist` |

Build actions after placing the files:
- `google-services.json` → **GoogleServicesJson**
- `GoogleService-Info.plist` → **BundleResource**

### Step 3 — iOS Crashlytics blocked until .NET 10

Firebase Crashlytics on iOS is currently disabled due to a `GULNetworkInfo` symbol conflict in `Plugin.Firebase.Crashlytics 3.1.1`. Crashlytics works on Android today. iOS will be enabled when upgrading to .NET 10 (see section 6).

### Step 4 — Test crash reporting

1. Build in **Release**, run on a real device
2. Call `Plugin.Firebase.Crashlytics.CrossFirebaseCrashlytics.Current.TestIt()` once to verify
3. Restart the app, wait ~5 min, check the Crashlytics dashboard

---

## 4 — In-App Purchases: "Remove Ads"

See `ADS_AND_IAP_SETUP_GUIDE.md` Parts 2–3 for full instructions. Product ID is `no_ads` for all apps on both platforms.

### Google Play Console (Android)

Do this for each of the 4 apps:
1. Play Console → app → **Monetize** → **Products** → **In-app products** → **Create product**
2. Product ID: `no_ads` (exact — must match code)
3. Status: Active, set your price
4. After uploading a release: test via **Internal Testing** track with a **License tester** account

### App Store Connect (iOS)

Do this for each of the 4 apps:
1. App Store Connect → app → **Features** → **In-App Purchases** → **+**
2. Type: **Non-Consumable**
3. Product ID: `no_ads` (exact — must match code)
4. Status: Ready to Submit (submits with the first app version)
5. A screenshot of the purchase flow is required for App Review

### Restore Purchases

Test after creating IAP products:
1. Uninstall and reinstall the app
2. Tap **Restore** in Settings
3. Verify ads are disabled and "Purchased" state persists across launches

---

## 5 — App Store / Play Store Listings

### Both stores (all 4 apps)

- [ ] App name, description, screenshots, icon uploaded
- [ ] Privacy policy URL set (required by both stores)
- [ ] Age rating / content rating completed
- [ ] Keywords / search optimization

### Google Play Console

- [ ] App signing configured (upload key enrolled)
- [ ] Target API level ≥ 34 (Android 14) — required for new apps
- [ ] Data safety form completed (declare what data the app collects)
- [ ] Internal → Closed → Open testing track progression before production release

### App Store Connect

- [ ] Provisioning profiles and distribution certificate up to date
- [ ] App Store Review notes filled in (mention test credentials if any gated content)
- [ ] Export compliance answered (no encryption = No to both questions for this app type)
- [ ] At least one iPhone and one iPad screenshot per locale

---

## 6 — .NET 10 Upgrade (post-launch, November 2026)

See `src/Maui/DOTNET10_UPGRADE.md` for full instructions. This is not a launch blocker but unlocks iOS Crashlytics.

Key changes when .NET 10 GA releases:
- Change all `net9.0-*` TFMs to `net10.0-*` in all 8 project files
- Bump `Plugin.Firebase.Crashlytics` from 3.1.1 → 4.0.0
- Remove Android-only condition on the Crashlytics package reference
- Add `CrossFirebase.Initialize()` back to all 4 iOS `AppDelegate.cs` files
- Change `ExceptionHandler.cs` `#if ANDROID` → `#if (ANDROID || IOS)` for crash capture

---

## 7 — Pre-Release Build Checklist

### Code

- [ ] All `TODO` comments resolved or documented
- [ ] No hardcoded test/debug values (search for `TODO:`, `YOUR_APP_ID`, `REPLACE`)
- [ ] Release build compiles with 0 errors on both platforms
- [ ] `dotnet restore` run after any package changes

### Ads

- [ ] `TestMode` is `false` in release builds — already handled via `#if DEBUG` in code
- [ ] Register test devices in AdMob to avoid invalid traffic during QA

### Firebase

- [ ] `google-services.json` and `GoogleService-Info.plist` placed for each app
- [ ] Build actions set correctly (GoogleServicesJson / BundleResource)
- [ ] Test crash verified in Firebase Console before shipping

### IAP

- [ ] `no_ads` product Active in Google Play Console for all 4 apps
- [ ] `no_ads` product Ready to Submit in App Store Connect for all 4 apps
- [ ] Purchase flow tested end-to-end on both platforms
- [ ] Restore Purchases tested on both platforms

### iOS App Store IDs

- [ ] All 4 iOS `AppInformation.cs` files updated with real App Store numeric IDs

### Android Share Links

- [ ] All 4 Android `AppInformation.cs` `AppShareLink` and `RateAppLink` use correct package names (see Section 1)

---

## 8 — Quick Checklist Summary

| Task | App | Done |
|------|-----|------|
| Fix Android share/rate link — Countries | Countries | ☐ |
| Fix Android share/rate link — Saints | Saints | ☐ |
| Fix Android share/rate link — WorldLeaders | WorldLeaders | ☐ |
| Fix Android share/rate link — Popes | Popes | ☐ |
| Fill iOS App Store ID — Countries | Countries | ☐ |
| Fill iOS App Store ID — Saints | Saints | ☐ |
| Fill iOS App Store ID — WorldLeaders | WorldLeaders | ☐ |
| Fill iOS App Store ID — Popes | Popes | ☐ |
| Create real AdMob app + all ad units — Saints Android | Saints | ☐ |
| Create real AdMob app + all ad units — Saints iOS | Saints | ☐ |
| Create real AdMob banner ad units — Popes Android | Popes | ☐ |
| Place `google-services.json` — all 4 apps | All | ☐ |
| Place `GoogleService-Info.plist` — all 4 apps | All | ☐ |
| Enable Crashlytics in Firebase Console — all 4 apps | All | ☐ |
| Test crash report appears in Firebase | All | ☐ |
| Create `no_ads` IAP in Google Play Console — Countries | Countries | ☐ |
| Create `no_ads` IAP in Google Play Console — Saints | Saints | ☐ |
| Create `no_ads` IAP in Google Play Console — WorldLeaders | WorldLeaders | ☐ |
| Create `no_ads` IAP in Google Play Console — Popes | Popes | ☐ |
| Create `no_ads` IAP in App Store Connect — Countries | Countries | ☐ |
| Create `no_ads` IAP in App Store Connect — Saints | Saints | ☐ |
| Create `no_ads` IAP in App Store Connect — WorldLeaders | WorldLeaders | ☐ |
| Create `no_ads` IAP in App Store Connect — Popes | Popes | ☐ |
| Test purchase + restore on Android | All | ☐ |
| Test purchase + restore on iOS | All | ☐ |
| Store listings complete (screenshots, description, privacy policy) | All | ☐ |
| Data safety form completed in Play Console | All | ☐ |
| Export compliance answered in App Store Connect | All | ☐ |
| Release build 0 errors on Android | All | ☐ |
| Release build 0 errors on iOS | All | ☐ |
