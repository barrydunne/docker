# dotnet tool install --global dotnet-reportgenerator-globaltool

Set-Location $PSScriptRoot
dotnet test .\Imaging.Infrastructure.Tests.csproj -c Release --no-restore --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Imaging.Infrastructure]* /p:Exclude=[Imaging.Infrastructure]*.BingModels.* /p:Threshold=100
reportgenerator -reports:TestResults/coverage.opencover.xml -targetdir:TestResults/CoverageReport -reporttypes:Html_Dark
./TestResults/CoverageReport/index.html