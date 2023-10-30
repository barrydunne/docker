using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace State.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class UpdateGeocodingResultCommandHandlerMetrics : IUpdateGeocodingResultCommandHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("UpdateGeocodingResult.Handled.Count", null, "The number of commands handled.");
        private static readonly Histogram<double> _guardTime = LogicMetrics.Meter.CreateHistogram<double>("UpdateGeocodingResult.Guard", unit: "ms", "Time taken to process input guards.");
        private static readonly Histogram<double> _updateTime = LogicMetrics.Meter.CreateHistogram<double>("UpdateGeocodingResult.Update", unit: "ms", "Time taken to update the local repository.");
        private static readonly Histogram<double> _publishTime = LogicMetrics.Meter.CreateHistogram<double>("UpdateGeocodingResult.Publish", unit: "ms", "Time taken to publish the event.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordGuardTime(double value) => _guardTime!.Record(value);

        /// <inheritdoc/>
        public void RecordUpdateTime(double value) => _updateTime!.Record(value);

        /// <inheritdoc/>
        public void RecordPublishTime(double value) => _publishTime!.Record(value);
    }
}
