############################################################
 
# Author: Douglas Loyo, Sr. Solutions Architect @ MSDF, Xavier Lucatero Sr. .Net Developer @ Near Shore Devs LLC
 
# Description: Downloads Ed-Fi binaries from the published MyGet feed and installs them.
#              After install it does appropriate configuration to have applications running.

# Note: This powershell has to be ran with Elevated Permissions (As Administrator) and in a x64 environment.
# Know issues and future todo's: (look at the .PSM1 file)
 
############################################################

Import-Module "$PSScriptRoot\Config" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\IIS" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\Prettify" -Force #-Verbose #-Force

Function Install-Prerequisites() {
    $allPreReqsInstalled = $true
    
    Write-Host "Ensurering all Prerequisites are installed:"

    # Ensure the following are installed.
    Install-Chocolatey
    #Install-IISPrerequisites
    #Install-IISUrlRewrite
    
    #Install-NetFramework48
    

    # MsSQL Server
    #if (!(Test-Path 'HKLM:\Software\Microsoft\Microsoft SQL Server\Instance Names\SQL')) { $allPreReqsInstalled = $false; Write-Host "     Prerequisite not installed: MsSQL-Server" }

    # If not all Pre Reqs installed halt!
    if(!$allPreReqsInstalled){ Write-Error "Error: Missing Prerequisites. Look above for list." -ErrorAction Stop }
}

Write-HostInfo "MSDF Student Engagement Tracker Installer"
Write-Host "To install the MSDF Student Engagement Tracker run one of the following commands:" 
Write-HostStep " Install everything"
Write-Host "    Install-StudentEngagementTracker" 
Write-HostStep " Only Install Web API"
Write-Host "    Install-StudentEngagementTrackerAPI" 
Write-HostStep " Only Install the Chrome Plugin"
Write-Host "    Install-StudentEngagementTrackerChromePlugin" 

$config = Get-ConfigurationParameters
Write-Host "Key: " $config.encryptionKey