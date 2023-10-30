# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Add a local directory as a NuGet package source
# This requires the docker context to be at the directory containing both Microservices.SharedLibraries and Geocoding
RUN mkdir /packages
COPY Microservices.SharedLibraries/Packages /packages
RUN dotnet nuget add source /packages -n Microservices.SharedLibraries
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY ["Geocoding/Geocoding/Geocoding.Api/Geocoding.Api.csproj", "Geocoding.Api/"]
RUN dotnet restore "Geocoding.Api/Geocoding.Api.csproj"
COPY ["Geocoding/Geocoding/Geocoding.Logic/Geocoding.Logic.csproj", "Geocoding.Logic/"]
RUN dotnet restore "Geocoding.Logic/Geocoding.Logic.csproj"
COPY ["Geocoding/Geocoding/Geocoding.ExternalService/Geocoding.ExternalService.csproj", "Geocoding.ExternalService/"]
RUN dotnet restore "Geocoding.ExternalService/Geocoding.ExternalService.csproj"
COPY ["Geocoding/Geocoding/Geocoding.Logic.Tests/Geocoding.Logic.Tests.csproj", "Geocoding.Logic.Tests/"]
RUN dotnet restore "Geocoding.Logic.Tests/Geocoding.Logic.Tests.csproj"
COPY ["Geocoding/Geocoding/Geocoding.ExternalService.Tests/Geocoding.ExternalService.Tests.csproj", "Geocoding.ExternalService.Tests/"]
RUN dotnet restore "Geocoding.ExternalService.Tests/Geocoding.ExternalService.Tests.csproj"
# Build and test the projects
COPY Geocoding/Geocoding/ .
RUN dotnet build "Geocoding.Api/Geocoding.Api.csproj" -c Release -o /app/build
RUN dotnet test "Geocoding.Logic.Tests/Geocoding.Logic.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"Logic.TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=Logic.TestResults/coverage /p:Include=[Geocoding.Logic]* /p:Threshold=100
RUN dotnet test "Geocoding.ExternalService.Tests/Geocoding.ExternalService.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"ExternalService.TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=ExternalService.TestResults/coverage /p:Include=[Geocoding.ExternalService]* /p:Exclude=[Geocoding.ExternalService]*.MapQuestModels.* /p:Threshold=100

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/Geocoding.Api"
RUN dotnet publish "Geocoding.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN echo "$(date +'%Y-%m-%d %H:%M:%S')" > /app/publish/version.txt

# Final image from AspNet runtime base image - Skipped for Visual Studio debugging
FROM base AS final
# Install curl as root to provide health check to verify container is running OK
USER root
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -yq curl
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s \
  CMD curl -f http://localhost:8080/health/status || exit 1
# Switch to non-root user to run app
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Geocoding.Api.dll"]
