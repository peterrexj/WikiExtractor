
#Set-ExecutionPolicy Bypass
# Function to push AAB file to Sauce Labs
function Push-AabToSauceLabs {
    param (
        [Parameter(Mandatory = $true)]
        [string] $AabFilePath,
        [string] $ApiKey = "d2d0caa7-58c6-414d-9107-9a54297cfbdf",
        [string] $Username = "peterrexj",
        [string] $Description
    )

    $apiUrl = "https://api.us-west-1.saucelabs.com/v1/storage/upload"
    $base64Auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$($Username):$($ApiKey)"))

    $headers = @{
        "Authorization" = "Basic $base64Auth"
        "Content-Type" = "application/octet-stream"
    }

    # Additional parameters required by Sauce Labs API
    $parameters = @{
        "payload" = Get-Content -Path $AabFilePath -Raw
        "name" = [IO.Path]::GetFileName($AabFilePath)
        "description" = $Description
    }

    try {
        # Check if AAB file exists
        if (Test-Path -Path $AabFilePath -PathType Leaf) {
            Write-Output "AAB file exists: $AabFilePath"

            # Upload AAB file to Sauce Labs
            $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Headers $headers -Body $parameters

            # Output the response
            Write-Output "Upload to Sauce Labs successful:"
            $response
        }
        else {
            Write-Output "AAB file not found: $AabFilePath"
        }
    }
    catch {
        Write-Error "Failed to push AAB file to Sauce Labs: $_"
    }
}

function Build-XamarinAndroidProject {
    param (
        [string] $ProjectFilePath,
        [string] $outputFolder,
        [string] $aabFilePath,
        [string] $androidSigningKeyAlias,
        [string] $androidSigningKeyPass,
        [string] $androidSigningStorePass,
        [string] $androidSigningKeyStore
    )

    # Combine paths and define output folder structure
    $msbuildPath = "C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MSBuild.exe"

    # Create the output folder if it doesn't exist
    if (-not (Test-Path -Path $outputFolder -PathType Container)) {
        New-Item -Path $outputFolder -ItemType Directory | Out-Null
    }

    # Build the Xamarin.Android project
    & $msbuildPath $ProjectFilePath /p:OutputPath=$outputFolder `
        /t:PackageForAndroid /p:Configuration=Release /p:AndroidPackageFormat=aab `
        /p:AndroidKeyStore=true /p:AndroidSigningKeyAlias=$androidSigningKeyAlias `
        /p:AndroidSigningKeyPass=$androidSigningKeyPass /p:AndroidSigningStorePass=$androidSigningStorePass `
        /p:AndroidSigningKeyStore=$androidSigningKeyStore

    if (Test-Path -Path $aabFilePath -PathType Leaf) {
        Write-Output "AAB file generated at: $aabFilePath" 
    } else {
        Write-Output "AAB file not found: $aabFilePath"
    }
}

$AndroidSigningKeyAlias = "keyalias.alias"
$AndroidSigningKeyPass = "CatholicSaintsPassword@01"
$AndroidSigningStorePass = "CatholicSaintsPassword@01"
$AndroidSigningKeyStore = [IO.Path]::Combine($PSScriptRoot, '..\Resources\droidCerts\catholicsaints.keystore')

$outputRootFolder = [IO.Path]::Combine($PSScriptRoot, '..\Builds\')
$dateOfBuild = (Get-Date -Format "yyyyMMdd_HHmmss")

$outputFolderSaints = Join-Path -Path $OutputRootFolder -ChildPath "Saints"
$outputFolderSaints = Join-Path -Path $outputFolderSaints -ChildPath $dateOfBuild
$outputFolderSaintsAabFile = Join-Path -Path $outputFolderSaints -ChildPath "com.pj.christiancatholicsaints.aab"
$saints = [IO.Path]::Combine($PSScriptRoot, '..\src\WikiExtractor.XamarinForms.App\ChristianCatholicSaints\ChristianCatholicSaints.Android\ChristianCatholicSaints.Android.csproj')

Build-XamarinAndroidProject -ProjectFilePath $saints -outputFolder $outputFolderSaints -aabFilePath $outputFolderSaintsAabFile -androidSigningKeyAlias $AndroidSigningKeyAlias -androidSigningKeyPass $AndroidSigningKeyPass -androidSigningStorePass $AndroidSigningStorePass -androidSigningKeyStore $AndroidSigningKeyStore


# if (Test-Path -Path $outputFolderSaintsAabFile -PathType Leaf) {
#     Push-AabToSauceLabs -AabFilePath $outputFolderSaintsAabFile -Description "Catholic saints app for testing"
# } else {
#     Write-Output "AAB file not found: $outputFolderSaintsAabFile"
# }