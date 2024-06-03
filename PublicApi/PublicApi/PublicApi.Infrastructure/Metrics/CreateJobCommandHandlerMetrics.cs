using AspNet.KickStarter;
using PublicApi.Application.Commands.CreateJob;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class CreateJobCommandHandlerMetrics : ICreateJobCommandHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _idempotencyTime;
    private readonly Histogram<double> _saveTime;
    private readonly Histogram<double> _publishTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJobCommandHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public CreateJobCommandHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(CreateJobCommand).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of commands handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _idempotencyTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.idempotency", description: "Time taken to check idempotency.", unit: "ms");
        _saveTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.save", description: "Time taken to save the job.", unit: "ms");
        _publishTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.publish", description: "Time taken to publish the event.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordIdempotencyTime(double value) => _idempotencyTime!.Record(value);

    /// <inheritdoc/>
    public void RecordSaveTime(double value) => _saveTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
