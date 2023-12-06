namespace Microservices.Shared.Events;

/// <summary>
/// A weather forecast at a single point in time.
/// </summary>
/// <param name="ForecastTimeUnixSeconds">The time in unix format. The number of seconds since 1 January 1970.</param>
/// <param name="LocalTimeOffsetSeconds">The local seconds offset from GMT+0.</param>
/// <param name="WeatherCode">The WMO weather code indicating the most severe weather condition.</param>
/// <param name="Description">The description of the most severe weather condition.</param>
/// <param name="ImageUrl">The url for an image representing the weather.</param>
/// <param name="MinimumTemperatureC">The minimum temperature in Celsius.</param>
/// <param name="MaximumTemperatureC">The maximum temperature in Celsius.</param>
/// <param name="PrecipitationProbabilityPercentage">The percentage chance of precipitation (0-100).</param>
public record WeatherForecastItem(long ForecastTimeUnixSeconds, int LocalTimeOffsetSeconds, int WeatherCode, string Description, string? ImageUrl, double MinimumTemperatureC, double MaximumTemperatureC, int PrecipitationProbabilityPercentage)
{
    /// <summary>
    /// Gets the local time for this forecast.
    /// </summary>
    public DateTimeOffset LocalTime { get; init; } = DateTimeOffset.FromUnixTimeSeconds(ForecastTimeUnixSeconds).ToOffset(TimeSpan.FromSeconds(LocalTimeOffsetSeconds));
}
