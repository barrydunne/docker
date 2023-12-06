using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Models;
using State.Application.Repositories;
using System.Diagnostics;

namespace State.Application.Commands.UpdateGeocodingResult;

/// <summary>
/// The handler for the <see cref="UpdateGeocodingResultCommand"/> command.
/// </summary>
internal class UpdateGeocodingResultCommandHandler : ICommandHandler<UpdateGeocodingResultCommand>
{
    private readonly IJobRepository _jobRepository;
    private readonly ISender _mediator;
    private readonly IQueue<LocationsReadyEvent> _locationsReadyQueue;
    private readonly IUpdateGeocodingResultCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateGeocodingResultCommandHandler"/> class.
    /// </summary>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    /// <param name="locationsReadyQueue">The queue for publishing <see cref="LocationsReadyEvent"/> events to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateGeocodingResultCommandHandler(IJobRepository jobRepository, ISender mediator, IQueue<LocationsReadyEvent> locationsReadyQueue, IUpdateGeocodingResultCommandHandlerMetrics metrics, ILogger<UpdateGeocodingResultCommandHandler> logger)
    {
        _jobRepository = jobRepository;
        _mediator = mediator;
        _locationsReadyQueue = locationsReadyQueue;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(UpdateGeocodingResultCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateGeocodingResultCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var successful = command.StartingCoordinates.IsSuccessful && command.DestinationCoordinates.IsSuccessful;
            var updateCount = await _jobRepository.UpdateJobStatusAsync(command.JobId, successful, new GeocodingResult(command.StartingCoordinates, command.DestinationCoordinates), cancellationToken);
            _metrics.RecordUpdateTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // It is possible that the GeocodingCompleteEvent message is processed before the JobCreatedEvent.
            // If we do not yet have the job in the repository, return unhandled
            if (updateCount == 0)
                return new Error("Job not yet available for update.");

            if (successful)
            {
                var job = await _jobRepository.GetJobByIdAsync(command.JobId, cancellationToken);
                var locationsReadyEvent = new LocationsReadyEvent(command.JobId, job!.StartingAddress, command.StartingCoordinates.Coordinates!, job.DestinationAddress, command.DestinationCoordinates.Coordinates!);
                await PublishEventAsync(command, locationsReadyEvent, cancellationToken);
            }
            else
            {
                var details = $"Geocoding starting location {(command.StartingCoordinates.IsSuccessful ? "succeeded" : "failed")}. Geocoding destination {(command.DestinationCoordinates.IsSuccessful ? "succeeded" : "failed")}";
                _logger.LogInformation(details);
                await _mediator.Send(new NotifyJobStatusUpdateCommand(command.JobId, JobStatus.Failed, details));
            }
            _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to geocode addresses. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }

    private async Task PublishEventAsync(UpdateGeocodingResultCommand command, LocationsReadyEvent locationsReadyEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Publishing locations ready event. {Event} [{CorrelationId}]", locationsReadyEvent, command.JobId);
        await _locationsReadyQueue.PublishAsync(locationsReadyEvent, cancellationToken);
        _logger.LogInformation("Published locations ready event. [{CorrelationId}]", command.JobId);
    }
}
