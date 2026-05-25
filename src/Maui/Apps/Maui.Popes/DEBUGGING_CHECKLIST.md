# 📝 Setup Checklist - Rider MAUI Android Debugging

## ✅ Completed Setup

- [x] Created Rider run configuration for **Emulator Debugging**
  - File: `.idea/runConfigurations/Maui_Popes_Android_Debug.xml`
  - Target: Android Emulator (arm64)
  - Configuration: Debug

- [x] Created Rider run configuration for **Device Debugging**
  - File: `.idea/runConfigurations/Maui_Popes_Android_Device.xml`
  - Target: Physical Android Device
  - Configuration: Debug

- [x] Enhanced `Maui.Popes.csproj` with debug optimizations
  - Added Debug PropertyGroup with:
    - Full debug symbols enabled
    - Code optimization disabled
    - No code linking
    - No ProGuard/R8 obfuscation
    - Shared runtime enabled for faster deployment
  - Added Release PropertyGroup for production builds

- [x] Created comprehensive documentation
  - `DEBUG_CONFIGURATION_GUIDE.md` - Detailed usage guide
  - `.vscode/launch.json` - VS Code reference configuration

---

## 🔧 Before You Start Debugging

### Prerequisites Check

- [ ] **Android SDK 36+** installed
  - Command: `echo $ANDROID_SDK_ROOT` or `echo $ANDROID_HOME`
  - If not set: Install Android SDK and set environment variable

- [ ] **Android Emulator** (for emulator debugging)
  - Create: `Tools → SDK Manager → Create Virtual Device`
  - Choose: API 36+ with arm64 architecture
  - Start: `emulator -avd <device_name> &`

- [ ] **Physical Device** (optional, for device debugging)
  - Connect via USB
  - Enable Developer Mode: Settings → About → Tap Build Number 7x
  - Enable USB Debugging: Settings → Developer Options → USB Debugging
  - Verify: Terminal → `adb devices` (should list your device)

- [ ] **Rider IDE**
  - Version: 2024.2+ (with MAUI support)
  - .NET 9 SDK: `dotnet --version` (should show 9.x.x)
  - MAUI workload: `dotnet workload list` (should show maui)
  - If missing: `dotnet workload install maui`

---

## 🚀 Step-by-Step: First Debug Session

### 1. Prepare Environment
```bash
# Verify .NET installation
dotnet --version

# Verify MAUI workload
dotnet workload list | grep maui

# If emulator: Start it
emulator -avd Pixel_7_API_36 &

# If device: Connect and verify
adb devices
```

### 2. Open Project in Rider
- Open Rider
- File → Open
- Navigate to: `/Users/josephpe/Git/peterrexj/new/WikiExtractor/src/Maui/Apps/Maui.Popes`
- Click Open

### 3. Reload Project (First Time)
- File → Invalidate Caches → Invalidate and Restart
- Rider will restart and load the new configurations

### 4. Select Debug Configuration
- Top toolbar → Find the **Run Configuration** dropdown (currently might say "Edit Configurations")
- Click dropdown
- Select: **"Maui.Popes - Android Debug"** (for emulator)
  - OR **"Maui.Popes - Android Device"** (for physical device)

### 5. Set Your First Breakpoint
- Open any `.cs` file (e.g., `MauiProgram.cs`)
- Click in the left gutter (margin) on any line
- A **red circle** appears = breakpoint set

### 6. Start Debugging
- Click the **Debug button** (green bug icon) in the toolbar
  - OR press **Cmd+D**
  - OR select Run → Debug

### 7. App Launches
- Rider will build the project
- Deploy APK to emulator/device
- Launch the app
- App should pause at your first breakpoint!

### 8. Debug Controls
- **F7** = Step Into (go into function calls)
- **F8** = Step Over (skip over function calls)
- **Shift+F8** = Step Out (exit current function)
- **F9** = Resume (continue execution)
- **Cmd+F8** = Toggle breakpoint on current line

---

## 🛠️ Common Debugging Tasks

### View Variable Values
1. During debug pause, hover over any variable
2. Tooltip shows current value
3. Or use Debug panel: View → Tool Windows → Debug

### View App Logs
1. View → Tool Windows → Logcat
2. Filter by app: `com.peterrexj.popesofchurch`
3. See real-time logs, warnings, and errors

### Inspect Call Stack
1. Debug panel (bottom of Rider)
2. See sequence of method calls leading to current location
3. Click any frame to jump to that line

### Evaluate Expressions
1. Debug console (bottom of Rider)
2. Type C# expressions to evaluate
3. Results shown in real-time

### Add Conditional Breakpoint
1. Right-click on breakpoint (red dot)
2. Choose "Edit breakpoint"
3. Add condition (e.g., `x > 10`)
4. Breakpoint only triggers when condition is true

---

## ⚠️ Troubleshooting

### "Configuration not found" error
**Solution:**
- Invalidate caches: File → Invalidate Caches → Invalidate and Restart
- Make sure `.idea` folder exists in project root
- Verify `.idea/runConfigurations/` folder has both XML files

### Emulator won't start
**Solution:**
```bash
# List available AVDs
emulator -list-avds

# Start specific emulator
emulator -avd Pixel_7_API_36 &

# Wait 2-3 minutes for emulator to fully boot
adb devices  # Should show your emulator when ready
```

### Device not recognized
**Solution:**
```bash
# Check connected devices
adb devices

# If device shows "unauthorized":
# 1. Check phone for "Allow USB debugging?" prompt and tap Allow
# 2. If not showing, restart ADB: adb kill-server && adb devices

# Try reconnecting device
adb disconnect
adb connect <device-ip-or-serial>
```

### Build fails
**Solution:**
- Clean project: Build → Clean
- Rebuild: Build → Rebuild Project
- Check Android SDK API 36+ is installed
- Verify .NET 9 SDK is installed: `dotnet --version`

### Debugger won't attach
**Solution:**
- Terminal: `adb devices` (verify connection)
- Kill any existing adb processes: `killall -9 adb`
- Restart adb: `adb devices`
- Try running again

### First deployment is very slow
**This is normal!** First deployment:
- Compiles entire project (~2-5 minutes)
- Creates APK
- Deploys to device/emulator
- Subsequent deployments are much faster (30-60 seconds)

---

## 📊 Configuration Summary

| Configuration | Target | Architecture | Build | Use Case |
|---------------|--------|--------------|-------|----------|
| Maui.Popes - Android Debug | Emulator | arm64 | Debug | Development/Debugging |
| Maui.Popes - Android Device | Physical Device | arm64 | Debug | Device Testing/Debugging |

---

## 🎯 Quick Reference

### Files Modified
- `Maui.Popes.csproj` - Added Debug/Release PropertyGroups

### Files Created
- `.idea/runConfigurations/Maui_Popes_Android_Debug.xml` - Emulator config
- `.idea/runConfigurations/Maui_Popes_Android_Device.xml` - Device config
- `DEBUG_CONFIGURATION_GUIDE.md` - Full guide
- `.vscode/launch.json` - VS Code reference

### Key Shortcuts
- `Cmd+D` - Start Debug
- `F7` - Step Into
- `F8` - Step Over
- `F9` - Resume
- `Cmd+F8` - Toggle Breakpoint

### App Package Name
```
com.peterrexj.popesofchurch
```

---

## 🎉 You're Ready!

Everything is configured. Just:
1. **Reload Rider**
2. **Select your debug configuration**
3. **Press Cmd+D**
4. **Enjoy debugging!**

---

**Questions?** Check `DEBUG_CONFIGURATION_GUIDE.md` for detailed information.


