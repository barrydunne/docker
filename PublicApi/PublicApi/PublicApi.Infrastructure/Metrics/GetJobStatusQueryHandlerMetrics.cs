using PublicApi.Application.Queries.GetJobStatus;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetJobStatusQueryHandlerMetrics : IGetJobStatusQueryHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("GetJobStatus.Handled.Count", null, "The number of queries handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetJobStatus.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _cacheGetTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetJobStatus.CacheGet", unit: "ms", "Time taken to get the job from cache.");
    private static readonly Histogram<double> _loadTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetJobStatus.Load", unit: "ms", "Time taken to load the job from the database.");
    private static readonly Histogram<double> _cacheSetTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetJobStatus.CacheSet", unit: "ms", "Time taken to store the job in cache.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordCacheGetTime(double value) => _cacheGetTime!.Record(value);

    /// <inheritdoc/>
    public void RecordLoadTime(double value) => _loadTime!.Record(value);

    /// <inheritdoc/>
    public void RecordCacheSetTime(double value) => _cacheSetTime!.Record(value);
}
