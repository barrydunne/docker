using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
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
    static ApplicationMetrics() => Meter = new("Weather.Application");

    /// <summary>
    /// Gets the shared metrics Meter.
    /// </summary>
    internal static Meter Meter { get; }

    /// <summary>
    /// Register the required metrics providers.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterMetrics(this IServiceCollection services)
    {
        return services
            .AddTransient<IGenerateWeatherCommandHandlerMetrics, GenerateWeatherCommandHandlerMetrics>()
            .AddTransient<IGetWeatherQueryHandlerMetrics, GetWeatherQueryHandlerMetrics>();
    }
}
