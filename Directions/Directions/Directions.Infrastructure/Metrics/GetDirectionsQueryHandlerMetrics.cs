using AspNet.KickStarter;
using Directions.Application.Queries.GetDirections;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Directions.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetDirectionsQueryHandlerMetrics : IGetDirectionsQueryHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _externalTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDirectionsQueryHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public GetDirectionsQueryHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(GetDirectionsQuery).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of queries handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _externalTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.external", description: "Time taken to get data from external service.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordExternalTime(double value) => _externalTime!.Record(value);
}
