namespace Microservices.Shared.Events;

/// <summary>
/// Geocoding of the starting and destination locations has completed.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="StartingCoordinates">The geocoding result for the starting location for the job.</param>
/// <param name="DestinationCoordinates">The geocoding result for the destination location for the job.</param>
public record GeocodingCompleteEvent(Guid JobId, GeocodingCoordinates StartingCoordinates, GeocodingCoordinates DestinationCoordinates)
    : BaseEvent(JobId, DateTime.UtcNow);
