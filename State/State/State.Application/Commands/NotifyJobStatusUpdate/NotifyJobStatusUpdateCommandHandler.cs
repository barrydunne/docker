using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.NotifyJobStatusUpdate;

/// <summary>
/// The handler for the <see cref="NotifyJobStatusUpdateCommand"/> command.
/// </summary>
internal class NotifyJobStatusUpdateCommandHandler : ICommandHandler<NotifyJobStatusUpdateCommand>
{
    private readonly IQueue<JobStatusUpdateEvent> _statusQueue;
    private readonly INotifyJobStatusUpdateCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyJobStatusUpdateCommandHandler"/> class.
    /// </summary>
    /// <param name="statusQueue">The queue for publishing <see cref="JobStatusUpdateEvent"/> events to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public NotifyJobStatusUpdateCommandHandler(IQueue<JobStatusUpdateEvent> statusQueue, INotifyJobStatusUpdateCommandHandlerMetrics metrics, ILogger<NotifyJobStatusUpdateCommandHandler> logger)
    {
        _statusQueue = statusQueue;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(NotifyJobStatusUpdateCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(NotifyJobStatusUpdateCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var jobStatusUpdateEvent = new JobStatusUpdateEvent(command.JobId, command.Status, command.Details);
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

    private async Task PublishEventAsync(NotifyJobStatusUpdateCommand command, JobStatusUpdateEvent jobStatusUpdateEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Publishing state update event. {Event} [{CorrelationId}]", jobStatusUpdateEvent, command.JobId);
        await _statusQueue.PublishAsync(jobStatusUpdateEvent, cancellationToken);
        _logger.LogInformation("Published state update event. [{CorrelationId}]", command.JobId);
    }
}
