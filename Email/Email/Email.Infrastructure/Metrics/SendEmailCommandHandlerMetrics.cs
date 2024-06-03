using AspNet.KickStarter;
using Email.Application.Commands.SendEmail;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class SendEmailCommandHandlerMetrics : ISendEmailCommandHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _imageTime;
    private readonly Histogram<double> _generateTime;
    private readonly Histogram<double> _emailTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendEmailCommandHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public SendEmailCommandHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(SendEmailCommand).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of commands handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _imageTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.image", description: "Time taken to obtain the image.", unit: "ms");
        _generateTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.generate", description: "Time taken to generate the email.", unit: "ms");
        _emailTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.email", description: "Time taken to send the email.", unit: "ms");
    }

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
