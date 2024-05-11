namespace Geocoding.Application.Queries.GetAddressCoordinates;

/// <summary>
/// Metrics for the <see cref="GetAddressCoordinatesQueryHandler"/> class.
/// </summary>
public interface IGetAddressCoordinatesQueryHandlerMetrics
{
    /// <summary>
    /// Increment the metrics record for the count of queries handled.
    /// </summary>
    void IncrementCount();

    /// <summary>
    /// Create a new metrics record for the time taken to perform guard checks.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordGuardTime(double value);

    /// <summary>
    /// Create a new metrics record for the time taken to get the coordinates from cache.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordCacheGetTime(double value);

    /// <summary>
    /// Create a new metrics record for the time taken to get data from the external service.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordExternalTime(double value);

    /// <summary>
    /// Create a new metrics record for the time taken to store the coordinates in the cache.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordCacheSetTime(double value);
}
