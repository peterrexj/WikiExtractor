# Scan obj/**/lp and bin for files that are expected to be managed assemblies but are not PE images
# Usage: Open PowerShell and run: .\Tools\find-nonpe.ps1 -Root "d:\Pro\WikiExtractor"
param(
    [string]$Root = "d:\Pro\WikiExtractor",
    [switch]$VerboseOutput
)

$patterns = @(
    "**\obj\**\lp\**\*",
    "**\bin\**\*.dll",
    "**\obj\**\*.dll",
    "**\obj\**\*.jar",
    "**\obj\**\*.*"
)

Write-Output "Scanning for candidate files under: $Root"

# Use Get-ChildItem with -File and recurse, filter to relevant directories
$items = Get-ChildItem -Path $Root -Recurse -File -ErrorAction SilentlyContinue | Where-Object {
    $_.FullName -match "\\obj\\.*\\lp\\" -or $_.FullName -match "\\obj\\.*\\.*\.dll$" -or $_.FullName -match "\\bin\\.*\\.*\.dll$"
}

if (-not $items) {
    Write-Output "No candidate files found under obj/lp or bin (they may not be generated yet). Try building first and re-run this script."
    return
}

$nonPe = @()
foreach ($f in $items) {
    try {
        $fs = [System.IO.File]::Open($f.FullName, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
        $br = New-Object System.IO.BinaryReader($fs)
        $sig = $br.ReadBytes(2)
        $fs.Close()
        if ($sig.Length -lt 2 -or $sig[0] -ne 0x4D -or $sig[1] -ne 0x5A) { # 'MZ'
            $nonPe += $f.FullName
            if ($VerboseOutput) { Write-Output "NON-PE: $($f.FullName)" }
        } else {
            if ($VerboseOutput) { Write-Output "PE: $($f.FullName)" }
        }
    } catch {
        Write-Output "Error reading $($f.FullName): $($_.Exception.Message)"
    }
}

if ($nonPe.Count -eq 0) {
    Write-Output "All scanned files start with 'MZ' (likely PE). No obvious non-PE files found among candidate files."
} else {
    Write-Output "Found $($nonPe.Count) files that are not PE images (do not start with 'MZ'):`n"
    $nonPe | Sort-Object | ForEach-Object { Write-Output " - $_" }
}

Write-Output "Done. To continue, you can: (1) inspect the listed files, (2) remove or exclude the offending file(s) from packaging, or (3) re-run the build and capture the exact filename in the ProcessAssemblies step."