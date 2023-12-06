using Microservices.Shared.Events;

namespace Weather.Application.ExternalApi;

/// <summary>
/// Generates the weather forecast at a location.
/// </summary>
public interface IExternalApi
{
    /// <summary>
    /// Generates the weather forecast at a location.
    /// </summary>
    /// <param name="coordinates">The location for the weather forecast.</param>
    /// <param name="correlationId">The correlation id to include in all logging.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The coordinates.</returns>
    /// <exception cref="WeatherException">If weather forecast could not be obtained from the external service.</exception>
    Task<WeatherForecast> GetWeatherAsync(Coordinates coordinates, Guid correlationId, CancellationToken cancellationToken = default);
}
