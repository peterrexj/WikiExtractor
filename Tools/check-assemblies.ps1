$path = 'D:\Pro\WikiExtractor\src\Maui\Apps\Maui.Popes\bin\Debug\net9.0-android'
Get-ChildItem -Path $path -Filter *.dll | ForEach-Object {
    $full = $_.FullName
    try {
        [Reflection.AssemblyName]::GetAssemblyName($full) | Write-Output
    } catch {
        Write-Output "BAD: $full -> $($_.Exception.Message)"
    }
}