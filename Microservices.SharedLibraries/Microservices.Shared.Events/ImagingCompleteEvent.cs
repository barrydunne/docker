namespace Microservices.Shared.Events
{
    /// <summary>
    /// Imaging of the destination location has completed.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="Imaging">The result of imaging.</param>
    public record ImagingCompleteEvent(Guid JobId, ImagingResult Imaging)
        : BaseEvent(JobId, DateTime.UtcNow);
}
