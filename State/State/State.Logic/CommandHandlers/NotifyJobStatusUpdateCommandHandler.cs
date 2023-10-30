using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using CSharpFunctionalExtensions;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Logic.Commands;
using State.Logic.Metrics;
using System.Diagnostics;

namespace State.Logic.CommandHandlers
{
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

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure<Guid>(guardResult.Error);

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
                return Result.Failure(ex.Message);
            }
        }

        private Result PerformGuardChecks(NotifyJobStatusUpdateCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(NotifyJobStatusUpdateCommand.JobId));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to validate command properties. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }

        private async Task PublishEventAsync(NotifyJobStatusUpdateCommand command, JobStatusUpdateEvent jobStatusUpdateEvent, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Publishing state update event. {Event} [{CorrelationId}]", jobStatusUpdateEvent, command.JobId);
            await _statusQueue.PublishAsync(jobStatusUpdateEvent, cancellationToken);
            _logger.LogInformation("Published state update event. [{CorrelationId}]", command.JobId);
        }
    }
}
