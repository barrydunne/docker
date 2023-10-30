using PublicApi.Logic.CommandHandlers;

namespace PublicApi.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="CreateJobCommandHandler"/> class.
    /// </summary>
    internal interface ICreateJobCommandHandlerMetrics
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
        /// Create a new metrics record for the time taken to perform idempotency checks.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordIdempotencyTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to save the new job.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordPublishTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to publish the job created event.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordSaveTime(double value);
    }
}
