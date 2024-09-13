using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microservices.Shared.Events;
using Microsoft.Extensions.Logging;
using Weather.Application.ExternalApi;

namespace Weather.Infrastructure.ExternalApi.Dummy;

/// <summary>
/// Uses dummy weather.
/// </summary>
public class DummyApi : IExternalApi
{
    private static readonly int[] _wmoCodes = new[] { 0, 1, 2, 3, 45, 48, 51, 53, 55, 56, 57, 61, 63, 65, 66, 67, 71, 73, 75, 77, 80, 81, 82, 85, 86, 95, 96, 99 };
    private static readonly Dictionary<int, (string Description, string Image)> _weatherInfo = new()
    {
        [0] = ("Sunny", "http://openweathermap.org/img/wn/01d@2x.png"),
        [1] = ("Mainly Sunny", "http://openweathermap.org/img/wn/01d@2x.png"),
        [2] = ("Partly Cloudy", "http://openweathermap.org/img/wn/02d@2x.png"),
        [3] = ("Cloudy", "http://openweathermap.org/img/wn/03d@2x.png"),
        [45] = ("Foggy", "http://openweathermap.org/img/wn/50d@2x.png"),
        [48] = ("Rime Fog", "http://openweathermap.org/img/wn/50d@2x.png"),
        [51] = ("Light Drizzle", "http://openweathermap.org/img/wn/09d@2x.png"),
        [53] = ("Drizzle", "http://openweathermap.org/img/wn/09d@2x.png"),
        [55] = ("Heavy Drizzle", "http://openweathermap.org/img/wn/09d@2x.png"),
        [56] = ("Light Freezing Drizzle", "http://openweathermap.org/img/wn/09d@2x.png"),
        [57] = ("Freezing Drizzle", "http://openweathermap.org/img/wn/09d@2x.png"),
        [61] = ("Light Rain", "http://openweathermap.org/img/wn/10d@2x.png"),
        [63] = ("Rain", "http://openweathermap.org/img/wn/10d@2x.png"),
        [65] = ("Heavy Rain", "http://openweathermap.org/img/wn/10d@2x.png"),
        [66] = ("Light Freezing Rain", "http://openweathermap.org/img/wn/10d@2x.png"),
        [67] = ("Freezing Rain", "http://openweathermap.org/img/wn/10d@2x.png"),
        [71] = ("Light Snow", "http://openweathermap.org/img/wn/13d@2x.png"),
        [73] = ("Snow", "http://openweathermap.org/img/wn/13d@2x.png"),
        [75] = ("Heavy Snow", "http://openweathermap.org/img/wn/13d@2x.png"),
        [77] = ("Snow Grains", "http://openweathermap.org/img/wn/13d@2x.png"),
        [80] = ("Light Showers", "http://openweathermap.org/img/wn/09d@2x.png"),
        [81] = ("Showers", "http://openweathermap.org/img/wn/09d@2x.png"),
        [82] = ("Heavy Showers", "http://openweathermap.org/img/wn/09d@2x.png"),
        [85] = ("Light Snow Showers", "http://openweathermap.org/img/wn/13d@2x.png"),
        [86] = ("Snow Showers", "http://openweathermap.org/img/wn/13d@2x.png"),
        [95] = ("Thunderstorm", "http://openweathermap.org/img/wn/11d@2x.png"),
        [96] = ("Light Thunderstorms With Hail", "http://openweathermap.org/img/wn/11d@2x.png"),
        [99] = ("Thunderstorm With Hail", "http://openweathermap.org/img/wn/11d@2x.png")
    };

    private readonly ILogger _logger;

    private static readonly ConcurrentDictionary<Coordinates, WeatherForecast> _knownWeather = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyApi"/> class.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    public DummyApi(ILogger<DummyApi> logger) => _logger = logger;

    /// <summary>
    /// Add some fixed weather for specific coordinates.
    /// </summary>
    /// <param name="coordinates">The destination location.</param>
    /// <param name="weather">The known weather forecast to return for these coordinates.</param>
    public static void AddWeather(Coordinates coordinates, WeatherForecast weather) => _knownWeather[coordinates] = weather;

    /// <inheritdoc/>
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Dummy data")]
    public Task<WeatherForecast> GetWeatherAsync(Coordinates coordinates, Guid correlationId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Returning weather. [{CorrelationId}]", correlationId);
        if (!_knownWeather.TryGetValue(coordinates, out var weather))
            weather = new WeatherForecast(true, GetWeatherForecastItems(), null);
        return Task.FromResult(weather);
    }

    private static WeatherForecastItem[] GetWeatherForecastItems()
        => Enumerable.Range(0, 7).Select(day => GetWeatherForecastItem(DateTimeOffset.Now.AddDays(day))).ToArray();

    private static WeatherForecastItem GetWeatherForecastItem(DateTimeOffset dateTimeOffset)
    {
        var wmoCode = _wmoCodes[Random.Shared.Next(0, _wmoCodes.Length)];
        var maxC = Random.Shared.Next(0, 400) / 10.0;
        var minC = maxC * 0.7;
        var percent = Random.Shared.Next(0, 101);
        return new(dateTimeOffset.ToUnixTimeSeconds(), (int)dateTimeOffset.Offset.TotalSeconds, wmoCode, _weatherInfo[wmoCode].Description, _weatherInfo[wmoCode].Image, minC, maxC, percent);
    }
}
