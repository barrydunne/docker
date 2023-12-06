using Email.Application.Queries.GetEmailsSentToRecipient;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GetEmailsSentToRecipientQueryHandlerMetrics : IGetEmailsSentToRecipientQueryHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("GetEmailsSentToRecipient.Handled.Count", null, "The number of queries handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetEmailsSentToRecipient.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _loadTime = ApplicationMetrics.Meter.CreateHistogram<double>("GetEmailsSentToRecipient.Load", unit: "ms", "Time taken to load the emails.");

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordLoadTime(double value) => _loadTime!.Record(value);
}
