using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GetJobStatusQueryHandlerMetrics : IGetJobStatusQueryHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GetJobStatus.Handled.Count", null, "The number of queries handled.");
        private static readonly Histogram<double> _cacheGetTime = LogicMetrics.Meter.CreateHistogram<double>("GetJobStatus.CacheGet", unit: "ms", "Time taken to get the job from cache.");
        private static readonly Histogram<double> _loadTime = LogicMetrics.Meter.CreateHistogram<double>("GetJobStatus.Load", unit: "ms", "Time taken to load the job from the database.");
        private static readonly Histogram<double> _cacheSetTime = LogicMetrics.Meter.CreateHistogram<double>("GetJobStatus.CacheSet", unit: "ms", "Time taken to store the job in cache.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordCacheGetTime(double value) => _cacheGetTime!.Record(value);

        /// <inheritdoc/>
        public void RecordLoadTime(double value) => _loadTime!.Record(value);

        /// <inheritdoc/>
        public void RecordCacheSetTime(double value) => _cacheSetTime!.Record(value);
    }
}
