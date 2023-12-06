using Microservices.Shared.Events;

namespace State.Application.Models;

/// <summary>
/// The result of geocoding.
/// </summary>
/// <param name="StartingCoordinates">The geocoding result for the starting location for the job.</param>
/// <param name="DestinationCoordinates">The geocoding result for the destination location for the job.</param>
public record GeocodingResult(GeocodingCoordinates StartingCoordinates, GeocodingCoordinates DestinationCoordinates);
