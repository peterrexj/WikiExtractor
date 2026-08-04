#Requires -Version 7
<#
.SYNOPSIS
    Clean, publish, and collect Android release bundles for all 4 MAUI apps.

.DESCRIPTION
    1. Deletes bin/ and obj/ for all 4 apps and confirms removal.
    2. Publishes each app sequentially, stopping on any failure.
    3. Copies each app's publish output to:
           Output/Android/<AppName>/publish/
    4. Validates expected files (AAB required; mapping + symbols optional).
    5. Prints a formatted summary table.

.EXAMPLE
    pwsh publish-android-all.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ── App definitions ────────────────────────────────────────────────────────────
$Apps = @(
    [pscustomobject]@{ Name = "Maui.Popes";       BundleId = "com.pj.popesofchurch";           Dir = "Maui.Popes";       Csproj = "Maui.Popes.csproj" }
    [pscustomobject]@{ Name = "Maui.Saints";      BundleId = "com.pj.christiancatholicsaints";  Dir = "Maui.Saints";      Csproj = "Maui.Saints.csproj" }
    [pscustomobject]@{ Name = "Maui.Countries";   BundleId = "com.pj.countriesofworld";          Dir = "Maui.Countries";   Csproj = "Maui.Countries.csproj" }
    [pscustomobject]@{ Name = "Maui.WorldLeaders"; BundleId = "com.pj.worldleadershub";          Dir = "Maui.WorldLeaders"; Csproj = "Maui.WorldLeaders.csproj" }
)

$ScriptDir  = $PSScriptRoot
$OutputRoot = Join-Path $ScriptDir "Output/Android"

# ── Helper: section header ─────────────────────────────────────────────────────
function Write-Section([string]$Title) {
    Write-Host ""
    Write-Host ("─" * 70) -ForegroundColor DarkGray
    Write-Host "  $Title" -ForegroundColor White
    Write-Host ("─" * 70) -ForegroundColor DarkGray
}

# ── Resolve Java ───────────────────────────────────────────────────────────────
Write-Section "Environment"
if (-not $env:JAVA_HOME) {
    $javaHome = $null
    if ($IsMacOS -or $env:OS -ne "Windows_NT") {
        foreach ($brew in @("/opt/homebrew/opt", "/usr/local/opt")) {
            if (Test-Path $brew) {
                $javaHome = Get-ChildItem $brew -Filter "openjdk*" -ErrorAction SilentlyContinue |
                            Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
                if ($javaHome) { break }
            }
        }
    } else {
        foreach ($base in @("$env:ProgramFiles\Java", "$env:ProgramFiles\Eclipse Adoptium", "$env:ProgramFiles\Microsoft", "$env:ProgramFiles\OpenJDK")) {
            if (Test-Path $base) {
                $javaHome = Get-ChildItem $base -Filter "jdk*" -ErrorAction SilentlyContinue |
                            Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
                if ($javaHome) { break }
            }
        }
    }
    if ($javaHome -and (Test-Path $javaHome)) {
        $env:JAVA_HOME = $javaHome
        $env:PATH = "$javaHome/bin$([IO.Path]::PathSeparator)$env:PATH"
    }
}
if ($env:JAVA_HOME) { Write-Host "  Java  : $env:JAVA_HOME" -ForegroundColor Cyan }
else                { Write-Host "  WARNING: JAVA_HOME not set, using system default" -ForegroundColor Yellow }

# ── Resolve Android SDK ────────────────────────────────────────────────────────
$SdkRoot = $env:ANDROID_HOME
if (-not $SdkRoot) { $SdkRoot = $env:ANDROID_SDK_ROOT }
if (-not $SdkRoot) {
    foreach ($c in @("$env:LOCALAPPDATA\Android\Sdk", "$env:USERPROFILE\AppData\Local\Android\Sdk",
                     "$HOME/Library/Android/sdk", "$HOME/Android/Sdk")) {
        if (Test-Path $c) { $SdkRoot = $c; break }
    }
}
if (-not $SdkRoot) { Write-Error "Android SDK not found. Set ANDROID_HOME or ANDROID_SDK_ROOT."; exit 1 }
Write-Host "  SDK   : $SdkRoot" -ForegroundColor Cyan
Write-Host "  Output: $OutputRoot" -ForegroundColor Cyan

