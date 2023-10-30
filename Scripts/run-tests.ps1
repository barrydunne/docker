$cwd = Get-Location
Set-Location $PSScriptRoot/..

# Use a local running copy of https://httpstat.us to remove the dependency on external site availability
# This is used by Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests
Write-Host Running local copy of httpstat.us on port 8888
try {
    docker rm -f http-status | Out-Null
} catch { }
docker run -p 8888:80 -d --name http-status ghcr.io/aaronpowell/httpstatus:c82331cbde67f430da66a84a758d94ba5afd7620
$timeout = (Get-Date).AddSeconds(60)
:whileWaiting while ((Get-Date) -lt $timeout) {
    try {
        Start-Sleep -Seconds 2
        Invoke-RestMethod -Uri 'http://localhost:8888/200' -Method Get
        Write-Host Local copy of httpstat.us is running
        break whileWaiting
    }
    catch {
        Write-Host Local copy of httpstat.us is not yet running
    }
}

function Invoke-Tests {
    param(
        [Parameter(Mandatory)]$csproj,
        [Parameter(Mandatory)]$includeAssembly,
        [Parameter(Mandatory)]$coverageName,
        [string]$exclude = 'NONE'
    )
    Write-Host "Running tests from $csproj"
    dotnet test $csproj -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/TestResults/$coverageName.xml /p:Include=[$includeAssembly]* /p:Exclude=[*]$exclude
    if ($LastExitCode -ne 0) {
        Write-Host "TESTS FAILED $csproj" -ForegroundColor Red
        exit 1
    }
}

Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudEmail.Smtp.UnitTests/Microservices.Shared.CloudEmail.Smtp.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudEmail.Smtp' -coverageName 'ce.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudEmail.Smtp.IntegrationTests/Microservices.Shared.CloudEmail.Smtp.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudEmail.Smtp' -coverageName 'ce.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudFiles.Ftp.UnitTests/Microservices.Shared.CloudFiles.Ftp.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudFiles.Ftp' -coverageName 'cf.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudFiles.Ftp.IntegrationTests/Microservices.Shared.CloudFiles.Ftp.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudFiles.Ftp' -coverageName 'cf.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudSecrets.SecretsManager.UnitTests/Microservices.Shared.CloudSecrets.SecretsManager.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudSecrets.SecretsManager' -coverageName 'cs.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests/Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudSecrets.SecretsManager' -coverageName 'cs.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.Queues.RabbitMQ.UnitTests/Microservices.Shared.Queues.RabbitMQ.UnitTests.csproj' -includeAssembly 'Microservices.Shared.Queues.RabbitMQ' -coverageName 'mq.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.Queues.RabbitMQ.IntegrationTests/Microservices.Shared.Queues.RabbitMQ.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.Queues.RabbitMQ' -coverageName 'mq.i'
Invoke-Tests -csproj 'Infrastructure/SecretsManager/SecretsManager.Logic.Tests/SecretsManager.Logic.Tests.csproj' -includeAssembly 'SecretsManager.Logic' -coverageName 'sm.l'
Invoke-Tests -csproj 'PublicApi/PublicApi/PublicApi.Logic.Tests/PublicApi.Logic.Tests.csproj' -includeAssembly 'PublicApi.Logic' -coverageName 'p.l'
Invoke-Tests -csproj 'PublicApi/PublicApi/PublicApi.Repository.IntegrationTests/PublicApi.Repository.IntegrationTests.csproj' -includeAssembly 'PublicApi.Repository' -coverageName 'p.r'
Invoke-Tests -csproj 'State/State/State.Logic.Tests/State.Logic.Tests.csproj' -includeAssembly 'State.Logic' -coverageName 's.l'
Invoke-Tests -csproj 'State/State/State.Repository.IntegrationTests/State.Repository.IntegrationTests.csproj' -includeAssembly 'State.Repository' -coverageName 's.r'
Invoke-Tests -csproj 'Geocoding/Geocoding/Geocoding.Logic.Tests/Geocoding.Logic.Tests.csproj' -includeAssembly 'Geocoding.Logic' -coverageName 'g.l'
Invoke-Tests -csproj 'Geocoding/Geocoding/Geocoding.ExternalService.Tests/Geocoding.ExternalService.Tests.csproj' -includeAssembly 'Geocoding.ExternalService' -exclude '*.MapQuestModels.*' -coverageName 'g.e'
Invoke-Tests -csproj 'Directions/Directions/Directions.Logic.Tests/Directions.Logic.Tests.csproj' -includeAssembly 'Directions.Logic' -coverageName 'd.l'
Invoke-Tests -csproj 'Directions/Directions/Directions.ExternalService.Tests/Directions.ExternalService.Tests.csproj' -includeAssembly 'Directions.ExternalService' -exclude '*.MapQuestModels.*' -coverageName 'd.e'
Invoke-Tests -csproj 'Weather/Weather/Weather.Logic.Tests/Weather.Logic.Tests.csproj' -includeAssembly 'Weather.Logic' -coverageName 'w.l'
Invoke-Tests -csproj 'Weather/Weather/Weather.ExternalService.Tests/Weather.ExternalService.Tests.csproj' -includeAssembly 'Weather.ExternalService' -exclude '*.OpenMeteoModels.*' -coverageName 'w.e'
Invoke-Tests -csproj 'Imaging/Imaging/Imaging.Logic.Tests/Imaging.Logic.Tests.csproj' -includeAssembly 'Imaging.Logic' -coverageName 'i.l'
Invoke-Tests -csproj 'Imaging/Imaging/Imaging.ExternalService.Tests/Imaging.ExternalService.Tests.csproj' -includeAssembly 'Imaging.ExternalService' -exclude '*.BingModels.*' -coverageName 'i.e'
Invoke-Tests -csproj 'Email/Email/Email.Logic.Tests/Email.Logic.Tests.csproj' -includeAssembly 'Email.Logic' -coverageName 'e.l'
Invoke-Tests -csproj 'Email/Email/Email.Repository.IntegrationTests/Email.Repository.IntegrationTests.csproj' -includeAssembly 'Email.Repository' -coverageName 'e.ri'
Invoke-Tests -csproj 'Email/Email/Email.Repository.UnitTests/Email.Repository.UnitTests.csproj' -includeAssembly 'Email.Repository' -coverageName 'e.ru'

