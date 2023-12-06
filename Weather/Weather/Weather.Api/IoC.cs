using Microservices.Shared.Utilities;
using Weather.Api.HttpHandlers;
using Weather.Application;
using Weather.Infrastructure;

namespace Weather.Api;

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
            .AddTransient<WeatherHandler>();

        // Application
        builder.Services
            .RegisterApplication();

        // Infrastructure
        builder.Services
            .RegisterInfrastructure();
    }
}
