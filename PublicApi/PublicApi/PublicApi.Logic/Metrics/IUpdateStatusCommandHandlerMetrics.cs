using PublicApi.Logic.CommandHandlers;

namespace PublicApi.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="UpdateStatusCommandHandler"/> class.
    /// </summary>
    internal interface IUpdateStatusCommandHandlerMetrics
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
        /// Create a new metrics record for the time taken to update the job status.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordUpdateTime(double value);
    }
}
