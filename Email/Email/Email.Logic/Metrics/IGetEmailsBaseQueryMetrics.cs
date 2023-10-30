namespace Email.Logic.Metrics
{
    /// <summary>
    /// Metrics for the various GetEmailsSentXXXQueryHandler classes.
    /// </summary>
    internal interface IGetEmailsBaseQueryMetrics
    {
        /// <summary>
        /// Increment the metrics record for the count of queries handled.
        /// </summary>
        void IncrementCount();

        /// <summary>
        /// Create a new metrics record for the time taken to load the emails.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordLoadTime(double value);
    }
}
