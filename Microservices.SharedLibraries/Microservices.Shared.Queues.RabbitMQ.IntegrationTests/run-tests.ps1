# dotnet tool install --global dotnet-reportgenerator-globaltool

Set-Location $PSScriptRoot
dotnet test .\Microservices.Shared.Queues.RabbitMQ.IntegrationTests.csproj -c Release --no-restore --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Microservices.Shared.Queues.RabbitMQ]* /p:ThresholdType=method /p:Threshold=100 /p:ThresholdType=line /p:Threshold=95 /p:ThresholdType=branch /p:Threshold=80
reportgenerator -reports:TestResults/coverage.opencover.xml -targetdir:TestResults/CoverageReport -reporttypes:Html_Dark
./TestResults/CoverageReport/index.html