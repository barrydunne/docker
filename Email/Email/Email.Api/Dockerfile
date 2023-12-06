# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

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
COPY ["Email/Email/Email.Application/Email.Application.csproj", "Email.Application/"]
RUN dotnet restore "Email.Application/Email.Application.csproj"
COPY ["Email/Email/Email.Application.Tests/Email.Application.Tests.csproj", "Email.Application.Tests/"]
RUN dotnet restore "Email.Application.Tests/Email.Application.Tests.csproj"
COPY ["Email/Email/Email.Infrastructure.UnitTests/Email.Infrastructure.UnitTests.csproj", "Email.Infrastructure.UnitTests/"]
RUN dotnet restore "Email.Infrastructure.UnitTests/Email.Infrastructure.UnitTests.csproj"
# Build and test the projects
COPY Email/Email/ .
RUN dotnet build "Email.Api/Email.Api.csproj" -c Release -o /app/build
RUN dotnet test "Email.Application.Tests/Email.Application.Tests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Email.Application]* /p:Threshold=100
RUN dotnet test "Email.Infrastructure.UnitTests/Email.Infrastructure.UnitTests.csproj" -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage /p:Include=[Email.Infrastructure]* /p:Threshold=100

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
