param(
    [string]$EmulatorName = "Medium_Phone_API_36.1"
)

$Project   = "Maui.Countries.csproj"
$BundleId  = "com.pj.countriesofworld"

# ── Java 21 via Homebrew (required for Android manifest merger) ──────────────
$javaHome = "/opt/homebrew/opt/openjdk@21"
if (Test-Path $javaHome) {
    $env:JAVA_HOME = $javaHome
    $env:PATH = "$javaHome/bin:$env:PATH"
    Write-Host "==> Using Java: $javaHome"
} else {
    Write-Host "WARNING: Java 21 not found at $javaHome, using system default" -ForegroundColor Yellow
}

$SdkRoot = $env:ANDROID_HOME
if (-not $SdkRoot) { $SdkRoot = $env:ANDROID_SDK_ROOT }
if (-not $SdkRoot) {
    $candidates = @(
        "$env:LOCALAPPDATA\Android\Sdk",
        "$env:USERPROFILE\AppData\Local\Android\Sdk",
        "$HOME/Library/Android/sdk",
        "$HOME/Android/Sdk"
    )
    foreach ($c in $candidates) {
        if (Test-Path $c) { $SdkRoot = $c; break }
    }
}
if (-not $SdkRoot) { Write-Error "Android SDK not found."; exit 1 }

$EmulatorExe = Join-Path $SdkRoot "emulator/emulator"
$AdbExe      = Join-Path $SdkRoot "platform-tools/adb"
if ($IsWindows -or $env:OS -eq "Windows_NT") { $EmulatorExe += ".exe"; $AdbExe += ".exe" }

Write-Host "==> Finding emulator: $EmulatorName..."
$avds = & $EmulatorExe -list-avds 2>$null
if ($avds -notcontains $EmulatorName) {
    Write-Host "ERROR: No emulator found matching '$EmulatorName'" -ForegroundColor Red
    $avds | ForEach-Object { Write-Host "    $_" }
    exit 1
}

Write-Host "==> Checking emulator state..."
$running = & $AdbExe devices | Select-String "emulator" | Select-String "device$"
if (-not $running) {
    Write-Host "    Starting emulator '$EmulatorName'..."
    $startArgs = @{ FilePath = $EmulatorExe; ArgumentList = "-avd", $EmulatorName }
    if ($IsWindows -or $env:OS -eq "Windows_NT") { $startArgs["WindowStyle"] = "Hidden" }
    Start-Process @startArgs
    Write-Host "    Waiting for device to come online..."
    & $AdbExe wait-for-device
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

$serial = (& $AdbExe devices | Select-String "emulator" | Select-String "device$" | Select-Object -First 1).ToString().Split("`t")[0].Trim()
Write-Host "    Serial: $serial"

Write-Host "==> Building..."
dotnet build $Project -f net9.0-android36.0 -c Debug -p:AndroidSdkDirectory=$SdkRoot -p:EmbedAssembliesIntoApk=true
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "==> Finding APK..."
$apk = Get-ChildItem "bin/Debug/net9.0-android36.0" -Filter "*-Signed.apk" -Recurse | Select-Object -First 1
if (-not $apk) { $apk = Get-ChildItem "bin/Debug/net9.0-android36.0" -Filter "*.apk" -Recurse | Select-Object -First 1 }
if (-not $apk) { Write-Error "APK not found"; exit 1 }
Write-Host "    APK: $($apk.FullName)"

Write-Host "==> Stopping existing app..."
& $AdbExe -s $serial shell am force-stop $BundleId

Write-Host "==> Installing..."
& $AdbExe -s $serial install -r $apk.FullName
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "==> Launching..."
$dumpOutput = & $AdbExe -s $serial shell pm dump $BundleId 2>$null
$mainActivity = $dumpOutput | Select-String "MainActivity filter" | Select-Object -First 1 |
                ForEach-Object { $_.Line.Trim() -replace '\s+', ' ' } |
                ForEach-Object { ($_ -split ' ')[1] }
if (-not $mainActivity) { Write-Error "Could not determine MainActivity"; exit 1 }
Write-Host "    Activity: $mainActivity"
& $AdbExe -s $serial shell am start -n $mainActivity

Write-Host "==> Done"
