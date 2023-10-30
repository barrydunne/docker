namespace Microservices.Shared.Events
{
    /// <summary>
    /// The job status has changed.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="Status">The current status of the job.</param>
    /// <param name="Details">Additional details relating to the job status.</param>
    public record JobStatusUpdateEvent(Guid JobId, JobStatus Status, string? Details)
        : BaseEvent(JobId, DateTime.UtcNow);
}
