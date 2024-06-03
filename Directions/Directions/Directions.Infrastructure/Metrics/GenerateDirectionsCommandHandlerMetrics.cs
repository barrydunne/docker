using AspNet.KickStarter;
using Directions.Application.Commands.GenerateDirections;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Directions.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class GenerateDirectionsCommandHandlerMetrics : IGenerateDirectionsCommandHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _directionsTime;
    private readonly Histogram<double> _publishTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateDirectionsCommandHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public GenerateDirectionsCommandHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(GenerateDirectionsCommand).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of commands handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _directionsTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.directions", description: "Time taken to generate directions.", unit: "ms");
        _publishTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.publish", description: "Time taken to publish the event.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordDirectionsTime(double value) => _directionsTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
