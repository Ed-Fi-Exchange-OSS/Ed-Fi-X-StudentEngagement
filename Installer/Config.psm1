Function Get-ConfigurationParameters() {
    Get-Content -Raw -Path config.json | ConvertFrom-Json
}