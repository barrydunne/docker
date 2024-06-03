using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Weather.Application.Commands.GenerateWeather;
using Weather.Application.Queries.GetWeather;

namespace Weather.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    /// <summary>
    /// Register the required metrics providers.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterMetrics(this IServiceCollection services)
    {
        return services
            .AddSingleton<IGenerateWeatherCommandHandlerMetrics, GenerateWeatherCommandHandlerMetrics>()
            .AddSingleton<IGetWeatherQueryHandlerMetrics, GetWeatherQueryHandlerMetrics>();
    }
}
