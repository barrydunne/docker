using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Weather.Application.ExternalApi;
using Weather.Infrastructure.ExternalApi.Dummy;
using Weather.Infrastructure.Metrics;

namespace Weather.Infrastructure;

/// <summary>
/// Register services for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class IoC
{
    /// <summary>
    /// Register services for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services)
    {
        // External service
        services
            .AddTransient<IExternalApi, DummyApi>(); // Replace DummyApi with OpenMeteoApi for real weather

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }
}
