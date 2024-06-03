using Directions.Application.Commands.GenerateDirections;
using Directions.Application.Queries.GetDirections;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Directions.Infrastructure.Metrics;

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
            .AddSingleton<IGenerateDirectionsCommandHandlerMetrics, GenerateDirectionsCommandHandlerMetrics>()
            .AddSingleton<IGetDirectionsQueryHandlerMetrics, GetDirectionsQueryHandlerMetrics>();
    }
}
