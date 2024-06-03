using AspNet.KickStarter;
using State.Application.Commands.NotifyProcessingComplete;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace State.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class NotifyProcessingCompleteCommandHandlerMetrics : INotifyProcessingCompleteCommandHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _publishTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyProcessingCompleteCommandHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public NotifyProcessingCompleteCommandHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(NotifyProcessingCompleteCommand).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of queries handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _publishTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.load", description: "Time taken to publish the event.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
