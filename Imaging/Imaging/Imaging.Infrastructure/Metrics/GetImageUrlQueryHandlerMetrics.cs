using Imaging.Application.Queries.GetImageUrl;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Imaging.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetImageUrlQueryHandlerMetrics : IGetImageUrlQueryHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("GetImageUrl.Handled.Count", null, "The number of queries handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetImageUrl.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _externalTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetImageUrl.External", unit: "ms", "Time taken to get data from external service.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordExternalTime(double value) => _externalTime!.Record(value);
}
