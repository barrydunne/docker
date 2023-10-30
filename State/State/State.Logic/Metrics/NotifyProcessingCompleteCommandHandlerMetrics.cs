using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace State.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class NotifyProcessingCompleteCommandHandlerMetrics : INotifyProcessingCompleteCommandHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("NotifyProcessingComplete.Handled.Count", null, "The number of commands handled.");
        private static readonly Histogram<double> _guardTime = LogicMetrics.Meter.CreateHistogram<double>("NotifyProcessingComplete.Guard", unit: "ms", "Time taken to process input guards.");
        private static readonly Histogram<double> _publishTime = LogicMetrics.Meter.CreateHistogram<double>("NotifyProcessingComplete.Publish", unit: "ms", "Time taken to publish the event.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordGuardTime(double value) => _guardTime!.Record(value);

        /// <inheritdoc/>
        public void RecordPublishTime(double value) => _publishTime!.Record(value);
    }
}
