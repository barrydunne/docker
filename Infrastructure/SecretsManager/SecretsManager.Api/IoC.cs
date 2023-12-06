using SecretsManager.Api.HttpHandlers;
using SecretsManager.Application;

namespace SecretsManager.Api;

/// <summary>
/// Register services for dependency injection.
/// </summary>
internal static class IoC
{
    /// <summary>
    /// Register services for dependency injection.
    /// </summary>
    /// <param name="builder">The builder to register services with.</param>
    /// <exception cref="KeyNotFoundException">If the Redis configuration is not found.</exception>
    internal static void RegisterServices(WebApplicationBuilder builder)
    {
        // API Handlers
        builder.Services
            .AddTransient<SecretsHandler>();

        // Application
        builder.Services
            .RegisterApplication(builder.Configuration);
    }
}
