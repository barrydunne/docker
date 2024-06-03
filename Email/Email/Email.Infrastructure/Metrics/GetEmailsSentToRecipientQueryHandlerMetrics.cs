using AspNet.KickStarter;
using Email.Application.Queries.GetEmailsSentToRecipient;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetEmailsSentToRecipientQueryHandlerMetrics : IGetEmailsSentToRecipientQueryHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _loadTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentToRecipientQueryHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public GetEmailsSentToRecipientQueryHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(GetEmailsSentToRecipientQuery).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of queries handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _loadTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.load", description: "Time taken to load the emails.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordLoadTime(double value) => _loadTime!.Record(value);
}
