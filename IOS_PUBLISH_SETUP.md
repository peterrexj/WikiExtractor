# iOS App Store Publishing — One-Time Setup Guide

This guide walks you through everything you need to do **once** before you can run the iOS publish scripts (`publish-ios-*.sh`). After completing these steps you will be able to build and submit any of the four apps (WorldLeaders, Popes, Saints, Countries) to the App Store.

**You only need to do this once per Mac.** After setup, publishing any app is just running a single script.

---

## What you will need

- A Mac (iOS publishing cannot be done on Windows)
- Xcode installed from the Mac App Store
- Access to the Apple Developer account (appleid used to enrol in the Apple Developer Program)
- The apps are already live on the App Store, so the certificates already exist — you just need to get them onto this Mac

---

## Step 1 — Install Xcode

1. Open the **App Store** on your Mac
2. Search for **Xcode**
3. Click **Get** then **Install**
4. Wait for the download to finish (it is several gigabytes, so this may take a while)
5. Once installed, open Xcode once so it can finish setting itself up — click **Install** when it asks to install additional components

> **How do you know it worked?** Open the Terminal app and type `xcode-select -p` then press Enter. You should see a path like `/Applications/Xcode.app/Contents/Developer`. If you do, Xcode is ready.

---

## Step 2 — Sign in to your Apple Developer account in Xcode

1. Open **Xcode**
2. In the top menu bar click **Xcode → Settings** (or press ⌘,)
3. Click the **Accounts** tab
4. Click the **+** button at the bottom left
5. Choose **Apple ID** and click **Continue**
6. Enter the Apple ID email and password used for the Apple Developer account
7. Click **Sign In**

You should now see your account listed with your name and team name underneath.

> **Why this matters:** Xcode needs to be signed in so it can automatically download and manage your provisioning profiles when the publish script runs.

---

## Step 3 — Download the Distribution Certificate (.p12) from Apple Developer portal

The Distribution Certificate is what Apple uses to verify that the app was built by you. It needs to be imported into your Mac's Keychain.

1. Open a web browser and go to **https://developer.apple.com**
2. Click **Account** and sign in
3. Click **Certificates, Identifiers & Profiles** in the left sidebar
4. Click **Certificates** in the left sidebar
5. Look for a certificate named **Apple Distribution** — it will show your name next to it (e.g. *Apple Distribution: Peter Joseph*)
6. Click on it
7. Click the **Download** button — this saves a `.cer` file to your Downloads folder

> **Note:** A `.cer` file alone is not enough — it does not contain the private key. The private key was created on the Mac where the certificate was originally generated. If you are on the same Mac, it is already in Keychain (skip to the verification step below). If you are on a different Mac, you need to export a `.p12` from the original Mac — see the section below.

### If you are on the same Mac where the certificate was created

The private key is already in Keychain. To confirm:

1. Open **Keychain Access** (search for it with Spotlight — press ⌘Space and type *Keychain Access*)
2. In the top left, make sure **login** is selected under Keychains
3. Click **My Certificates** in the left sidebar
4. Look for **Apple Distribution: Peter Joseph (5PNCUV7LZ5)**
5. There should be a small triangle/arrow next to it that you can expand to reveal a private key underneath

If you see it with the private key — you are done with this step.

### If you are on a different Mac (exporting from the original Mac)

On the **original Mac**:

1. Open **Keychain Access**
2. Click **My Certificates** in the left sidebar
3. Find **Apple Distribution: Peter Joseph (5PNCUV7LZ5)**
4. Right-click on it and choose **Export "Apple Distribution: Peter Joseph..."**
5. Choose a save location, give it a name like `distribution.p12`, and click **Save**
6. You will be asked to set a password — choose something you will remember and click **OK**
7. You may be asked for your Mac login password — enter it and click **Allow**

Transfer the `.p12` file to your new Mac (AirDrop, USB drive, or any method you prefer).

On the **new Mac**:

1. Double-click the `.p12` file
2. Keychain Access will open and ask for the password you set in step 6 above — enter it
3. Click **OK**

The certificate is now in Keychain. Confirm by following the "same Mac" verification steps above.

---

## Step 4 — Let Xcode download your Provisioning Profiles

Provisioning profiles tell Apple which apps you are allowed to distribute and on which devices. The publish scripts use Automatic provisioning, meaning Xcode handles this for you.

1. Open **Xcode**
2. Go to **Xcode → Settings** (⌘,) and click the **Accounts** tab
3. Click your Apple ID account in the list
4. Click **Download Manual Profiles** button at the bottom right

Xcode will download all provisioning profiles associated with your Apple Developer account, including the distribution profiles for WorldLeaders, Popes, Saints, and Countries.

> **How do you know it worked?** Click the arrow next to your team name in the Accounts pane — you should see profiles listed including ones for your app bundle IDs (e.g. `com.pj.worldleadershub`).

---

## Step 5 — Verify everything is ready

Open the **Terminal** app and run these three commands one at a time. Each one should return a result (not an error).

**Check Xcode is installed:**
```
xcode-select -p
```
Expected output: `/Applications/Xcode.app/Contents/Developer`

**Check the distribution certificate is in Keychain:**
```
security find-identity -v -p codesigning | grep "Apple Distribution"
```
Expected output: a line containing `Apple Distribution: Peter Joseph (5PNCUV7LZ5)`

**Check .NET is installed:**
```
dotnet --version
```
Expected output: a version number like `9.0.316`

If all three commands return the expected output, your Mac is fully set up and ready to publish iOS apps.

---

## Step 6 — Run a publish script

Once setup is complete, navigate to the app folder in Terminal and run the script:

```bash
cd /path/to/WikiExtractor/src/Maui/Apps/Maui.WorldLeaders
bash publish-ios-worldleaders.sh
```

When it finishes, it will print the path to the `.ipa` file. You then upload that file to **App Store Connect** (https://appstoreconnect.apple.com) using either:
- **Xcode → Organizer → Distribute App**, or
- The **Transporter** app (free on the Mac App Store)

---

## Troubleshooting

| Problem | Solution |
|---|---|
| `xcode-select: error: invalid developer directory` | Open Xcode once and accept the license agreement, or run `sudo xcode-select --switch /Applications/Xcode.app` |
| `No signing certificate "Apple Distribution" found` | The certificate is not in Keychain — repeat Step 3 |
| `Provisioning profile ... doesn't include signing certificate` | The profile and certificate don't match. Re-download profiles in Xcode (Step 4) |
| `Error: Publish failed` | Run the script again with `bash -x publish-ios-worldleaders.sh` to see the full error output |
| Certificate expired | Log in to developer.apple.com, revoke the old certificate, create a new one, and repeat Steps 3–4 |
