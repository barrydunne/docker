using Geocoding.Logic.QueryHandlers;

namespace Geocoding.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="GetAddressCoordinatesQueryHandler"/> class.
    /// </summary>
    public interface IGeocodeAddressesCommandHandlerMetrics
    {
        /// <summary>
        /// Increment the metrics record for the count of commands handled.
        /// </summary>
        void IncrementCount();

        /// <summary>
        /// Create a new metrics record for the time taken to perform guard checks.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordGeocodeTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to perform geocoding.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordGuardTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to publish the job created event.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordPublishTime(double value);
    }
}
