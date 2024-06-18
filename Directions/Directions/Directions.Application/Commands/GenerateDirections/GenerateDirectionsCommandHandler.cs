using AspNet.KickStarter.CQRS.Abstractions.Commands;
using AspNet.KickStarter.FunctionalResult;
using Directions.Application.Queries.GetDirections;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Directions.Application.Commands.GenerateDirections;

/// <summary>
/// The handler for the <see cref="GenerateDirectionsCommand"/> command.
/// </summary>
internal class GenerateDirectionsCommandHandler : ICommandHandler<GenerateDirectionsCommand>
{
    private readonly IQueue<DirectionsCompleteEvent> _completeQueue;
    private readonly ISender _mediator;
    private readonly IGenerateDirectionsCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateDirectionsCommandHandler"/> class.
    /// </summary>
    /// <param name="completeQueue">The queue for publishing <see cref="DirectionsCompleteEvent"/> events to.</param>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GenerateDirectionsCommandHandler(IQueue<DirectionsCompleteEvent> completeQueue, ISender mediator, IGenerateDirectionsCommandHandlerMetrics metrics, ILogger<GenerateDirectionsCommandHandler> logger)
    {
        _completeQueue = completeQueue;
        _mediator = mediator;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(GenerateDirectionsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GenerateDirectionsCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var directionsTask = _mediator.Send(new GetDirectionsQuery(command.JobId, command.StartingCoordinates, command.DestinationCoordinates), cancellationToken);
            try
            {
                await directionsTask;
            }
            catch (Exception ex)
            {
                // This should not happen due to exception handling in the query handler that will return a failed Result instead of throwing an exception. Added for safety.
                _logger.LogError(ex, "Unexpected error waiting for directions. [{CorrelationId}]", command.JobId);
            }
            var directionsResult = directionsTask.GetTaskResult();
            _metrics.RecordDirectionsTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            var completeEvent = CreateDirectionsCompleteEvent(command, directionsResult);
            await PublishEventAsync(command, completeEvent, cancellationToken);
            _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get directions. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }

    private DirectionsCompleteEvent CreateDirectionsCompleteEvent(GenerateDirectionsCommand command, Result<Microservices.Shared.Events.Directions> result)
    {
        var directions = result.IsSuccess
            ? result.Value
            : new Microservices.Shared.Events.Directions(false, null, null, null, result.Error!.Value.Message);
        return new(command.JobId, directions);
    }

    private async Task PublishEventAsync(GenerateDirectionsCommand command, DirectionsCompleteEvent completeEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Publishing directions complete event. {Event} [{CorrelationId}]", completeEvent, command.JobId);
        await _completeQueue.PublishAsync(completeEvent, cancellationToken);
        _logger.LogInformation("Published directions complete event. [{CorrelationId}]", command.JobId);
    }
}
