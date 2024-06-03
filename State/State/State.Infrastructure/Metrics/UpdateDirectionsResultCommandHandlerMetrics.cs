using AspNet.KickStarter;
using State.Application.Commands.UpdateDirectionsResult;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace State.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class UpdateDirectionsResultCommandHandlerMetrics : IUpdateDirectionsResultCommandHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _updateTime;
    private readonly Histogram<double> _publishTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDirectionsResultCommandHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public UpdateDirectionsResultCommandHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(UpdateDirectionsResultCommand).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of commands handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _updateTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.update", description: "Time taken to update the local repository.", unit: "ms");
        _publishTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.publish", description: "Time taken to publish the event.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordUpdateTime(double value) => _updateTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