# ════════════════════════════════════════════════════════════════════════════════
# PHASE 1 — Clean bin/ and obj/
# ════════════════════════════════════════════════════════════════════════════════
Write-Section "Phase 1 of 3 — Cleaning bin/ and obj/"

foreach ($app in $Apps) {
    $appDir = Join-Path $ScriptDir $app.Dir
    foreach ($folder in @("bin", "obj")) {
        $path = Join-Path $appDir $folder
        if (Test-Path $path) {
            Write-Host "  Removing $($app.Name)/$folder ..." -NoNewline
            Remove-Item $path -Recurse -Force
            if (Test-Path $path) {
                Write-Host " FAILED" -ForegroundColor Red
                Write-Error "Could not delete $path"
                exit 1
            }
            Write-Host " done" -ForegroundColor Green
        } else {
            Write-Host "  $($app.Name)/$folder — not present, skipping" -ForegroundColor DarkGray
        }
    }
}
Write-Host ""
Write-Host "  All bin/ and obj/ folders removed." -ForegroundColor Green

# ════════════════════════════════════════════════════════════════════════════════
# PHASE 2 — Publish each app
# ════════════════════════════════════════════════════════════════════════════════
Write-Section "Phase 2 of 3 — Publishing"

# Collect results for the summary
$Results = [System.Collections.Generic.List[pscustomobject]]::new()

foreach ($app in $Apps) {
    Write-Host ""
    Write-Host "  [$($app.Name)]" -ForegroundColor Cyan

    $appDir = Join-Path $ScriptDir $app.Dir
    Push-Location $appDir

    try {
        # ── Resolve Android target framework ──────────────────────────────────
        $xml = [xml](Get-Content $app.Csproj)
        $AndroidTF = $xml.SelectNodes("//*[local-name()='TargetFrameworks' or local-name()='TargetFramework']") |
                     ForEach-Object { $_.InnerText -split '[;\s]+' } |
                     Where-Object { $_ -match '^net\d+\.\d+-android' } |
                     Select-Object -First 1
        if (-not $AndroidTF) { throw "Could not determine Android TargetFramework from $($app.Csproj)" }
        $AndroidTF = $AndroidTF.Trim()
        Write-Host "    Framework : $AndroidTF"

        # ── dotnet publish ─────────────────────────────────────────────────────
        Write-Host "    Publishing..."
        dotnet publish $app.Csproj -f $AndroidTF -c Release -p:AndroidSdkDirectory=$SdkRoot
        if ($LASTEXITCODE -ne 0) { throw "dotnet publish exited with code $LASTEXITCODE" }

        # ── Locate outputs ─────────────────────────────────────────────────────
        $publishDir = "bin/Release/$AndroidTF/publish"
        $releaseDir = "bin/Release/$AndroidTF"

        $aab = Get-ChildItem $publishDir -Filter "$($app.BundleId)-Signed.aab" -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $aab) { $aab = Get-ChildItem $publishDir -Filter "*.aab" -ErrorAction SilentlyContinue | Select-Object -First 1 }
        if (-not $aab) { throw "AAB not found under $publishDir" }

        $sym = Get-ChildItem $publishDir -Filter "*-symbols.zip" -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $sym) { $sym = Get-ChildItem $releaseDir -Filter "*-symbols.zip" -ErrorAction SilentlyContinue | Select-Object -First 1 }

        $map = Get-Item "$releaseDir/mapping.txt" -ErrorAction SilentlyContinue
        if (-not $map) { $map = Get-ChildItem $releaseDir -Filter "mapping.txt" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1 }

        Write-Host "    AAB       : $($aab.Name)" -ForegroundColor Green
        if ($map) { Write-Host "    Mapping   : $($map.Name)" -ForegroundColor Green }
        if ($sym) { Write-Host "    Symbols   : $($sym.Name)" -ForegroundColor Green }

        $Results.Add([pscustomobject]@{
            Name       = $app.Name
            AndroidTF  = $AndroidTF
            PublishDir = (Resolve-Path $publishDir).Path
            AabPath    = $aab.FullName
            AabName    = $aab.Name
            MapPath    = if ($map) { $map.FullName } else { $null }
            SymPath    = if ($sym) { $sym.FullName } else { $null }
            Status     = "OK"
            Error      = $null
        })
    } catch {
        Write-Host "    ERROR: $_" -ForegroundColor Red
        $Results.Add([pscustomobject]@{
            Name       = $app.Name
            AndroidTF  = ""
            PublishDir = ""
            AabPath    = $null
            AabName    = $null
            MapPath    = $null
            SymPath    = $null
            Status     = "FAILED"
            Error      = "$_"
        })
        Pop-Location
        Write-Error "Publish failed for $($app.Name). Aborting."
        exit 1
    }

    Pop-Location
}

