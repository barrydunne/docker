namespace Microservices.Shared.Events;

/// <summary>
/// Geocoding has successfully obtained the coordinates for the starting and destination locations.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="StartingAddress">The starting location for the job.</param>
/// <param name="StartingCoordinates">The starting location coordinates for the job.</param>
/// <param name="DestinationAddress">The destination location for the job.</param>
/// <param name="DestinationCoordinates">The destination location coordinates for the job.</param>
public record LocationsReadyEvent(Guid JobId, string StartingAddress, Coordinates StartingCoordinates, string DestinationAddress, Coordinates DestinationCoordinates)
    : BaseEvent(JobId, DateTime.UtcNow);
