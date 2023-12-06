using Directions.Application.ExternalApi;
using Directions.Infrastructure.ExternalApi.Dummy;
using Directions.Infrastructure.Metrics;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Directions.Infrastructure;

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
            .AddTransient<IExternalApi, DummyApi>(); // Replace DummyApi with MapQuestApi for real directions

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }
}
