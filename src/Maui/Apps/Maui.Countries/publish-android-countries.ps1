$Project  = "Maui.Countries.csproj"
$BundleId = "com.pj.countriesofworld"

# ── Resolve Android target framework from csproj ─────────────────────────────
$xml = [xml](Get-Content $Project)
$AndroidTF = $xml.SelectNodes("//*[local-name()='TargetFrameworks' or local-name()='TargetFramework']") |
             ForEach-Object { $_.InnerText -split '[;\s]+' } |
             Where-Object { $_ -match '^net\d+\.\d+-android' } |
             Select-Object -First 1
if (-not $AndroidTF) { Write-Error "Could not determine Android TargetFramework from $Project"; exit 1 }
$AndroidTF = $AndroidTF.Trim()
Write-Host "==> Target framework: $AndroidTF"

$OutDir = "bin/Release/$AndroidTF/publish"

# ── Discover Java ─────────────────────────────────────────────────────────────
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
if ($env:JAVA_HOME) { Write-Host "==> Using Java: $env:JAVA_HOME" }
else { Write-Host "WARNING: Java not found, using system default" -ForegroundColor Yellow }

# ── Locate Android SDK ───────────────────────────────────────────────────────
$SdkRoot = $env:ANDROID_HOME
if (-not $SdkRoot) { $SdkRoot = $env:ANDROID_SDK_ROOT }
if (-not $SdkRoot) {
    foreach ($c in @("$env:LOCALAPPDATA\Android\Sdk", "$env:USERPROFILE\AppData\Local\Android\Sdk", "$HOME/Library/Android/sdk", "$HOME/Android/Sdk")) {
        if (Test-Path $c) { $SdkRoot = $c; break }
    }
}
if (-not $SdkRoot) { Write-Error "Android SDK not found. Set ANDROID_HOME or ANDROID_SDK_ROOT."; exit 1 }
Write-Host "==> Android SDK: $SdkRoot"

# ── Publish ──────────────────────────────────────────────────────────────────
Write-Host "==> Publishing release AAB..."
dotnet publish $Project -f $AndroidTF -c Release -p:AndroidSdkDirectory=$SdkRoot
if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit 1 }

# ── Locate output AAB ────────────────────────────────────────────────────────
$aab = Get-ChildItem $OutDir -Filter "$BundleId-Signed.aab" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $aab) { $aab = Get-ChildItem $OutDir -Filter "*.aab" -ErrorAction SilentlyContinue | Select-Object -First 1 }
if (-not $aab) { Write-Error "AAB not found under $OutDir"; exit 1 }

# ── Locate debug symbols zip ─────────────────────────────────────────────────
$sym = Get-ChildItem $OutDir -Filter "*-symbols.zip" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $sym) { $sym = Get-ChildItem "bin/Release/$AndroidTF" -Filter "*-symbols.zip" -ErrorAction SilentlyContinue | Select-Object -First 1 }

# ── Locate R8 mapping file ────────────────────────────────────────────────────
$map = Get-Item "bin/Release/$AndroidTF/mapping.txt" -ErrorAction SilentlyContinue
if (-not $map) { $map = Get-ChildItem "bin/Release/$AndroidTF" -Filter "mapping.txt" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1 }

Write-Host ""
Write-Host "==> Done. Upload this file to the Play Store:" -ForegroundColor Green
Write-Host "    $($aab.FullName)" -ForegroundColor Cyan
if ($map) {
    Write-Host ""
    Write-Host "==> Upload this R8 mapping file in Play Console (Android vitals > Deobfuscation files):" -ForegroundColor Green
    Write-Host "    $($map.FullName)" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "WARNING: mapping.txt not found. Only present when AndroidLinkTool=r8." -ForegroundColor Yellow
}
if ($sym) {
    Write-Host ""
    Write-Host "==> Upload this symbols file in Play Console (App bundle explorer > Downloads > Native debug symbols):" -ForegroundColor Green
    Write-Host "    $($sym.FullName)" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "WARNING: Symbols zip not found. Only present when AndroidIncludeDebugSymbols=true and AOT is enabled." -ForegroundColor Yellow
}
