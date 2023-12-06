using Microservices.Shared.Utilities;
using PublicApi.Api.HttpHandlers;
using PublicApi.Application;
using PublicApi.Infrastructure;

namespace PublicApi.Api;

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
            .AddTransient<JobHandler>()
            .AddTransient<TokenHandler>();

        // Application
        builder.Services
            .RegisterApplication();

        // Infrastructure
        builder.Services
            .RegisterInfrastructure();
    }
}
