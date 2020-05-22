Import-Module "$PSScriptRoot\Config" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\IIS" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\Prettify" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\ImportStudentInformation" -Force -DisableNameChecking #-Verbose #-Force

Function Install-Chocolatey(){
    if(!(Test-Path "$($env:ProgramData)\chocolatey\choco.exe"))
    {
        Write-Host "Installing: Cocholatey..."
        Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
    }else{Write-Host "Skipping: Cocholatey is already installed."}
}
Function Install-Prerequisites() {
    $allPreReqsInstalled = $true
    
    Write-Host "Ensurering all Prerequisites are installed:"

    # Ensure the following are installed.
    Install-Chocolatey
    Install-IISPrerequisites
    
    #install db libraries
    choco install mysql-connector
    Install-Module -Name SqlServer

    # MsSQL Server
    #if (!(Test-Path 'HKLM:\Software\Microsoft\Microsoft SQL Server\Instance Names\SQL')) { $allPreReqsInstalled = $false; Write-Host "     Prerequisite not installed: MsSQL-Server" }

    # If not all Pre Reqs installed halt!
    if(!$allPreReqsInstalled){ Write-Error "Error: Missing Prerequisites. Look above for list." -ErrorAction Stop }
}

Function Set-AppSettingsJsonFile($jsonFilePath, $config) {
    $appSettings = Get-Content $jsonFilePath -raw | ConvertFrom-Json

    $appSettings.update | % {$appSettings.ConnectionStrings.DefaultConnection = $config.BinaryMetadata.ApiBinaries.ConnectionString.StudentLearningEventsConnectionString}
    $appSettings.update | % {$appSettings.encryptionExportedKey = $config.EncryptionKey}
    
    $appSettings | ConvertTo-Json -depth 32| set-content $jsonFilePath
}

Function Install-Binaries($settings, $tempPathForBinaries){
    $filePath = $tempPathForBinaries + "\" + $settings.Name + ".zip"
    # Optimization (Caching Packages): Check to see if file exists. If it does NOT then download.
    if(!(Test-Path $filePath -PathType Leaf)) {
        Write-Host "     Downloading " $settings.BinaryUrl " to -> " $filePath
        Invoke-WebRequest -Uri $settings.BinaryUrl -OutFile $filePath
    }
    #2.1) Once downloaded unzip to install path.
    $installPath = $settings.InstallPathForBinary
    Write-Host "     Installing to -> $installPath"
    Expand-Archive -LiteralPath $filePath -DestinationPath $installPath -Force

}

# All Install Options go here
Function Install-StudentEngagementTracker() {
    # Get all the configuration parameters
    $config = Get-ConfigurationParameters
    
    Write-HostStep "Installing the full Student Engagement Stack"
    # Ensure all prerequisits are in place.
    Install-Prerequisites


    #1) Ensure temp path is accessible and exists if not create it.
    Write-HostStep "Step: Ensuring temp path is accessible. " $config.TempPathForBinaryDownloads
    New-Item -ItemType Directory -Force -Path $config.TempPathForBinaryDownloads

    #2) Download necesarry binaries and install them to their final install location.
    Install-Binaries $config.BinaryMetadata.ApiBinaries $config.TempPathForBinaryDownloads
    Install-Binaries $config.BinaryMetadata.ChromeExtensionUrl $config.TempPathForBinaryDownloads

    #3) IIS Settings
    # Create Web Site / Web Application
    Write-HostStep "Step: IIS Creating WebApplications and Configuring Authetication Settings"
    $installPath = $config.BinaryMetadata.ApiBinaries.InstallPathForBinary
    New-WebApplication -Name $config.IISSettings.WebSiteName  -Site $config.IISSettings.ParentSiteName -PhysicalPath $installPath -ApplicationPool $config.IISSettings.ApplicationPool -Force

    # Set IIS Authentication settings
    $applicationIISPath = $config.IISSettings.ParentSiteName + "/" + $config.IISSettings.WebSiteName
    Write-Host "     Setting IIS Auth " $applicationIISPath
    Set-WebConfigurationProperty -Filter "/system.webServer/security/authentication/anonymousAuthentication" -Name Enabled -Value "true" -PSPath IIS:\ -Location $applicationIISPath
    Set-WebConfigurationProperty -Filter "/system.webServer/security/authentication/windowsAuthentication" -Name Enabled -Value "false" -PSPath IIS:\ -Location $applicationIISPath

    #4) Update appSettings.json params
    Write-HostStep "Step: IIS Configuring appSettings.json, connectionStrings, logfiles etc..."
    $appSettingsPath = $installPath + "\appsettings.json"
    Set-AppSettingsJsonFile $appSettingsPath $config

    #5) StudentInformation ETL
    Write-HostStep "Step: Importing  StudentInformation"
    $sourceConnStr = $config.BinaryMetadata.ApiBinaries.ConnectionString.EdFiODSConnectionString 
    $destConnStr = $config.BinaryMetadata.ApiBinaries.ConnectionString.StudentLearningEventsConnectionString 
    Import-StudentInfo $sourceConnStr $destConnStr
 
    Write-HostStep "Step: Opening webapi"
    Start-Process "https://localhost/StudentEngagement/api/LearningActivityEvents"
}