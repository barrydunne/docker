using AspNet.KickStarter;
using Imaging.Application.Commands.SaveImage;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Imaging.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class SaveImageCommandHandlerMetrics : ISaveImageCommandHandlerMetrics
{
    private readonly Counter<long> _count;
    private readonly Histogram<double> _guardTime;
    private readonly Histogram<double> _imagingTime;
    private readonly Histogram<double> _downloadTime;
    private readonly Histogram<double> _uploadTime;
    private readonly Histogram<double> _publishTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveImageCommandHandlerMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory to supply the <see cref="Meter"/>.</param>
    public SaveImageCommandHandlerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.CreateAssemblyMeter();
        var subjectName = nameof(SaveImageCommand).ToLower();

        _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.handled.count", description: "The number of commands handled.");
        _guardTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.guard", description: "Time taken to process input guards.", unit: "ms");
        _imagingTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.imaging", description: "Time taken to obtain image.", unit: "ms");
        _downloadTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.download", description: "Time taken to download image.", unit: "ms");
        _uploadTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.upload", description: "Time taken to upload image.", unit: "ms");
        _publishTime = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.publish", description: "Time taken to publish the event.", unit: "ms");
    }

    /// <inheritdoc/>
    public void IncrementCount() => _count!.Add(1);

    /// <inheritdoc/>
    public void RecordGuardTime(double value) => _guardTime!.Record(value);

    /// <inheritdoc/>
    public void RecordImagingTime(double value) => _imagingTime!.Record(value);

    /// <inheritdoc/>
    public void RecordDownloadTime(double value) => _downloadTime!.Record(value);

    /// <inheritdoc/>
    public void RecordUploadTime(double value) => _uploadTime!.Record(value);

    /// <inheritdoc/>
    public void RecordPublishTime(double value) => _publishTime!.Record(value);
}