# ════════════════════════════════════════════════════════════════════════════════
# PHASE 3 — Copy to Output/Android/<AppName>/publish/
# ════════════════════════════════════════════════════════════════════════════════
Write-Section "Phase 3 of 3 — Collecting output"

foreach ($r in $Results) {
    $dest = Join-Path $OutputRoot "$($r.Name)/publish"
    Write-Host ""
    Write-Host "  [$($r.Name)] -> $dest" -ForegroundColor Cyan

    # Create destination (clean it first so stale files don't accumulate)
    if (Test-Path $dest) { Remove-Item $dest -Recurse -Force }
    New-Item -ItemType Directory -Path $dest -Force | Out-Null

    $copied = [System.Collections.Generic.List[string]]::new()

    foreach ($src in @($r.AabPath, $r.MapPath, $r.SymPath)) {
        if ($src -and (Test-Path $src)) {
            $destFile = Join-Path $dest (Split-Path $src -Leaf)
            Copy-Item $src $destFile -Force

            if (Test-Path $destFile) {
                $size = (Get-Item $destFile).Length
                $sizeMb = [math]::Round($size / 1MB, 2)
                Write-Host "    [OK] $(Split-Path $src -Leaf) ($sizeMb MB)" -ForegroundColor Green
                $copied.Add((Split-Path $src -Leaf))
            } else {
                Write-Host "    [FAIL] Copy failed: $src" -ForegroundColor Red
                Write-Error "Failed to copy $src to $destFile"
                exit 1
            }
        }
    }

    $r | Add-Member -NotePropertyName CopiedFiles -NotePropertyValue ($copied -join ", ") -Force
}

# ════════════════════════════════════════════════════════════════════════════════
# SUMMARY
# ════════════════════════════════════════════════════════════════════════════════
Write-Section "Summary"
Write-Host ""

$col1 = 18; $col2 = 12; $col3 = 42; $col4 = 20

$header = ("  {0,-$col1} {1,-$col2} {2,-$col3} {3,-$col4}" -f "App", "Status", "AAB File", "Also Copied")
Write-Host $header -ForegroundColor White
Write-Host ("  " + "─" * ($col1 + $col2 + $col3 + $col4 + 3)) -ForegroundColor DarkGray

foreach ($r in $Results) {
    $statusColor = if ($r.Status -eq "OK") { "Green" } else { "Red" }
    $extras = @()
    if ($r.MapPath) { $extras += "mapping.txt" }
    if ($r.SymPath) { $extras += "symbols.zip" }
    $extrasStr = if ($extras.Count -gt 0) { $extras -join ", " } else { "—" }

    $line = "  {0,-$col1} {1,-$col2} {2,-$col3} {3,-$col4}" -f `
        $r.Name, $r.Status, ($r.AabName ?? "—"), $extrasStr
    Write-Host $line -ForegroundColor $statusColor
}

Write-Host ""
Write-Host ("  " + "─" * ($col1 + $col2 + $col3 + $col4 + 3)) -ForegroundColor DarkGray
Write-Host "  Output root: $OutputRoot" -ForegroundColor Cyan
Write-Host ""

$failed = $Results | Where-Object { $_.Status -ne "OK" }
if ($failed) {
    Write-Host "  $($failed.Count) app(s) failed." -ForegroundColor Red
    exit 1
} else {
    Write-Host "  All $($Results.Count) apps published and collected successfully." -ForegroundColor Green
}
