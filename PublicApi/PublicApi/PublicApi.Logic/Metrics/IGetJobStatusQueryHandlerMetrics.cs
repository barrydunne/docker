using PublicApi.Logic.QueryHandlers;

namespace PublicApi.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="GetJobStatusQueryHandler"/> class.
    /// </summary>
    internal interface IGetJobStatusQueryHandlerMetrics
    {
        /// <summary>
        /// Increment the metrics record for the count of queries handled.
        /// </summary>
        void IncrementCount();

        /// <summary>
        /// Create a new metrics record for the time taken to get the job from cache.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordCacheGetTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to load the job from the database.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordLoadTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to store the job in the cache.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordCacheSetTime(double value);
    }
}
