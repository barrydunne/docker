param([switch]$skipLibraries)

$cwd = Get-Location

# Build shared packages
if (!$skipLibraries) {
    Set-Location $PSScriptRoot
    ../Microservices.SharedLibraries/build.ps1
}

# Build and run containers
Set-Location $PSScriptRoot
./build-infrastructure.ps1
./build-publicapi.ps1
./build-state.ps1
./build-geocoding.ps1
./build-directions.ps1
./build-weather.ps1
./build-imaging.ps1
./build-email.ps1

# Apply any post build script, eg add integration test api keys to secrets
if (Test-Path ./post-build.ps1 -PathType Leaf) {
    ./post-build.ps1
}

# Perform an end-to-end test that everything is working
./sanity-check.ps1
Write-Host Waiting 60 seconds before running end-to-end test
for ($i = 1; $i -le 30; $i++) {
    Start-Sleep -Seconds 2
    Write-Host . -NoNewline
}
Write-Host
./end-to-end-test.ps1

Set-Location $cwd