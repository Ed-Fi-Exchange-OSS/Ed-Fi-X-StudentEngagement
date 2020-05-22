############################################################
 
# Author: Douglas Loyo, Sr. Solutions Architect @ MSDF, Xavier Lucatero Sr. .Net Developer @ Near Shore Devs LLC
 
# Description: Downloads Ed-Fi binaries from the published MyGet feed and installs them.
#              After install it does appropriate configuration to have applications running.

# Note: This powershell has to be ran with Elevated Permissions (As Administrator) and in a x64 environment.
# Know issues and future todo's: (look at the .PSM1 file)
 
############################################################

Import-Module "$PSScriptRoot\InstallSteps" -Force -DisableNameChecking #-Verbose #-Force
Import-Module "$PSScriptRoot\Config" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\IIS" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\Prettify" -Force #-Verbose #-Force

Write-HostInfo "MSDF Student Engagement Tracker Installer"
Write-Host "To install the MSDF Student Engagement Tracker run one of the following commands:" 
Write-HostStep " Install everything"
Write-Host "    Install-StudentEngagementTracker" 
Write-HostStep " Only Install Web API"
Write-Host "    Install-StudentEngagementTrackerAPI" 
Write-HostStep " Only Install the Chrome Plugin"
Write-Host "    Install-StudentEngagementTrackerChromePlugin" 

$config = Get-ConfigurationParameters
if ($config.BinaryMetadata.ApiBinaries.ConnectionString.StudentLearningEventsConnectionString.Trim().Length -eq 0) {
    Write-Warning "StudentLearningEventsConnectionString is empty. Configure a connection string in the $PSScriptRoot\config.json file for the destination database"
}
if ($config.BinaryMetadata.ApiBinaries.ConnectionString.EdFiODSConnectionString.Trim().Length -eq 0) {
    Write-Warning "EdFiODSConnectionString is empty. Configure a connection string in the $PSScriptRoot\config.json file for the MSSQL EdFi database"
}

Write-Host "Key: " $config.encryptionKey

