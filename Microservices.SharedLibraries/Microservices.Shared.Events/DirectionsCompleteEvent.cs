namespace Microservices.Shared.Events;

/// <summary>
/// Directions from the starting to destination location has completed.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="Directions">The directions between the locations.</param>
public record DirectionsCompleteEvent(Guid JobId, Directions Directions)
    : BaseEvent(JobId, DateTime.UtcNow);
