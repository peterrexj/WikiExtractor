# 🚀 MAUI Android Debugging in Rider - Setup Complete!

## ✅ Status: All Configurations Ready

Your Rider IDE is now fully configured for debugging the MAUI.Popes Android application.

---

## 📚 Documentation Index

### For a Quick Start → Read First:
- **📖 [READY_TO_DEBUG.md](READY_TO_DEBUG.md)** - 5-minute quick start guide

### For Complete Details:
- **📋 [DEBUG_CONFIGURATION_GUIDE.md](DEBUG_CONFIGURATION_GUIDE.md)** - Comprehensive guide with all features
- **✅ [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md)** - Step-by-step setup with troubleshooting

---

## 🎯 What Was Configured

### 1. Rider Run Configurations ✅
Two production-ready debug configurations created:

```
📁 .idea/runConfigurations/
├── Maui_Popes_Android_Debug.xml      (Emulator)
└── Maui_Popes_Android_Device.xml     (Physical Device)
```

### 2. Project Optimizations ✅
`Maui.Popes.csproj` enhanced with:
- Debug PropertyGroup (optimized for development)
- Release PropertyGroup (optimized for production)

### 3. Documentation ✅
- `DEBUG_CONFIGURATION_GUIDE.md` - Features & usage
- `DEBUGGING_CHECKLIST.md` - Setup steps & troubleshooting
- `.vscode/launch.json` - VS Code reference
- `README_DEBUG_SETUP.md` - This file

---

## 🚀 Quick Start (3 Simple Steps)

### Step 1: Reload Rider
```
File → Invalidate Caches → Invalidate and Restart
```

### Step 2: Select Configuration
Top toolbar → Run Configuration dropdown:
- **"Maui.Popes - Android Debug"** (emulator) OR
- **"Maui.Popes - Android Device"** (physical)

### Step 3: Start Debugging
Press **Cmd+D** (or click 🐛 Debug button)

**That's it!** App builds, deploys, and launches with debugger attached.

---

## 🎮 Common Debug Tasks

| Task | How |
|------|-----|
| **Set Breakpoint** | Click gutter in code |
| **Step Into Code** | F7 |
| **Step Over Function** | F8 |
| **Continue Execution** | F9 |
| **Inspect Variable** | Hover over it |
| **View App Logs** | View → Tool Windows → Logcat |
| **View Call Stack** | Debug panel (bottom) |
| **Pause Execution** | Click pause button |

---

## ⚙️ What Each Configuration Does

### "Maui.Popes - Android Debug" (Emulator)
- Targets: Android Emulator (arm64 architecture)
- Deploys APK to running emulator
- Attaches debugger automatically
- Best for: Development & testing on emulator
- Speed: 2-5 min first deploy, 30-60s subsequent

### "Maui.Popes - Android Device" (Physical)
- Targets: Physical Android device (USB connected)
- Deploys APK via ADB
- Attaches debugger automatically
- Best for: Testing on real device
- Speed: Often faster than emulator
- Prerequisite: USB debugging enabled on device

---

## 🔧 Prerequisites Check

Before debugging, ensure you have:

```bash
# 1. .NET 9 SDK
dotnet --version
# Should show: 9.x.x

# 2. MAUI Workload
dotnet workload list | grep maui
# Should show: maui

# 3. Android SDK API 36+
# Android SDK Manager in Rider or command line

# 4. For Emulator:
emulator -list-avds
# Should show your virtual device

# 5. For Physical Device:
adb devices
# Should show your connected device
```

---

## 📍 File Locations

### Run Configurations (New)
```
📁 Maui.Popes/
└── 📁 .idea/
    └── 📁 runConfigurations/
        ├── Maui_Popes_Android_Debug.xml
        └── Maui_Popes_Android_Device.xml
```

### Project Configuration (Modified)
```
📁 Maui.Popes/
└── Maui.Popes.csproj (added Debug/Release PropertyGroups)
```

### Documentation (New)
```
📁 Maui.Popes/
├── DEBUG_CONFIGURATION_GUIDE.md
├── DEBUGGING_CHECKLIST.md
├── .vscode/launch.json
└── README_DEBUG_SETUP.md (this file)
```

---

## 🎯 Your App Details

| Setting | Value |
|---------|-------|
| **Project Name** | Maui.Popes |
| **Package Name** | com.peterrexj.popesofchurch |
| **Target Framework** | net9.0-android36.0 |
| **Namespace** | Maui.Wiki |
| **Main Activity** | MainActivity |
| **Supported ABIs** | arm64-v8a, x86_64 |
| **Min API Level** | 21 (Android 5.0) |
| **Target API Level** | 34 (Android 14) |

---

## 🎓 Next Actions

1. **Read** [READY_TO_DEBUG.md](READY_TO_DEBUG.md) (5 min)
2. **Reload** Rider
3. **Verify** prerequisites (emulator/device)
4. **Select** debug configuration
5. **Press** Cmd+D
6. **Set** breakpoints
7. **Debug** your code! 🎉

---

## ⚠️ Troubleshooting Quick Links

**Configuration not appearing?** → See [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md#configuration-not-found-error)

**Debugger won't attach?** → See [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md#debugger-wont-attach)

**Build fails?** → See [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md#build-fails)

**First deploy very slow?** → See [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md#first-deployment-is-very-slow)

---

## 💡 Pro Tips

✅ **Use Logcat** - Real-time app logs are invaluable  
✅ **Set Conditional Breakpoints** - Right-click breakpoint to add conditions  
✅ **Use Watch Expressions** - Debug panel → add custom watches  
✅ **Try Hot Reload** - Make code changes without full rebuild  
✅ **Physical Device** - Usually faster than emulator  
✅ **Keep Window Open** - Visual feedback during deployment  

---

## 🆘 Getting Help

1. **Installation Issues?** → Check prerequisites above
2. **Configuration Questions?** → See [DEBUG_CONFIGURATION_GUIDE.md](DEBUG_CONFIGURATION_GUIDE.md)
3. **Setup Steps?** → Follow [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md)
4. **Specific Error?** → Search error in [DEBUGGING_CHECKLIST.md](DEBUGGING_CHECKLIST.md#troubleshooting)

---

## 🎉 You're Ready to Debug!

Everything is configured and documented. Your MAUI Android app is ready for debugging in Rider.

**Happy coding!** 🚀

---

### Last Updated
April 5, 2026

### Configuration Version
1.0 - MAUI Android Rider Debug Setup Complete


