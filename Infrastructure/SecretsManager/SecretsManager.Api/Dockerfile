# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Add a local directory as a NuGet package source
# This requires the docker context to be at the directory containing both Microservices.SharedLibraries and PublicApi
RUN mkdir /packages
COPY Microservices.SharedLibraries/Packages /packages
RUN dotnet nuget add source /packages -n Microservices.SharedLibraries
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY ["Infrastructure/SecretsManager/SecretsManager.Api/SecretsManager.Api.csproj", "SecretsManager.Api/"]
RUN dotnet restore "SecretsManager.Api/SecretsManager.Api.csproj"
COPY ["Infrastructure/SecretsManager/SecretsManager.Application/SecretsManager.Application.csproj", "SecretsManager.Application/"]
RUN dotnet restore "SecretsManager.Application/SecretsManager.Application.csproj"
COPY ["Infrastructure/SecretsManager/SecretsManager.Application.Tests/SecretsManager.Application.Tests.csproj", "SecretsManager.Application.Tests/"]
RUN dotnet restore "SecretsManager.Application.Tests/SecretsManager.Application.Tests.csproj"
# Build and test the projects
COPY Infrastructure/SecretsManager/ .
RUN dotnet build "SecretsManager.Api/SecretsManager.Api.csproj" -c Release -o /app/build
RUN dotnet test "SecretsManager.Application.Tests/SecretsManager.Application.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[SecretsManager.Application]* /p:Threshold=100

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/SecretsManager.Api"
RUN dotnet publish "SecretsManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
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
ENTRYPOINT ["dotnet", "SecretsManager.Api.dll"]
