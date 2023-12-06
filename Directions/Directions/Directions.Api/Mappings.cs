using Directions.Application.Commands.GenerateDirections;
using Mapster;
using Microservices.Shared.Events;

namespace Directions.Api;

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
        TypeAdapterConfig<LocationsReadyEvent, GenerateDirectionsCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.StartingCoordinates, src => src.StartingCoordinates)
            .Map(dest => dest.DestinationCoordinates, src => src.DestinationCoordinates);
    }
}
