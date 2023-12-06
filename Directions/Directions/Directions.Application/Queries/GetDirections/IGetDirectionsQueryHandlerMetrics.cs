namespace Directions.Application.Queries.GetDirections;

/// <summary>
/// Metrics for the <see cref="GetDirectionsQueryHandler"/> class.
/// </summary>
public interface IGetDirectionsQueryHandlerMetrics
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
    /// Create a new metrics record for the time taken to get data from the external service.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordExternalTime(double value);
}
