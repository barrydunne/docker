namespace Microservices.Shared.Events
{
    /// <summary>
    /// Weather from the starting to destination location has completed.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="Weather">The weather forecast at the destination.</param>
    public record WeatherCompleteEvent(Guid JobId, WeatherForecast Weather)
        : BaseEvent(JobId, DateTime.UtcNow);
}
