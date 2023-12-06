using Geocoding.Api.HttpHandlers;
using Geocoding.Application;
using Geocoding.Infrastructure;
using Microservices.Shared.Utilities;

namespace Geocoding.Api;

/// <summary>
/// Register services for dependency injection.
/// </summary>
internal static class IoC
{
    /// <summary>
    /// Register services for dependency injection.
    /// </summary>
    /// <param name="builder">The builder to register services with.</param>
    internal static void RegisterServices(WebApplicationBuilder builder)
    {
        // Default microservice dependencies
        builder.AddMicroserviceDependencies();

        // API Handlers
        builder.Services
            .AddTransient<GeocodingHandler>();

        // Application
        builder.Services
            .RegisterApplication();

        // Infrastructure
        builder.Services
            .RegisterInfrastructure(builder.Configuration);
    }
}
