function New-TemporaryDirectory {
   $parent = [System.IO.Path]::GetTempPath()
   $name = [System.IO.Path]::GetRandomFileName()
   New-Item -ItemType Directory -Path (Join-Path $parent $name)
}

function TestDotNetIsInstalled(){
   try{
     $dotNet = dotnet --info
   }catch{
      return $FALSE
   }
   return ($null -ne ($dotNet | ? { $_ -match "AspNetCore.App 3."}) -and $null -ne ($dotNet | ? { $_ -match "NETCore.App 3."}))
}

function DownloadDotNetInstaller($tempDir){
   #
   # Download the Windows Hosting Bundle Installer for ASP.NET Core 3.1 Runtime (v3.1.0)
   #
   # The installer URL was obtained from:
   # https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.0-windows-hosting-bundle-installer
   #

   $whb_installer_url = "https://download.visualstudio.microsoft.com/download/pr/fa3f472e-f47f-4ef5-8242-d3438dd59b42/9b2d9d4eecb33fe98060fd2a2cb01dcd/dotnet-hosting-3.1.0-win.exe"
   $whb_installer_file = $tempDir + [System.IO.Path]::GetFileName( $whb_installer_url )
   if ( [System.IO.File]::Exists( $whb_installer_file ) ) {
      return $whb_installer_file 
   }

   Try {
      Invoke-WebRequest -Uri $whb_installer_url -OutFile $whb_installer_file
      return $whb_installer_file
   }
   Catch {
      Write-Output ( $_.Exception.ToString() )
      throw
   }
}

function ExecuteInstaller($installerFile){
   Start-Process -Wait -FilePath $installerFile 
}

function Install-DotNetCore31($config) {
   #
   # Reference: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1
   #
    Write-Host "Check if Dot Net Core is already installed"
   if (TestDotNetIsInstalled) {
      Write-Host "Dot Net Core is already installed"
      return
   }

   if($null -ne $config){
      $tempDir = $config.TempPathForBinaryDownloads
   }else{
      $tempDir = "C:\temp\edfi\StudentEngegement\"
   }
   if ( ![System.IO.Directory]::Exists( $tempDir ) ) {
      throw "Error creating temo directory"
      
   }

   Write-Host "* Downloading Dot Net Core 3.1"
   $installerFile = DownloadDotNetInstaller $tempDir
   Write-Host "* Installing Dot Net Core 3.1"
   $installerFile 
   ExecuteInstaller $installerFile
}