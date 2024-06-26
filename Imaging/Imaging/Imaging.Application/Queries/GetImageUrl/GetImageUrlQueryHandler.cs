﻿using AspNet.KickStarter.CQRS.Abstractions.Queries;
using AspNet.KickStarter.FunctionalResult;
using Imaging.Application.ExternalApi;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Imaging.Application.Queries.GetImageUrl;

/// <summary>
/// The handler for the <see cref="GetImageUrlQuery"/> query.
/// </summary>
internal class GetImageUrlQueryHandler : IQueryHandler<GetImageUrlQuery, string?>
{
    private readonly IExternalApi _externalService;
    private readonly IGetImageUrlQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetImageUrlQueryHandler"/> class.
    /// </summary>
    /// <param name="externalService">The external service that provides images of addresses.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetImageUrlQueryHandler(IExternalApi externalService, IGetImageUrlQueryHandlerMetrics metrics, ILogger<GetImageUrlQueryHandler> logger)
    {
        _externalService = externalService;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<string?>> Handle(GetImageUrlQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GetImageUrlQuery), query.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var imageUrl = await _externalService.GetImageUrlAsync(query.Address, query.Coordinates, query.JobId, cancellationToken);
            _metrics.RecordExternalTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            if (!string.IsNullOrWhiteSpace(imageUrl))
                _logger.LogDebug("Image URL: {URL}. [{CorrelationId}]", imageUrl, query.JobId);

            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get image URL. [{CorrelationId}]", query.JobId);
            return ex;
        }
    }
}
