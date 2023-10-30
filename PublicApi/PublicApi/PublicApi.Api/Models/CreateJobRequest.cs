namespace PublicApi.Api.Models
{
    /// <summary>
    /// Request a new processing job to be started.
    /// </summary>
    public class CreateJobRequest
    {
        /// <summary>
        /// Gets or sets the starting location for the job.
        /// </summary>
        public string StartingAddress { get; set; } = null!;

        /// <summary>
        /// Gets or sets the destination location for the job.
        /// </summary>
        public string DestinationAddress { get; set; } = null!;

        /// <summary>
        /// Gets or sets the notification email address for the job.
        /// </summary>
        public string Email { get; set; } = null!;
    }
}
