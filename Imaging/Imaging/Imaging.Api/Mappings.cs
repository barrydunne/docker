using Imaging.Application.Commands.SaveImage;
using Mapster;
using Microservices.Shared.Events;

namespace Imaging.Api;

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
        TypeAdapterConfig<LocationsReadyEvent, SaveImageCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Address, src => src.DestinationAddress)
            .Map(dest => dest.Coordinates, src => src.DestinationCoordinates);
    }
}
