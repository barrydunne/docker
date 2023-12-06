using Directions.Application.Queries.GetDirections;

namespace Directions.Application.Commands.GenerateDirections;

/// <summary>
/// Metrics for the <see cref="GetDirectionsQueryHandler"/> class.
/// </summary>
public interface IGenerateDirectionsCommandHandlerMetrics
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
    /// Create a new metrics record for the time taken to generate directions.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordDirectionsTime(double value);

    /// <summary>
    /// Create a new metrics record for the time taken to publish the job created event.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordPublishTime(double value);
}
