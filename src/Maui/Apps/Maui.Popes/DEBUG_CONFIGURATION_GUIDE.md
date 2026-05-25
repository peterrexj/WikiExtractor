# MAUI Android Debugging Configuration in Rider

## Overview
I've set up two debug configurations for your MAUI.Popes Android app in Rider:

1. **Maui.Popes - Android Debug** (Emulator)
2. **Maui.Popes - Android Device** (Physical Device)

## Configuration Details

### What Was Added:

#### 1. Run Configurations
- ✅ Created `.idea/runConfigurations/Maui_Popes_Android_Debug.xml` - For emulator debugging
- ✅ Created `.idea/runConfigurations/Maui_Popes_Android_Device.xml` - For physical device debugging

#### 2. Project File Optimizations (Maui.Popes.csproj)
- ✅ Added Debug PropertyGroup with optimized settings:
  - `DebugSymbols=true` - Enables full debug symbol generation
  - `DebugType=portable` - Portable debug format compatible with cross-platform debugging
  - `Optimize=false` - Disables optimizations for clearer debugging
  - `AndroidUseSharedRuntime=true` - Faster deployment during development
  - `AndroidLinkMode=None` - No code linking (preserves all symbols)
  - `EmbedAssembliesIntoApk=false` - Allows faster reload during development

- ✅ Added Release PropertyGroup for production builds

## How to Use These Configurations

### Step 1: Reload Project
1. In Rider, go to **File** → **Invalidate Caches** → **Invalidate and Restart**
2. Or simply close and reopen the project

### Step 2: Select Debug Configuration

**For Emulator Debugging:**
1. At the top of Rider, find the Run Configuration dropdown (shows current config)
2. Select **Maui.Popes - Android Debug**
3. Ensure your Android Emulator is running
4. Click the **Debug** button (green bug icon) or press `Cmd+D`

**For Physical Device Debugging:**
1. Connect your Android device via USB
2. Enable USB debugging on your device
3. Verify connection: Run in terminal: `adb devices`
4. Select **Maui.Popes - Android Device** from the dropdown
5. Click the **Debug** button or press `Cmd+D`

### Step 3: Start Debugging
- **Green Play Button** (▶) = Run app without debugger
- **Green Bug Button** (🐛) = Run app with debugger attached

## Debug Features Available

### Breakpoints
1. Click in the gutter (left margin) of any `.cs` file to set a breakpoint
2. The red dot indicates an active breakpoint
3. App will pause at breakpoint when reached

### Variable Inspection
- Hover over variables to see their current values
- Use the **Debug** tool window (View → Tool Windows → Debug) to inspect:
  - Local variables
  - Watch expressions
  - Call stack

### Logcat Monitoring
- View Android logs: **View** → **Tool Windows** → **Logcat**
- Filter by your app: Search for `com.peterrexj.popesofchurch`
- See debug output, errors, and warnings in real-time

### Step Debugging
- **F7** - Step Into
- **F8** - Step Over
- **Shift+F8** - Step Out
- **F9** - Resume execution

## Troubleshooting

### Configuration Not Appearing
1. Invalidate caches and restart Rider
2. Make sure `.idea/runConfigurations/` folder exists
3. Reload the project from disk

### Debugger Not Attaching
1. Check emulator/device is properly connected: `adb devices`
2. Ensure app is built for Debug: Set Configuration to **Debug** in top menu
3. Try restarting the emulator or reconnecting the device

### Slow Deployment
- Use the emulator (faster than device) during development
- These configurations have `USE_FAST_DEPLOYMENT=true` enabled
- First deploy is always slower; subsequent ones are faster due to caching

### Can't Find Breakpoints
- Make sure you're debugging (not just running)
- Verify debug symbols are being generated (check build output)
- Try doing a Clean Build: **Build** → **Clean**

## Project Settings Optimized for Debugging

The Debug PropertyGroup includes:

```xml
<DebugSymbols>true</DebugSymbols>
<DebugType>portable</DebugType>
<Optimize>false</Optimize>
<AndroidUseSharedRuntime>true</AndroidUseSharedRuntime>
<AndroidLinkMode>None</AndroidLinkMode>
<AndroidEnableProguard>false</AndroidEnableProguard>
<AndroidEnableR8FullMode>false</AndroidEnableR8FullMode>
<EmbedAssembliesIntoApk>false</EmbedAssembliesIntoApk>
```

These settings:
- ✅ Preserve all debug information
- ✅ Disable code obfuscation
- ✅ Speed up builds and deployments
- ✅ Make debugging more reliable

## Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Start Debug | `Cmd+D` |
| Run | `Cmd+R` |
| Step Into | `F7` |
| Step Over | `F8` |
| Step Out | `Shift+F8` |
| Resume | `F9` |
| Toggle Breakpoint | `Cmd+F8` |
| View Debug Window | `Cmd+5` |
| View Logcat | `Cmd+6` |

## Next Steps

1. **Reload Rider** to load the new configurations
2. **Select the debug configuration** from the dropdown
3. **Connect your emulator or device**
4. **Press Cmd+D to start debugging**
5. **Set breakpoints** and watch your code execute!

## Important Notes

- These configurations are machine-specific (paths are relative)
- The app package name is: `com.peterrexj.popesofchurch`
- Target framework: `net9.0-android36.0`
- Debug builds are larger and slower than release builds (this is normal!)

Happy debugging! 🐛🔍

