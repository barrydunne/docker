using AspNet.KickStarter.CQRS.Abstractions.Commands;
using AspNet.KickStarter.FunctionalResult;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.NotifyProcessingComplete;

/// <summary>
/// The handler for the <see cref="NotifyProcessingCompleteCommand"/> command.
/// </summary>
internal class NotifyProcessingCompleteCommandHandler : ICommandHandler<NotifyProcessingCompleteCommand>
{
    private readonly IQueue<ProcessingCompleteEvent> _statusQueue;
    private readonly INotifyProcessingCompleteCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyProcessingCompleteCommandHandler"/> class.
    /// </summary>
    /// <param name="statusQueue">The queue for publishing <see cref="ProcessingCompleteEvent"/> events to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public NotifyProcessingCompleteCommandHandler(IQueue<ProcessingCompleteEvent> statusQueue, INotifyProcessingCompleteCommandHandlerMetrics metrics, ILogger<NotifyProcessingCompleteCommandHandler> logger)
    {
        _statusQueue = statusQueue;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(NotifyProcessingCompleteCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(NotifyProcessingCompleteCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var jobStatusUpdateEvent = new ProcessingCompleteEvent(command.JobId, command.Job.Email, command.Job.StartingAddress, command.Job.DestinationAddress, command.Job.Directions!, command.Job.WeatherForecast!, command.Job.ImagingResult!);
            await PublishEventAsync(command, jobStatusUpdateEvent, cancellationToken);
            _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }

    private async Task PublishEventAsync(NotifyProcessingCompleteCommand command, ProcessingCompleteEvent jobStatusUpdateEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Publishing processing complete event. {Event} [{CorrelationId}]", jobStatusUpdateEvent, command.JobId);
        await _statusQueue.PublishAsync(jobStatusUpdateEvent, cancellationToken);
        _logger.LogInformation("Published processing complete event. [{CorrelationId}]", command.JobId);
    }
}
