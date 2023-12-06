namespace Microservices.Shared.Events;

/// <summary>
/// The status of a job.
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// The job has been accepted but not yet begun to be processed.
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// The job is currently processing.
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Processing was unsuccessful.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Processing was at least partially successful.
    /// </summary>
    Complete = 4
}
