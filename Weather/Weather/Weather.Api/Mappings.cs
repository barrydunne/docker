using Mapster;
using Microservices.Shared.Events;
using Weather.Application.Commands.GenerateWeather;

namespace Weather.Api;

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
        TypeAdapterConfig<LocationsReadyEvent, GenerateWeatherCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Coordinates, src => src.DestinationCoordinates);
    }
}
