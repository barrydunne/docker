using Email.Logic.CommandHandlers;

namespace Email.Logic.Metrics
{
    /// <summary>
    /// Metrics for the <see cref="SendEmailCommandHandler"/> class.
    /// </summary>
    public interface ISendEmailCommandHandlerMetrics
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
        /// Create a new metrics record for the time taken to obtain the image.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordImageTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to generate the email.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordGenerateTime(double value);

        /// <summary>
        /// Create a new metrics record for the time taken to send the email.
        /// </summary>
        /// <param name="value">The time taken in ms.</param>
        void RecordEmailTime(double value);
    }
}
