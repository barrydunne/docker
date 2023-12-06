using Microsoft.Extensions.DependencyInjection;
using State.Application.Repositories;
using State.Infrastructure.Metrics;
using State.Infrastructure.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace State.Infrastructure;

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
        // Repositories
        services
            .AddSingleton<IJobRepository, JobRepository>();

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }
}
