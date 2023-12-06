using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Geocoding.Application.Queries.GetAddressCoordinates;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Geocoding.Application.Commands.GeocodeAddresses;

/// <summary>
/// The handler for the <see cref="GeocodeAddressesCommand"/> command.
/// </summary>
internal class GeocodeAddressesCommandHandler : ICommandHandler<GeocodeAddressesCommand>
{
    private readonly IQueue<GeocodingCompleteEvent> _completeQueue;
    private readonly ISender _mediator;
    private readonly IGeocodeAddressesCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeocodeAddressesCommandHandler"/> class.
    /// </summary>
    /// <param name="completeQueue">The queue for publishing <see cref="GeocodingCompleteEvent"/> events to.</param>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GeocodeAddressesCommandHandler(IQueue<GeocodingCompleteEvent> completeQueue, ISender mediator, IGeocodeAddressesCommandHandlerMetrics metrics, ILogger<GeocodeAddressesCommandHandler> logger)
    {
        _completeQueue = completeQueue;
        _mediator = mediator;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(GeocodeAddressesCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GeocodeAddressesCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var (geocodeStartingResult, geocodeDestinationResult) = await GeocodeAddressesAsync(command, cancellationToken);
            _metrics.RecordGeocodeTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            var completeEvent = CreateGeocodingCompleteEvent(command, geocodeStartingResult, geocodeDestinationResult);
            await PublishEventAsync(command, completeEvent, cancellationToken);
            _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to geocode addresses. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }

    private async Task<(Result<Coordinates> StartingResult, Result<Coordinates> DestinationResult)> GeocodeAddressesAsync(GeocodeAddressesCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Requesting individual address locations. [{CorrelationId}]", command.JobId);
        var geocodeStartingQuery = _mediator.Send(new GetAddressCoordinatesQuery(command.JobId, command.StartingAddress), cancellationToken);
        var geocodeDestinationQuery = _mediator.Send(new GetAddressCoordinatesQuery(command.JobId, command.DestinationAddress), cancellationToken);
        try
        {
            await Task.WhenAll(geocodeStartingQuery, geocodeDestinationQuery);
        }
        catch (Exception ex)
        {
            // This should not happen due to exception handling in the query handler that will return a failed Result instead of throwing an exception. Added for safety.
            _logger.LogError(ex, "Unexpected error waiting for geocoding. [{CorrelationId}]", command.JobId);
        }

        // If there were exceptions, convert into failed Results
        var geocodeStartingQueryResult = geocodeStartingQuery.GetTaskResult();
        var geocodeDestinationQueryResult = geocodeDestinationQuery.GetTaskResult();

        return (geocodeStartingQueryResult, geocodeDestinationQueryResult);
    }

    private GeocodingCompleteEvent CreateGeocodingCompleteEvent(GeocodeAddressesCommand command, Result<Coordinates> geocodeStartingQueryResult, Result<Coordinates> geocodeDestinationQueryResult)
    {
        var starting = geocodeStartingQueryResult.IsSuccess ? new GeocodingCoordinates(true, geocodeStartingQueryResult.Value, null) : new GeocodingCoordinates(false, null, geocodeStartingQueryResult.Error!.Value.Message);
        var destination = geocodeDestinationQueryResult.IsSuccess ? new GeocodingCoordinates(true, geocodeDestinationQueryResult.Value, null) : new GeocodingCoordinates(false, null, geocodeDestinationQueryResult.Error!.Value.Message);
        return new(command.JobId, starting, destination);
    }

    private async Task PublishEventAsync(GeocodeAddressesCommand command, GeocodingCompleteEvent completeEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Publishing geocoding complete event. {Event} [{CorrelationId}]", completeEvent, command.JobId);
        await _completeQueue.PublishAsync(completeEvent, cancellationToken);
        _logger.LogInformation("Published geocoding complete event. [{CorrelationId}]", command.JobId);
    }
}
