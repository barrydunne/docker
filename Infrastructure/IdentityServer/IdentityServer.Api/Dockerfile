# The ASP.Net 8 runtime is the base image required for both building a final image and Visual Studio debugging
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

# Build from SDK image - This is the image that gets used when debugging from Visual Studio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Restore the projects, this is done to enable docker caching of the image with the packages
WORKDIR /src
COPY ["IdentityServer.Api/IdentityServer.Api.csproj", "IdentityServer.Api/"]
RUN dotnet restore "IdentityServer.Api/IdentityServer.Api.csproj"
# Build and test the projects
COPY . .
RUN dotnet build "IdentityServer.Api/IdentityServer.Api.csproj" -c Release -o /app/build

# Publish the application - Skipped for Visual Studio debugging
FROM build AS publish
WORKDIR "/src/IdentityServer.Api"
RUN dotnet publish "IdentityServer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN echo "$(date +'%Y-%m-%d %H:%M:%S')" > /app/publish/version.txt

# Final image from AspNet runtime base image - Skipped for Visual Studio debugging
FROM base AS final
# Install curl as root to provide health check to verify container is running OK
USER root
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -yq curl
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s \
  CMD curl -f http://localhost:8080/.well-known/openid-configuration || exit 1

# Switch to non-root user to run app
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.Api.dll"]
