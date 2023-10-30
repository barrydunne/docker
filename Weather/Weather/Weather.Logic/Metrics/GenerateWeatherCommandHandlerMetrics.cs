using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Weather.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GenerateWeatherCommandHandlerMetrics : IGenerateWeatherCommandHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GenerateWeather.Handled.Count", null, "The number of commands handled.");
        private static readonly Histogram<double> _guardTime = LogicMetrics.Meter.CreateHistogram<double>("GenerateWeather.Guard", unit: "ms", "Time taken to process input guards.");
        private static readonly Histogram<double> _weatherTime = LogicMetrics.Meter.CreateHistogram<double>("GenerateWeather.Weather", unit: "ms", "Time taken to generate weather forecast.");
        private static readonly Histogram<double> _publishTime = LogicMetrics.Meter.CreateHistogram<double>("GenerateWeather.Publish", unit: "ms", "Time taken to publish the event.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordGuardTime(double value) => _guardTime!.Record(value);

        /// <inheritdoc/>
        public void RecordWeatherTime(double value) => _weatherTime!.Record(value);

        /// <inheritdoc/>
        public void RecordPublishTime(double value) => _publishTime!.Record(value);
    }
}
