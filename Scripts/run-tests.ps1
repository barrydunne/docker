$cwd = Get-Location
Set-Location $PSScriptRoot/..

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

Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudEmail.Aws.UnitTests/Microservices.Shared.CloudEmail.Aws.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudEmail.Aws' -coverageName 'cea.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudEmail.Aws.IntegrationTests/Microservices.Shared.CloudEmail.Aws.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudEmail.Aws' -coverageName 'cea.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudEmail.Smtp.UnitTests/Microservices.Shared.CloudEmail.Smtp.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudEmail.Smtp' -coverageName 'ce.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudEmail.Smtp.IntegrationTests/Microservices.Shared.CloudEmail.Smtp.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudEmail.Smtp' -coverageName 'ce.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudFiles.Aws.UnitTests/Microservices.Shared.CloudFiles.Aws.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudFiles.Aws' -coverageName 'cfa.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudFiles.Aws.IntegrationTests/Microservices.Shared.CloudFiles.Aws.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudFiles.Aws' -coverageName 'cfa.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudFiles.Ftp.UnitTests/Microservices.Shared.CloudFiles.Ftp.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudFiles.Ftp' -coverageName 'cf.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudFiles.Ftp.IntegrationTests/Microservices.Shared.CloudFiles.Ftp.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudFiles.Ftp' -coverageName 'cf.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudSecrets.Aws.UnitTests/Microservices.Shared.CloudSecrets.Aws.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudSecrets.Aws' -coverageName 'csa.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudSecrets.Aws.IntegrationTests/Microservices.Shared.CloudSecrets.Aws.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudSecrets.Aws' -coverageName 'csa.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudSecrets.SecretsManager.UnitTests/Microservices.Shared.CloudSecrets.SecretsManager.UnitTests.csproj' -includeAssembly 'Microservices.Shared.CloudSecrets.SecretsManager' -coverageName 'cs.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests/Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.CloudSecrets.SecretsManager' -coverageName 'cs.i'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.Queues.RabbitMQ.UnitTests/Microservices.Shared.Queues.RabbitMQ.UnitTests.csproj' -includeAssembly 'Microservices.Shared.Queues.RabbitMQ' -coverageName 'mq.u'
Invoke-Tests -csproj 'Microservices.SharedLibraries/Microservices.Shared.Queues.RabbitMQ.IntegrationTests/Microservices.Shared.Queues.RabbitMQ.IntegrationTests.csproj' -includeAssembly 'Microservices.Shared.Queues.RabbitMQ' -coverageName 'mq.i'
Invoke-Tests -csproj 'Infrastructure/SecretsManager/SecretsManager.Application.Tests/SecretsManager.Application.Tests.csproj' -includeAssembly 'SecretsManager.Application' -coverageName 'sm.a'
Invoke-Tests -csproj 'PublicApi/PublicApi/PublicApi.Application.Tests/PublicApi.Application.Tests.csproj' -includeAssembly 'PublicApi.Application' -coverageName 'p.a'
Invoke-Tests -csproj 'PublicApi/PublicApi/PublicApi.Infrastructure.IntegrationTests/PublicApi.Infrastructure.IntegrationTests.csproj' -includeAssembly 'PublicApi.Infrastructure' -coverageName 'p.r'
Invoke-Tests -csproj 'State/State/State.Application.Tests/State.Application.Tests.csproj' -includeAssembly 'State.Application' -coverageName 's.a'
Invoke-Tests -csproj 'State/State/State.Infrastructure.IntegrationTests/State.Infrastructure.IntegrationTests.csproj' -includeAssembly 'State.Infrastructure' -coverageName 's.i'
Invoke-Tests -csproj 'Geocoding/Geocoding/Geocoding.Application.Tests/Geocoding.Application.Tests.csproj' -includeAssembly 'Geocoding.Application' -coverageName 'g.a'
Invoke-Tests -csproj 'Geocoding/Geocoding/Geocoding.Infrastructure.Tests/Geocoding.Infrastructure.Tests.csproj' -includeAssembly 'Geocoding.Infrastructure' -exclude '*.MapQuestModels.*' -coverageName 'g.e'
Invoke-Tests -csproj 'Directions/Directions/Directions.Application.Tests/Directions.Application.Tests.csproj' -includeAssembly 'Directions.Application' -coverageName 'd.a'
Invoke-Tests -csproj 'Directions/Directions/Directions.Infrastructure.Tests/Directions.Infrastructure.Tests.csproj' -includeAssembly 'Directions.Infrastructure' -exclude '*.MapQuestModels.*' -coverageName 'd.e'
Invoke-Tests -csproj 'Weather/Weather/Weather.Application.Tests/Weather.Application.Tests.csproj' -includeAssembly 'Weather.Application' -coverageName 'w.a'
Invoke-Tests -csproj 'Weather/Weather/Weather.Infrastructure.Tests/Weather.Infrastructure.Tests.csproj' -includeAssembly 'Weather.Infrastructure' -exclude '*.OpenMeteoModels.*' -coverageName 'w.e'
Invoke-Tests -csproj 'Imaging/Imaging/Imaging.Application.Tests/Imaging.Application.Tests.csproj' -includeAssembly 'Imaging.Application' -coverageName 'i.a'
Invoke-Tests -csproj 'Imaging/Imaging/Imaging.Infrastructure.Tests/Imaging.Infrastructure.Tests.csproj' -includeAssembly 'Imaging.Infrastructure' -exclude '*.BingModels.*' -coverageName 'i.e'
Invoke-Tests -csproj 'Email/Email/Email.Application.Tests/Email.Application.Tests.csproj' -includeAssembly 'Email.Application' -coverageName 'e.a'
Invoke-Tests -csproj 'Email/Email/Email.Infrastructure.IntegrationTests/Email.Infrastructure.IntegrationTests.csproj' -includeAssembly 'Email.Infrastructure' -coverageName 'e.ri'
Invoke-Tests -csproj 'Email/Email/Email.Infrastructure.UnitTests/Email.Infrastructure.UnitTests.csproj' -includeAssembly 'Email.Infrastructure' -coverageName 'e.ru'

Write-Host Generating Reports
Set-Location $PSScriptRoot
# Check dotnet-reportgenerator-globaltool is installed
dotnet tool list -g dotnet-reportgenerator-globaltool | Out-Null
if ($LastExitCode -ne 0) {
    dotnet tool install --global dotnet-reportgenerator-globaltool
}
reportgenerator -reports:'TestResults/cea.u.xml;TestResults/cea.i.xml;TestResults/ce.u.xml;TestResults/ce.i.xml;TestResults/cfa.u.xml;TestResults/cfa.i.xml;TestResults/cf.u.xml;TestResults/cf.i.xml;TestResults/csa.u.xml;TestResults/csa.i.xml;TestResults/cs.u.xml;TestResults/cs.i.xml;TestResults/mq.u.xml;TestResults/mq.i.xml;TestResults/sm.a.xml;TestResults/p.a.xml;TestResults/p.r.xml;TestResults/s.a.xml;TestResults/s.i.xml;TestResults/g.a.xml;TestResults/g.e.xml;TestResults/d.a.xml;TestResults/d.e.xml;TestResults/w.a.xml;TestResults/w.e.xml;TestResults/i.a.xml;TestResults/e.a.xml;TestResults/e.ri.xml;TestResults/e.ru.xml' -targetdir:TestResults/CoverageReport -reporttypes:"Html_Dark;TextSummary"
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
