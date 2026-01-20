# Usage: .\tools\check-apk-contains-mobileads.ps1 -ApkPath .\path\to\app.apk
param(
  [Parameter(Mandatory=$true)][string]$ApkPath
)

if (-not (Get-Command unzip -ErrorAction SilentlyContinue)) {
  Write-Error "The 'unzip' utility is required. Install it (e.g. via WSL or Git Bash) and retry."
  exit 2
}

$tmp = Join-Path $env:TEMP ([Guid]::NewGuid().ToString())
New-Item -ItemType Directory -Path $tmp | Out-Null
try {
  & unzip -q $ApkPath -d $tmp

  $found = $false
  Get-ChildItem -Path $tmp -Filter *.dex | ForEach-Object {
    $content = & strings.exe $_.FullName 2>$null
    if ($content -match "com/google/android/gms/ads/MobileAdsInitProvider") {
      Write-Host "Found MobileAdsInitProvider in $($_.Name)"
      $found = $true
    }
  }

  if (-not $found) {
    Write-Host "MobileAdsInitProvider NOT found in any classes.dex"
    exit 1
  }
} finally {
  Remove-Item -Recurse -Force $tmp
}