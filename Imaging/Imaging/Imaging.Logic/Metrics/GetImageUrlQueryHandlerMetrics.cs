using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Imaging.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GetImageUrlQueryHandlerMetrics : IGetImageUrlQueryHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GetImageUrl.Handled.Count", null, "The number of queries handled.");
        private static readonly Histogram<double> _externalTime = LogicMetrics.Meter.CreateHistogram<double>("GetImageUrl.External", unit: "ms", "Time taken to get data from external service.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordExternalTime(double value) => _externalTime!.Record(value);
    }
}
