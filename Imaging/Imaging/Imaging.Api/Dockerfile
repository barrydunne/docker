# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Add a local directory as a NuGet package source
# This requires the docker context to be at the directory containing both Microservices.SharedLibraries and Imaging
RUN mkdir /packages
COPY Microservices.SharedLibraries/Packages /packages
RUN dotnet nuget add source /packages -n Microservices.SharedLibraries
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY ["Imaging/Imaging/Imaging.Api/Imaging.Api.csproj", "Imaging.Api/"]
RUN dotnet restore "Imaging.Api/Imaging.Api.csproj"
COPY ["Imaging/Imaging/Imaging.Application/Imaging.Application.csproj", "Imaging.Application/"]
RUN dotnet restore "Imaging.Application/Imaging.Application.csproj"
COPY ["Imaging/Imaging/Imaging.Infrastructure/Imaging.Infrastructure.csproj", "Imaging.Infrastructure/"]
RUN dotnet restore "Imaging.Infrastructure/Imaging.Infrastructure.csproj"
COPY ["Imaging/Imaging/Imaging.Application.Tests/Imaging.Application.Tests.csproj", "Imaging.Application.Tests/"]
RUN dotnet restore "Imaging.Application.Tests/Imaging.Application.Tests.csproj"
COPY ["Imaging/Imaging/Imaging.Infrastructure.Tests/Imaging.Infrastructure.Tests.csproj", "Imaging.Infrastructure.Tests/"]
RUN dotnet restore "Imaging.Infrastructure.Tests/Imaging.Infrastructure.Tests.csproj"
# Build and test the projects
COPY Imaging/Imaging/ .
RUN dotnet build "Imaging.Api/Imaging.Api.csproj" -c Release -o /app/build
RUN dotnet test "Imaging.Application.Tests/Imaging.Application.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Imaging.Application]* /p:Threshold=100
RUN dotnet test "Imaging.Infrastructure.Tests/Imaging.Infrastructure.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Imaging.Infrastructure]* /p:Exclude=[Imaging.Infrastructure]*.BingModels.* /p:Threshold=100

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/Imaging.Api"
RUN dotnet publish "Imaging.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
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
ENTRYPOINT ["dotnet", "Imaging.Api.dll"]
