# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Add a local directory as a NuGet package source
# This requires the docker context to be at the directory containing both Microservices.SharedLibraries and Directions
RUN mkdir /packages
COPY Microservices.SharedLibraries/Packages /packages
RUN dotnet nuget add source /packages -n Microservices.SharedLibraries
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY ["Directions/Directions/Directions.Api/Directions.Api.csproj", "Directions.Api/"]
RUN dotnet restore "Directions.Api/Directions.Api.csproj"
COPY ["Directions/Directions/Directions.Application/Directions.Application.csproj", "Directions.Application/"]
RUN dotnet restore "Directions.Application/Directions.Application.csproj"
COPY ["Directions/Directions/Directions.Infrastructure/Directions.Infrastructure.csproj", "Directions.Infrastructure/"]
RUN dotnet restore "Directions.Infrastructure/Directions.Infrastructure.csproj"
COPY ["Directions/Directions/Directions.Application.Tests/Directions.Application.Tests.csproj", "Directions.Application.Tests/"]
RUN dotnet restore "Directions.Application.Tests/Directions.Application.Tests.csproj"
COPY ["Directions/Directions/Directions.Infrastructure.Tests/Directions.Infrastructure.Tests.csproj", "Directions.Infrastructure.Tests/"]
RUN dotnet restore "Directions.Infrastructure.Tests/Directions.Infrastructure.Tests.csproj"
# Build and test the projects
COPY Directions/Directions/ .
RUN dotnet build "Directions.Api/Directions.Api.csproj" -c Release -o /app/build
RUN dotnet test "Directions.Application.Tests/Directions.Application.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Directions.Application]* /p:Threshold=100
RUN dotnet test "Directions.Infrastructure.Tests/Directions.Infrastructure.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Directions.Infrastructure]* /p:Exclude=[Directions.Infrastructure]*.MapQuestModels.* /p:Threshold=100

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/Directions.Api"
RUN dotnet publish "Directions.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
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
ENTRYPOINT ["dotnet", "Directions.Api.dll"]
