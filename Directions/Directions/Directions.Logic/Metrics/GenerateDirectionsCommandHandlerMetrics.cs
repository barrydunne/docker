using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Directions.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GenerateDirectionsCommandHandlerMetrics : IGenerateDirectionsCommandHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GenerateDirections.Handled.Count", null, "The number of commands handled.");
        private static readonly Histogram<double> _guardTime = LogicMetrics.Meter.CreateHistogram<double>("GenerateDirections.Guard", unit: "ms", "Time taken to process input guards.");
        private static readonly Histogram<double> _directionsTime = LogicMetrics.Meter.CreateHistogram<double>("GenerateDirections.Directions", unit: "ms", "Time taken to generate directions.");
        private static readonly Histogram<double> _publishTime = LogicMetrics.Meter.CreateHistogram<double>("GenerateDirections.Publish", unit: "ms", "Time taken to publish the event.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordGuardTime(double value) => _guardTime!.Record(value);

        /// <inheritdoc/>
        public void RecordDirectionsTime(double value) => _directionsTime!.Record(value);

        /// <inheritdoc/>
        public void RecordPublishTime(double value) => _publishTime!.Record(value);
    }
}
