using Imaging.Application.Commands.SaveImage;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Imaging.Infrastructure.Metrics;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal class SaveImageCommandHandlerMetrics : ISaveImageCommandHandlerMetrics
{
    private static readonly Counter<long> _count = ApplicationMetrics.Meter.CreateCounter<long>("SaveImage.Handled.Count", null, "The number of commands handled.");
    private static readonly Histogram<double> _guardTime = ApplicationMetrics.Meter.CreateHistogram<double>("SaveImage.Guard", unit: "ms", "Time taken to process input guards.");
    private static readonly Histogram<double> _imagingTime = ApplicationMetrics.Meter.CreateHistogram<double>("SaveImage.Imaging", unit: "ms", "Time taken to obtain image.");
    private static readonly Histogram<double> _downloadTime = ApplicationMetrics.Meter.CreateHistogram<double>("SaveImage.Download", unit: "ms", "Time taken to download image.");
    private static readonly Histogram<double> _uploadTime = ApplicationMetrics.Meter.CreateHistogram<double>("SaveImage.Upload", unit: "ms", "Time taken to upload image.");
    private static readonly Histogram<double> _publishTime = ApplicationMetrics.Meter.CreateHistogram<double>("SaveImage.Publish", unit: "ms", "Time taken to publish the event.");

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
