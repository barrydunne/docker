using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class CreateJobCommandHandlerMetrics : ICreateJobCommandHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("CreateJob.Handled.Count", null, "The number of commands handled.");
        private static readonly Histogram<double> _guardTime = LogicMetrics.Meter.CreateHistogram<double>("CreateJob.Guard", unit: "ms", "Time taken to process input guards.");
        private static readonly Histogram<double> _idempotencyTime = LogicMetrics.Meter.CreateHistogram<double>("CreateJob.Idempotency", unit: "ms", "Time taken to check idempotency.");
        private static readonly Histogram<double> _saveTime = LogicMetrics.Meter.CreateHistogram<double>("CreateJob.Save", unit: "ms", "Time taken to save the job.");
        private static readonly Histogram<double> _publishTime = LogicMetrics.Meter.CreateHistogram<double>("CreateJob.Publish", unit: "ms", "Time taken to publish the event.");

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
}
