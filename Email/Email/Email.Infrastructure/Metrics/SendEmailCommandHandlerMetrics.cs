using Email.Application.Commands.SendEmail;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class SendEmailCommandHandlerMetrics : ISendEmailCommandHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("SendEmail.Handled.Count", null, "The number of commands handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("SendEmail.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _imageTime = ApplicationMetrics.Meter.CreateHistogram<double>("SendEmail.Image", unit: "ms", "Time taken to obtain the image.");
    private static readonly Histogram<double> _generateTime = ApplicationMetrics.Meter.CreateHistogram<double>("SendEmail.Generate", unit: "ms", "Time taken to generate the email.");
    private static readonly Histogram<double> _emailTime = ApplicationMetrics.Meter.CreateHistogram<double>("SendEmail.Email", unit: "ms", "Time taken to send the email.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordImageTime(double value) => _imageTime!.Record(value);

    /// <inheritdoc/>
    public void RecordGenerateTime(double value) => _generateTime!.Record(value);

    /// <inheritdoc/>
    public void RecordEmailTime(double value) => _emailTime!.Record(value);
}
