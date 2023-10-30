using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class UpdateStatusCommandHandlerMetrics : IUpdateStatusCommandHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("UpdateStatus.Handled.Count", null, "The number of commands handled.");
        private static readonly Histogram<double> _guardTime = LogicMetrics.Meter.CreateHistogram<double>("UpdateStatus.Guard", unit: "ms", "Time taken to process input guards.");
        private static readonly Histogram<double> _updateTime = LogicMetrics.Meter.CreateHistogram<double>("UpdateStatus.Update", unit: "ms", "Time taken to update the job status.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordGuardTime(double value) => _guardTime!.Record(value);

        /// <inheritdoc/>
        public void RecordUpdateTime(double value) => _updateTime!.Record(value);
    }
}
