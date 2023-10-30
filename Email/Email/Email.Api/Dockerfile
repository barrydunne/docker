# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Add a local directory as a NuGet package source
# This requires the docker context to be at the directory containing both Microservices.SharedLibraries and Email
RUN mkdir /packages
COPY Microservices.SharedLibraries/Packages /packages
RUN dotnet nuget add source /packages -n Microservices.SharedLibraries
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY ["Email/Email/Email.Api/Email.Api.csproj", "Email.Api/"]
RUN dotnet restore "Email.Api/Email.Api.csproj"
COPY ["Email/Email/Email.Logic/Email.Logic.csproj", "Email.Logic/"]
RUN dotnet restore "Email.Logic/Email.Logic.csproj"
COPY ["Email/Email/Email.Logic.Tests/Email.Logic.Tests.csproj", "Email.Logic.Tests/"]
RUN dotnet restore "Email.Logic.Tests/Email.Logic.Tests.csproj"
COPY ["Email/Email/Email.Repository.UnitTests/Email.Repository.UnitTests.csproj", "Email.Repository.UnitTests/"]
RUN dotnet restore "Email.Repository.UnitTests/Email.Repository.UnitTests.csproj"
# Build and test the projects
COPY Email/Email/ .
RUN dotnet build "Email.Api/Email.Api.csproj" -c Release -o /app/build
RUN dotnet test "Email.Logic.Tests/Email.Logic.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Email.Logic]* /p:Threshold=100
RUN dotnet test "Email.Repository.UnitTests/Email.Repository.UnitTests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Email.Repository]* /p:Threshold=100

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/Email.Api"
RUN dotnet publish "Email.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
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
ENTRYPOINT ["dotnet", "Email.Api.dll"]
