using AspNet.KickStarter.CQRS.Abstractions.Queries;
using AspNet.KickStarter.FunctionalResult;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Weather.Application.ExternalApi;

namespace Weather.Application.Queries.GetWeather;

/// <summary>
/// The handler for the <see cref="GetWeatherQuery"/> query.
/// </summary>
internal class GetWeatherQueryHandler : IQueryHandler<GetWeatherQuery, WeatherForecast>
{
    private readonly IExternalApi _externalService;
    private readonly IGetWeatherQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWeatherQueryHandler"/> class.
    /// </summary>
    /// <param name="externalService">The external service that provides weather of addresses.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetWeatherQueryHandler(IExternalApi externalService, IGetWeatherQueryHandlerMetrics metrics, ILogger<GetWeatherQueryHandler> logger)
    {
        _externalService = externalService;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<WeatherForecast>> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GetWeatherQuery), query.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var weather = await _externalService.GetWeatherAsync(query.Coordinates, query.JobId, cancellationToken);
            _metrics.RecordExternalTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            if (weather.IsSuccessful)
                _logger.LogDebug("Weather forecast: {Forecast}. [{CorrelationId}]", weather.Items!.Select(_ => $"{_.LocalTime.ToString("yyyy-MM-dd HH:mm:ss")} {_.Description} {_.MinimumTemperatureC}°C to {_.MaximumTemperatureC}°C {_.PrecipitationProbabilityPercentage}% chance of rain."), query.JobId);

            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get weather. [{CorrelationId}]", query.JobId);
            return ex;
        }
    }
}