Write-Host Removing local copy of httpstat.us
docker rm -f http-status

Write-Host Generating Reports
Set-Location $PSScriptRoot
# Check dotnet-reportgenerator-globaltool is installed
dotnet tool list -g dotnet-reportgenerator-globaltool | Out-Null
if ($LastExitCode -ne 0) {
    dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.26
}
reportgenerator -reports:'TestResults/ce.u.xml;TestResults/ce.i.xml;TestResults/cf.u.xml;TestResults/cf.i.xml;TestResults/cs.u.xml;TestResults/cs.i.xml;TestResults/mq.u.xml;TestResults/mq.i.xml;TestResults/sm.l.xml;TestResults/p.l.xml;TestResults/p.r.xml;TestResults/s.l.xml;TestResults/s.r.xml;TestResults/g.l.xml;TestResults/g.e.xml;TestResults/d.l.xml;TestResults/d.e.xml;TestResults/w.l.xml;TestResults/w.e.xml;TestResults/i.l.xml;TestResults/e.l.xml;TestResults/e.ri.xml;TestResults/e.ru.xml' -targetdir:TestResults/CoverageReport -reporttypes:"Html_Dark;TextSummary"
if ($IsLinux) {
    $reportPath = Join-Path -Path $PSScriptRoot -ChildPath './TestResults/CoverageReport/index.html'
    Write-Host "HTML Coverage report: $reportPath"
    $report = Get-Content 'TestResults/CoverageReport/Summary.txt' -Raw
    Write-Host $report
}
else {
    ./TestResults/CoverageReport/index.html
}
Set-Location $cwd
