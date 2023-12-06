# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

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
COPY ["Geocoding/Geocoding/Geocoding.Application/Geocoding.Application.csproj", "Geocoding.Application/"]
RUN dotnet restore "Geocoding.Application/Geocoding.Application.csproj"
COPY ["Geocoding/Geocoding/Geocoding.Infrastructure/Geocoding.Infrastructure.csproj", "Geocoding.Infrastructure/"]
RUN dotnet restore "Geocoding.Infrastructure/Geocoding.Infrastructure.csproj"
COPY ["Geocoding/Geocoding/Geocoding.Application.Tests/Geocoding.Application.Tests.csproj", "Geocoding.Application.Tests/"]
RUN dotnet restore "Geocoding.Application.Tests/Geocoding.Application.Tests.csproj"
COPY ["Geocoding/Geocoding/Geocoding.Infrastructure.Tests/Geocoding.Infrastructure.Tests.csproj", "Geocoding.Infrastructure.Tests/"]
RUN dotnet restore "Geocoding.Infrastructure.Tests/Geocoding.Infrastructure.Tests.csproj"
# Build and test the projects
COPY Geocoding/Geocoding/ .
RUN dotnet build "Geocoding.Api/Geocoding.Api.csproj" -c Release -o /app/build
RUN dotnet test "Geocoding.Application.Tests/Geocoding.Application.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"Logic.TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=Logic.TestResults/coverage /p:Include=[Geocoding.Application]* /p:Threshold=100
RUN dotnet test "Geocoding.Infrastructure.Tests/Geocoding.Infrastructure.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"ExternalService.TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=ExternalService.TestResults/coverage /p:Include=[Geocoding.Infrastructure]* /p:Exclude=[Geocoding.Infrastructure]*.MapQuestModels.* /p:Threshold=100

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
