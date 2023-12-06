using Geocoding.Application.Queries.GetAddressCoordinates;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Geocoding.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetAddressCoordinatesQueryHandlerMetrics : IGetAddressCoordinatesQueryHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("GetAddressCoordinates.Handled.Count", null, "The number of queries handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetAddressCoordinates.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _externalTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetAddressCoordinates.External", unit: "ms", "Time taken to get data from external service.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordExternalTime(double value) => _externalTime!.Record(value);
}
