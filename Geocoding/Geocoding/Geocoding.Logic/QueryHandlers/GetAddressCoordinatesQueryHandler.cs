using AspNet.KickStarter.CQRS.Abstractions.Queries;
using CSharpFunctionalExtensions;
using Geocoding.ExternalService;
using Geocoding.Logic.Caching;
using Geocoding.Logic.Metrics;
using Geocoding.Logic.Queries;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Geocoding.Logic.QueryHandlers
{
    /// <summary>
    /// The handler for the <see cref="GetAddressCoordinatesQuery"/> command.
    /// </summary>
    internal class GetAddressCoordinatesQueryHandler : IQueryHandler<GetAddressCoordinatesQuery, Result<Coordinates>>
    {
        private readonly IExternalService _externalService;
        private readonly IGeocodingCache _geocodingCache;
        private readonly IGetAddressCoordinatesQueryHandlerMetrics _metrics;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAddressCoordinatesQueryHandler"/> class.
        /// </summary>
        /// <param name="externalService">The external service that provides geocoding of addresses.</param>
        /// <param name="geocodingCache">The cache to save and retrieve coordinates from in preference to using the external service.</param>
        /// <param name="metrics">The metrics provider for this handler.</param>
        /// <param name="logger">The logger to write to.</param>
        public GetAddressCoordinatesQueryHandler(IExternalService externalService, IGeocodingCache geocodingCache, IGetAddressCoordinatesQueryHandlerMetrics metrics, ILogger<GetAddressCoordinatesQueryHandler> logger)
        {
            _externalService = externalService;
            _geocodingCache = geocodingCache;
            _metrics = metrics;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Result<Coordinates>> Handle(GetAddressCoordinatesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GetAddressCoordinatesQuery), query.JobId);
            _metrics.IncrementCount();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var coordinates = await _geocodingCache.GetAsync(query.Address, cancellationToken);
                if (coordinates is null)
                {
                    coordinates = await _externalService.GetCoordinatesAsync(query.Address, query.JobId, cancellationToken);
                    await _geocodingCache.SetAsync(query.Address, coordinates, TimeSpan.FromDays(7), cancellationToken);
                    _metrics.RecordExternalTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
                }
                _logger.LogDebug("Coordinates for {Address} are {Latitude},{Longitude}. [{CorrelationId}]", query.Address, coordinates.Latitude, coordinates.Longitude, query.JobId);
                return Result.Success(coordinates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to geocode address. [{CorrelationId}]", query.JobId);
                return Result.Failure<Coordinates>(ex.Message);
            }
        }
    }
}
