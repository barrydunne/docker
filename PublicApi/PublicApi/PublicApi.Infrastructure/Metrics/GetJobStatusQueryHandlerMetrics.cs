using AspNet.KickStarter;
using PublicApi.Application.Queries.GetJobStatus;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetJobStatusQueryHandlerMetrics : IGetJobStatusQueryHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _cacheGetTime;
    private readonly Histogram<double> _loadTime;
    private readonly Histogram<double> _cacheSetTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetJobStatusQueryHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public GetJobStatusQueryHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(GetJobStatusQuery).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of queries handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _cacheGetTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.cache.get", description: "Time taken to get the job from cache.", unit: "ms");
        _loadTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.load", description: "Time taken to load the job from the database.", unit: "ms");
        _cacheSetTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.cache.set", description: "Time taken to store the job in cache.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordCacheGetTime(double value) => _cacheGetTime!.Record(value);

    /// <inheritdoc/>
    public void RecordLoadTime(double value) => _loadTime!.Record(value);

    /// <inheritdoc/>
    public void RecordCacheSetTime(double value) => _cacheSetTime!.Record(value);
}
