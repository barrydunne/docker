namespace Email.Application.Queries;

/// <summary>
/// Metrics for the various GetEmailsSentXXXQueryHandler classes.
/// </summary>
public interface IGetEmailsBaseQueryMetrics
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
    /// Create a new metrics record for the time taken to load the emails.
    /// </summary>
    /// <param name="value">The time taken in ms.</param>
    void RecordLoadTime(double value);
}
