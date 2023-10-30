using Microservices.Shared.Events;

namespace PublicApi.Repository.Models
{
    /// <summary>
    /// Represents a processing job.
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Gets or sets the correlation id for tracking this job.
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Gets or sets the idempotency key for the request to create the job.
        /// </summary>
        public string IdempotencyKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the creation time of this job.
        /// </summary>
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the current status of this job.
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Gets or sets any additional information about this job.
        /// </summary>
        public string? AdditionalInformation { get; set; }
    }
}
