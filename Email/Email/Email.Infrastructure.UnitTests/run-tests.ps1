# dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.26

Set-Location $PSScriptRoot
dotnet test .\Email.Infrastructure.UnitTests.csproj -c Release --no-restore --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Email.Infrastructure]* /p:Threshold=100
reportgenerator -reports:TestResults/coverage.opencover.xml -targetdir:TestResults/CoverageReport -reporttypes:Html_Dark
./TestResults/CoverageReport/index.html