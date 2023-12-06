using Geocoding.Application.Commands.GeocodeAddresses;
using Mapster;
using Microservices.Shared.Events;

namespace Geocoding.Api;

/// <summary>
/// Provides the auto-mapping configuration for type conversions.
/// </summary>
internal static class Mappings
{
    /// <summary>
    /// Configure the auto-mapping configuration for type conversions.
    /// </summary>
    internal static void Map()
    {
        TypeAdapterConfig<JobCreatedEvent, GeocodeAddressesCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.StartingAddress, src => src.StartingAddress)
            .Map(dest => dest.DestinationAddress, src => src.DestinationAddress);
    }
}
