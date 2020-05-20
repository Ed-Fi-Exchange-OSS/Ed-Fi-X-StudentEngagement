Import-Module "$PSScriptRoot\Config" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\IIS" -Force #-Verbose #-Force
Import-Module "$PSScriptRoot\Prettify" -Force #-Verbose #-Force

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
    
    #Install-NetFramework48
    

    # MsSQL Server
    #if (!(Test-Path 'HKLM:\Software\Microsoft\Microsoft SQL Server\Instance Names\SQL')) { $allPreReqsInstalled = $false; Write-Host "     Prerequisite not installed: MsSQL-Server" }

    # If not all Pre Reqs installed halt!
    if(!$allPreReqsInstalled){ Write-Error "Error: Missing Prerequisites. Look above for list." -ErrorAction Stop }
}

# All Install Options go here
Function Install-StudentEngagementTracker() {
    Write-HostStep "Installing the full Student Engagement Stack"
    Install-Prerequisites
}