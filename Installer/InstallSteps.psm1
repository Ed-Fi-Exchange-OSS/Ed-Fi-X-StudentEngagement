Import-Module "$PSScriptRoot\Config" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\IIS" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\Prettify" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\ImportStudentInformation" -Force -DisableNameChecking #-Verbose #-Force

Function Install-Chocolatey() {
    if (!(Test-Path "$($env:ProgramData)\chocolatey\choco.exe")) {
        Write-Host "Installing: Cocholatey..."
        Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
    }else { Write-Host "Skipping: Cocholatey is already installed." }
}

Function Get-GoogleChromeExePath() {
    return (Get-ItemProperty -ErrorAction Ignore  'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe').'(Default)'
}


Function Install-GoogleChrome() {
    $exePath = Get-GoogleChromeExePath
    if ( $exePath.Length -eq 0 ) {
    #$installed = (Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*)
    #if( $null -eq ($installed | Where-Object { $_.DisplayName -eq "Google Chrome" }) ){    
        Write-Host
        Write-Host "Installing Google Chrome web browser"
        choco install googlechrome -y 
    }
}
Function Add-GoogleExtensionToExtensionInstallForcelist($extensionId) {
    $regLocation = 'Software\Policies\Google\Chrome\ExtensionInstallForcelist'
    If (!(Test-Path "HKCU:\$regLocation")) {
        [int]$count = 0
        New-Item -Path "HKCU:\$regLocation" -Force
    }
    Else {
        if( ((Get-ItemProperty "HKCU:\$regLocation").PSObject.Properties | Where-Object { $_.Value -like 'bfnmmobhknbpakaapfddhbpofiidofdg;*' }).Count -gt 0){
            return
        }
        [int]$count = (((Get-ItemProperty "HKCU:\$regLocation").PSObject.Properties | 
            #Check if the property name is an int
            Where-Object { $_.Name -match "^[\d\.]+$" }) | 
            #Convert property name to int
            ForEach-Object -Process { [int]$_.Name } | 
            #Get Maximum
            Measure-Object -Maximum).Maximum
    }
    $regName = $count + 1
    $regValue = "$extensionId;https://clients2.google.com/service/update2/crx"
    $null = New-ItemProperty -Path "HKCU:\$regLocation" -Name $regName -Value $regValue -PropertyType STRING -Force
}

Function Install-Prerequisites() {
    $allPreReqsInstalled = $true
    
    Write-Host "Ensurering all Prerequisites are installed:"

    # Ensure the following are installed.
    Install-Chocolatey
    Install-IISPrerequisites
    Install-GoogleChrome
    
    #install db libraries
    #choco install mysql-connector -y
    
    #install PostGreSQL odbc driver
    choco install psqlodbc -y
    
    # TODO: Ensure that MSSQL module dont exists before installing
    if ( $null -eq (Get-InstalledModule -Name "SQLServer" -ErrorAction Ignore)) {         
        Write-Host "Adding powershell SQLServer module"
        $null = Install-PackageProvider -Name NuGet -Force
        Install-Module -Name SqlServer -Force
    }

    # MsSQL Server
    #if (!(Test-Path 'HKLM:\Software\Microsoft\Microsoft SQL Server\Instance Names\SQL')) { $allPreReqsInstalled = $false; Write-Host "     Prerequisite not installed: MsSQL-Server" }

    # If not all Pre Reqs installed halt!
    if (!$allPreReqsInstalled) { Write-Error "Error: Missing Prerequisites. Look above for list." -ErrorAction Stop }
}

Function Set-AppSettingsJsonFile($jsonFilePath, $config) {
    $appSettings = Get-Content $jsonFilePath -raw | ConvertFrom-Json

    $appSettings.update | % { $appSettings.ConnectionStrings.DefaultConnection = $config.BinaryMetadata.ApiBinaries.ConnectionString.StudentLearningEventsConnectionString }
    $appSettings.update | % { $appSettings.encryptionExportedKey = $config.EncryptionKey }
    
    $appSettings | ConvertTo-Json -depth 32 | set-content $jsonFilePath
}

Function Install-Binaries($settings, $tempPathForBinaries) {
    $filePath = $tempPathForBinaries + "\" + $settings.Name + ".zip"
    # Optimization (Caching Packages): Check to see if file exists. If it does NOT then download.
    if (!(Test-Path $filePath -PathType Leaf)) {
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
    
    $destConnStr = $config.BinaryMetadata.ApiBinaries.ConnectionString.StudentLearningEventsConnectionString 
    if ($destConnStr.Trim().Length -eq 0) {
        Write-Warning "StudentLearningEventsConnectionString is empty. Configure a connection string in the $PSScriptRoot\config.json for the destination database. Terminating script"
        return
    }
    
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
    if ($sourceConnStr.Trim().Length -eq 0) {
        Write-Warning "EdFiODSConnectionString is empty. Configure a connection string for the MSSQL EdFi database"
    }
    if ($destConnStr.Trim().Length -gt 0 -and $sourceConnStr.Trim().Length -gt 0) {
        Import-StudentInfo $sourceConnStr $destConnStr
    }

    #6) Install extension
    Write-HostStep "Step: Adding Chrome extension "
    Add-GoogleExtensionToExtensionInstallForcelist "bfnmmobhknbpakaapfddhbpofiidofdg"
 
    Write-HostStep "Step: Opening webapi"
    $chromeExe = Get-GoogleChromeExePath
    Start-Process $chromeExe  "https://localhost/StudentEngagement/api/LearningActivityEvents"
}
