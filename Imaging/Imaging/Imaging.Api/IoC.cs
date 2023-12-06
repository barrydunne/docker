using Imaging.Api.HttpHandlers;
using Imaging.Application;
using Imaging.Infrastructure;
using Microservices.Shared.Utilities;

namespace Imaging.Api;

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
            .AddTransient<ImagingHandler>();

        // Application
        builder.Services
            .RegisterApplication();

        // Infrastructure
        builder.Services
            .RegisterInfrastructure(builder.Configuration);
    }
}
