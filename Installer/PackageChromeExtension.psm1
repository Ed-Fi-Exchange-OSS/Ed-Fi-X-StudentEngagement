Import-Module .\Prettify.psm1

function Write-ChromePackage($path, $pemFilePath) {    
    $chrome = Get-ChromeFileName
    if ($null -eq $pemFilePath ) {
        Write-HostStep "Packaging chrome extension and pem file"
        Start-Process -FilePath $chrome --pack-extension=$path
    }
    else {
        Write-HostStep "Packaging chrome extension using an existing pem file"
        Start-Process -FilePath $chrome --pack-extension=$path --pack-extension-key=$pemFilePath
    }
}

function Get-ChromeFileName() {
    (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe').'(Default)'
}
