using Geocoding.Application.Commands.GeocodeAddresses;
using Geocoding.Application.Queries.GetAddressCoordinates;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Geocoding.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    static ApplicationMetrics() => Meter = new("Geocoding.Application");

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
            .AddTransient<IGeocodeAddressesCommandHandlerMetrics, GeocodeAddressesCommandHandlerMetrics>()
            .AddTransient<IGetAddressCoordinatesQueryHandlerMetrics, GetAddressCoordinatesQueryHandlerMetrics>();
    }
}
