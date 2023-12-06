using Mapster;
using Microservices.Shared.Events;
using State.Application.Commands.CreateJob;
using State.Application.Commands.UpdateDirectionsResult;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Commands.UpdateImagingResult;
using State.Application.Commands.UpdateWeatherResult;

namespace State.Api;

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
        TypeAdapterConfig<JobCreatedEvent, CreateJobCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.StartingAddress, src => src.StartingAddress)
            .Map(dest => dest.DestinationAddress, src => src.DestinationAddress)
            .Map(dest => dest.Email, src => src.Email);

        TypeAdapterConfig<GeocodingCompleteEvent, UpdateGeocodingResultCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.StartingCoordinates, src => src.StartingCoordinates)
            .Map(dest => dest.DestinationCoordinates, src => src.DestinationCoordinates);

        TypeAdapterConfig<DirectionsCompleteEvent, UpdateDirectionsResultCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Directions, src => src.Directions);

        TypeAdapterConfig<ImagingCompleteEvent, UpdateImagingResultCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Imaging, src => src.Imaging);

        TypeAdapterConfig<WeatherCompleteEvent, UpdateWeatherResultCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Weather, src => src.Weather);

        // Fix for error: No default constructor for type 'WeatherForecastItem', please use 'ConstructUsing' or 'MapWith'
        TypeAdapterConfig<WeatherForecastItem, WeatherForecastItem>.NewConfig()
            .ConstructUsing(src => new WeatherForecastItem(src.ForecastTimeUnixSeconds, src.LocalTimeOffsetSeconds, src.WeatherCode, src.Description, src.ImageUrl, src.MinimumTemperatureC, src.MaximumTemperatureC, src.PrecipitationProbabilityPercentage));
    }
}
