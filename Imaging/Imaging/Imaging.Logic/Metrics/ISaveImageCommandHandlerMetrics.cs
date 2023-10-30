using Imaging.Logic.CommandHandlers;

namespace Imaging.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="SaveImageCommandHandler"/> class.
    /// </summary>
    public interface ISaveImageCommandHandlerMetrics
    {
        /// <summary>
        /// Increment the metrics record for the count of commands handled.
        /// </summary>
        void IncrementCount();

        /// <summary>
        /// Create a new metrics record for the time taken to perform guard checks.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordGuardTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to obtain an image.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordImagingTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to download an image.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordDownloadTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to upload an image.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordUploadTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to publish the job created event.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordPublishTime(double value);
    }
}
