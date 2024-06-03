using AspNet.KickStarter;
using Imaging.Application.Queries.GetImageUrl;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Imaging.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetImageUrlQueryHandlerMetrics : IGetImageUrlQueryHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _externalTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetImageUrlQueryHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public GetImageUrlQueryHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(GetImageUrlQuery).ToLower();

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
