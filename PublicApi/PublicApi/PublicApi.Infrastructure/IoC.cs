using Microsoft.Extensions.DependencyInjection;
using PublicApi.Application.Caching;
using PublicApi.Application.Repositories;
using PublicApi.Infrastructure.Caching;
using PublicApi.Infrastructure.Metrics;
using PublicApi.Infrastructure.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace PublicApi.Infrastructure;

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
            .AddSingleton<IJobRepository, JobRepository>()
            .AddSingleton<IJobCache, JobCache>();

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }
}
