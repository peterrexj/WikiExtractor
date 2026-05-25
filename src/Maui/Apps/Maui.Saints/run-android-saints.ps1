param(
    [string]$EmulatorName = "Medium_Phone_API_36.1"
)

$Project   = "Maui.Saints.csproj"
$BundleId  = "com.peterrexj.christiancatholicsaints"

# ── Java 21 via Homebrew (required for Android manifest merger) ──────────────
$javaHome = "/opt/homebrew/opt/openjdk@21"
if (Test-Path $javaHome) {
    $env:JAVA_HOME = $javaHome
    $env:PATH = "$javaHome/bin:$env:PATH"
    Write-Host "==> Using Java: $javaHome"
} else {
    Write-Host "WARNING: Java 21 not found at $javaHome, using system default" -ForegroundColor Yellow
}

# Locate Android SDK — check env var first, then common paths
$SdkRoot = $env:ANDROID_HOME
if (-not $SdkRoot) { $SdkRoot = $env:ANDROID_SDK_ROOT }
if (-not $SdkRoot) {
    $candidates = @(
        "$env:LOCALAPPDATA\Android\Sdk",           # Windows
        "$env:USERPROFILE\AppData\Local\Android\Sdk",
        "$HOME/Library/Android/sdk",               # macOS
        "$HOME/Android/Sdk"                        # Linux
    )
    foreach ($c in $candidates) {
        if (Test-Path $c) { $SdkRoot = $c; break }
    }
}
if (-not $SdkRoot) {
    Write-Error "Android SDK not found. Set ANDROID_HOME or ANDROID_SDK_ROOT."
    exit 1
}

$EmulatorExe = Join-Path $SdkRoot "emulator/emulator"
$AdbExe      = Join-Path $SdkRoot "platform-tools/adb"

# Windows executables have .exe extension
if ($IsWindows -or $env:OS -eq "Windows_NT") {
    $EmulatorExe += ".exe"
    $AdbExe      += ".exe"
}

# ── Find emulator by name ────────────────────────────────────────────────────
Write-Host "==> Finding emulator: $EmulatorName..."
$avds = & $EmulatorExe -list-avds 2>$null
if ($avds -notcontains $EmulatorName) {
    Write-Host "ERROR: No emulator found matching '$EmulatorName'" -ForegroundColor Red
    Write-Host "Available emulators:"
    $avds | ForEach-Object { Write-Host "    $_" }
    exit 1
}

# ── Boot emulator if not already running ────────────────────────────────────
Write-Host "==> Checking emulator state..."
$running = & $AdbExe devices | Select-String "emulator" | Select-String "device$"

if (-not $running) {
    Write-Host "    Starting emulator '$EmulatorName'..."
    $startArgs = @{ FilePath = $EmulatorExe; ArgumentList = "-avd", $EmulatorName }
    if ($IsWindows -or $env:OS -eq "Windows_NT") { $startArgs["WindowStyle"] = "Hidden" }
    Start-Process @startArgs
    Write-Host "    Waiting for device to come online..."
    & $AdbExe wait-for-device
    # Wait for boot to complete
    $booted = ""
    while ($booted -ne "1") {
        Start-Sleep -Seconds 2
        $booted = & $AdbExe shell getprop sys.boot_completed 2>$null
        $booted = $booted.Trim()
        Write-Host "    Boot status: $booted"
    }
    Write-Host "    Emulator ready."
} else {
    Write-Host "    Emulator already running."
}

# ── Get emulator serial (e.g. emulator-5554) ────────────────────────────────
$serial = (& $AdbExe devices | Select-String "emulator" | Select-String "device$" | Select-Object -First 1).ToString().Split("`t")[0].Trim()
Write-Host "    Serial: $serial"

# ── Build ────────────────────────────────────────────────────────────────────
Write-Host "==> Building..."
dotnet build $Project -f net9.0-android36.0 -c Debug -p:AndroidSdkDirectory=$SdkRoot -p:EmbedAssembliesIntoApk=true
if ($LASTEXITCODE -ne 0) { exit 1 }

# ── Install ──────────────────────────────────────────────────────────────────
Write-Host "==> Finding APK..."
$apk = Get-ChildItem "bin/Debug/net9.0-android36.0" -Filter "*-Signed.apk" -Recurse |
       Select-Object -First 1
if (-not $apk) {
    $apk = Get-ChildItem "bin/Debug/net9.0-android36.0" -Filter "*.apk" -Recurse |
           Select-Object -First 1
}
if (-not $apk) {
    Write-Error "APK not found under bin/Debug/net9.0-android36.0"
    exit 1
}
Write-Host "    APK: $($apk.FullName)"

# Force-stop before reinstall
Write-Host "==> Stopping existing app..."
& $AdbExe -s $serial shell am force-stop $BundleId

Write-Host "==> Installing..."
& $AdbExe -s $serial install -r $apk.FullName
if ($LASTEXITCODE -ne 0) { exit 1 }

# ── Launch ───────────────────────────────────────────────────────────────────
Write-Host "==> Launching..."
$dumpOutput = & $AdbExe -s $serial shell pm dump $BundleId 2>$null
$mainActivity = $dumpOutput |
                Select-String "MainActivity filter" |
                Select-Object -First 1 |
                ForEach-Object { $_.Line.Trim() -replace '\s+', ' ' } |
                ForEach-Object { ($_ -split ' ')[1] }
if (-not $mainActivity) {
    Write-Error "Could not determine MainActivity for $BundleId"
    exit 1
}
Write-Host "    Activity: $mainActivity"
& $AdbExe -s $serial shell am start -n $mainActivity

Write-Host "==> Done"
