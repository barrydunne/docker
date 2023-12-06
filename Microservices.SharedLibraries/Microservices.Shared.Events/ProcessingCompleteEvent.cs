namespace Microservices.Shared.Events;

/// <summary>
/// Imaging of the destination location has completed.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="Email">The notification email address for the job.</param>
/// <param name="StartingAddress">The starting location for the job.</param>
/// <param name="DestinationAddress">The destination location for the job.</param>
/// <param name="Directions">The directions between the locations.</param>
/// <param name="Weather">The weather forecast at the destination.</param>
/// <param name="Imaging">The result of imaging.</param>
public record ProcessingCompleteEvent(Guid JobId, string Email, string StartingAddress, string DestinationAddress, Directions Directions, WeatherForecast Weather, ImagingResult Imaging)
    : BaseEvent(JobId, DateTime.UtcNow);
