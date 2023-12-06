using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Directions.Application.ExternalApi;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Directions.Application.Queries.GetDirections;

/// <summary>
/// The handler for the <see cref="GetDirectionsQuery"/> query.
/// </summary>
internal class GetDirectionsQueryHandler : IQueryHandler<GetDirectionsQuery, Microservices.Shared.Events.Directions>
{
    private readonly IExternalApi _externalService;
    private readonly IGetDirectionsQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDirectionsQueryHandler"/> class.
    /// </summary>
    /// <param name="externalService">The external service that provides directions of addresses.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetDirectionsQueryHandler(IExternalApi externalService, IGetDirectionsQueryHandlerMetrics metrics, ILogger<GetDirectionsQueryHandler> logger)
    {
        _externalService = externalService;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<Microservices.Shared.Events.Directions>> Handle(GetDirectionsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GetDirectionsQuery), query.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var directions = await _externalService.GetDirectionsAsync(query.StartingCoordinates, query.DestinationCoordinates, query.JobId, cancellationToken);
            _metrics.RecordExternalTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            if (directions.IsSuccessful)
                _logger.LogDebug("Directions: {Distance} km in {Time} seconds. {Steps}. [{CorrelationId}]", directions.DistanceKm, directions.TravelTimeSeconds, directions.Steps!.Select(_ => _.Description), query.JobId);

            return directions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get directions. [{CorrelationId}]", query.JobId);
            return ex;
        }
    }
}
