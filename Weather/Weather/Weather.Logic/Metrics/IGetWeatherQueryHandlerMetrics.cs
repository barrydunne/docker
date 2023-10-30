using Weather.Logic.QueryHandlers;

namespace Weather.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="GetWeatherQueryHandler"/> class.
    /// </summary>
    public interface IGetWeatherQueryHandlerMetrics
    {
        /// <summary>
        /// Increment the metrics record for the count of queries handled.
        /// </summary>
        void IncrementCount();

        /// <summary>
        /// Create a new metrics record for the time taken to get data from the external service.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordExternalTime(double value);
    }
}
