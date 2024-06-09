Set-Location $PSScriptRoot

# Run unit tests (integration tests may be run manually)
function Invoke-Tests {
    param(
        [Parameter(Mandatory)]$csproj,
        [Parameter(Mandatory)]$assembly
    )
    dotnet build $csproj -c Release --framework net8.0
    Remove-Item -Path './Packages/*' -Force
    dotnet test $csproj --no-build -c Release --framework net8.0 -l "console;verbosity=normal"  --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[$assembly]* /p:Threshold=100
    if ($LastExitCode -ne 0) {
        Write-Host "TESTS FAILED $csproj" -ForegroundColor Red
        exit 1
    }
}

Invoke-Tests -csproj 'Microservices.Shared.CloudEmail.Aws.UnitTests/Microservices.Shared.CloudEmail.Aws.UnitTests.csproj' -assembly 'Microservices.Shared.CloudEmail.Aws'
Invoke-Tests -csproj 'Microservices.Shared.CloudEmail.Smtp.UnitTests/Microservices.Shared.CloudEmail.Smtp.UnitTests.csproj' -assembly 'Microservices.Shared.CloudEmail.Smtp'
Invoke-Tests -csproj 'Microservices.Shared.CloudFiles.Aws.UnitTests/Microservices.Shared.CloudFiles.Aws.UnitTests.csproj' -assembly 'Microservices.Shared.CloudFiles.Aws'
Invoke-Tests -csproj 'Microservices.Shared.CloudFiles.Ftp.UnitTests/Microservices.Shared.CloudFiles.Ftp.UnitTests.csproj' -assembly 'Microservices.Shared.CloudFiles.Ftp'
Invoke-Tests -csproj 'Microservices.Shared.CloudSecrets.Aws.UnitTests/Microservices.Shared.CloudSecrets.Aws.UnitTests.csproj' -assembly 'Microservices.Shared.CloudSecrets.Aws'
Invoke-Tests -csproj 'Microservices.Shared.CloudSecrets.SecretsManager.UnitTests/Microservices.Shared.CloudSecrets.SecretsManager.UnitTests.csproj' -assembly 'Microservices.Shared.CloudSecrets.SecretsManager'
Invoke-Tests -csproj 'Microservices.Shared.Queues.RabbitMQ.UnitTests/Microservices.Shared.Queues.RabbitMQ.UnitTests.csproj' -assembly 'Microservices.Shared.Queues.RabbitMQ'

# Clear local nuget cache to allow updates with the same version number
if ($IsLinux) {
    $profileDir = $env:HOME
}
else {
    $profileDir = $env:USERPROFILE
}
$nugetDirectory = Join-Path -Path $profileDir -ChildPath '.nuget'
$nugetDirectory = Join-Path -Path $nugetDirectory -ChildPath 'packages'
$prefixToDelete = 'microservices.shared.'
Write-Host "Checking package folder $nugetDirectory for old packages"
$packageFolders = Get-ChildItem -Path $nugetDirectory -Directory | Where-Object { $_.Name -like "$prefixToDelete*" }
foreach ($folder in $packageFolders) {
    Write-Host "Deleting old package $($folder.Name)"
    Remove-Item -Path $folder.FullName -Recurse -Force
}

# Build new packages
dotnet build -c Release .\Microservices.SharedLibraries.sln

# Add local source
try
{
    dotnet nuget remove source Microservices.SharedLibraries | Out-Null
} catch { }
$sourceDirectory = Join-Path -Path $PSScriptRoot -ChildPath 'Packages'
dotnet nuget add source "$sourceDirectory" -n Microservices.SharedLibraries
