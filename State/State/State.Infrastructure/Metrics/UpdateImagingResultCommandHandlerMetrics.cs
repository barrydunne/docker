using State.Application.Commands.UpdateImagingResult;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace State.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class UpdateImagingResultCommandHandlerMetrics : IUpdateImagingResultCommandHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("UpdateImagingResult.Handled.Count", null, "The number of commands handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("UpdateImagingResult.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _updateTime = ApplicationMetrics.Meter.CreateHistogram<double>("UpdateImagingResult.Update", unit: "ms", "Time taken to update the local repository.");
    private static readonly Histogram<double> _publishTime = ApplicationMetrics.Meter.CreateHistogram<double>("UpdateImagingResult.Publish", unit: "ms", "Time taken to publish the event.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordUpdateTime(double value) => _updateTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
