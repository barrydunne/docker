using Geocoding.Application.Commands.GeocodeAddresses;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Geocoding.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GeocodeAddressesCommandHandlerMetrics : IGeocodeAddressesCommandHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("GeocodeAddresses.Handled.Count", null, "The number of commands handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("GeocodeAddresses.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _geocodeTime = ApplicationMetrics.Meter.CreateHistogram<double>("GeocodeAddresses.Geocode", unit: "ms", "Time taken to geocode both addresses.");
    private static readonly Histogram<double> _publishTime = ApplicationMetrics.Meter.CreateHistogram<double>("GeocodeAddresses.Publish", unit: "ms", "Time taken to publish the event.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordGeocodeTime(double value) => _geocodeTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
