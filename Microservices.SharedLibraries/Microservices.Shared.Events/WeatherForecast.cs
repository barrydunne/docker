namespace Microservices.Shared.Events
{
    /// <summary>
    /// The weather forecast at a location.
    /// </summary>
    /// <param name="IsSuccessful">Whether the forecast was obtained successfully.</param>
    /// <param name="Items">The individual timestamped forecast items.</param>
    /// <param name="Error">The error if not successful.</param>
    public record WeatherForecast(bool IsSuccessful, WeatherForecastItem[]? Items, string? Error);
}
