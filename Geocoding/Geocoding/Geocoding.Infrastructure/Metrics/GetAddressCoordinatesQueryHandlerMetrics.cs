using AspNet.KickStarter;
using Geocoding.Application.Queries.GetAddressCoordinates;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Geocoding.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetAddressCoordinatesQueryHandlerMetrics : IGetAddressCoordinatesQueryHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _cacheGetTime;
    private readonly Histogram<double> _externalTime;
    private readonly Histogram<double> _cacheSetTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAddressCoordinatesQueryHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public GetAddressCoordinatesQueryHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(GetAddressCoordinatesQuery).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of queries handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _cacheGetTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.cache.get", description: "Time taken to get the coordinates from cache.", unit: "ms");
        _externalTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.external", description: "Time taken to get data from external service.", unit: "ms");
        _cacheSetTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.cache.set", description: "Time taken to store the coordinates in cache.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordCacheGetTime(double value) => _cacheGetTime!.Record(value);

    /// <inheritdoc/>
    public void RecordExternalTime(double value) => _externalTime!.Record(value);

    /// <inheritdoc/>
    public void RecordCacheSetTime(double value) => _cacheSetTime!.Record(value);
}
