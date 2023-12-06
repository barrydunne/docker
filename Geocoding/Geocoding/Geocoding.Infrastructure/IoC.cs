using Geocoding.Application.Caching;
using Geocoding.Application.ExternalApi;
using Geocoding.Infrastructure.Caching;
using Geocoding.Infrastructure.ExternalApi.Dummy;
using Geocoding.Infrastructure.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Diagnostics.CodeAnalysis;

namespace Geocoding.Infrastructure;

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
    /// <param name="configuration">The application configuration manager.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
    {
        // External service
        services
            .AddTransient<IExternalApi, DummyApi>(); // Replace DummyApi with MapQuestApi for real geocoding

        // Caching
        var conf = configuration.GetSection("Redis").Get<RedisConfiguration>() ?? throw new KeyNotFoundException("Redis configuration not found");
        services
            .AddSingleton<IGeocodingCache, GeocodingCache>()
            .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(conf);

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }
}
