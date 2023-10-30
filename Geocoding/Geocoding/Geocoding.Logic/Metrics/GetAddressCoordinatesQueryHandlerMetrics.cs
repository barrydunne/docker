using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Geocoding.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GetAddressCoordinatesQueryHandlerMetrics : IGetAddressCoordinatesQueryHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GetAddressCoordinates.Handled.Count", null, "The number of queries handled.");
        private static readonly Histogram<double> _externalTime = LogicMetrics.Meter.CreateHistogram<double>("GetAddressCoordinates.External", unit: "ms", "Time taken to get data from external service.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordExternalTime(double value) => _externalTime!.Record(value);
    }
}
