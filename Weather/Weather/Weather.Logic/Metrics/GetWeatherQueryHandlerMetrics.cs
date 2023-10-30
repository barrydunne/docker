using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Weather.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GetWeatherQueryHandlerMetrics : IGetWeatherQueryHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GetWeather.Handled.Count", null, "The number of queries handled.");
        private static readonly Histogram<double> _externalTime = LogicMetrics.Meter.CreateHistogram<double>("GetWeather.External", unit: "ms", "Time taken to get data from external service.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordExternalTime(double value) => _externalTime!.Record(value);
    }
}
