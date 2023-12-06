using Directions.Application.Commands.GenerateDirections;
using Directions.Application.Queries.GetDirections;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Directions.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    static ApplicationMetrics() => Meter = new("Directions.Application");

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
            .AddTransient<IGenerateDirectionsCommandHandlerMetrics, GenerateDirectionsCommandHandlerMetrics>()
            .AddTransient<IGetDirectionsQueryHandlerMetrics, GetDirectionsQueryHandlerMetrics>();
    }
}
