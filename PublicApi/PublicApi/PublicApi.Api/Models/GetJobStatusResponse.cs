using Microservices.Shared.Events;

namespace PublicApi.Api.Models
{
    /// <summary>
    /// The response from a job status request.
    /// </summary>
    public class GetJobStatusResponse
    {
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
