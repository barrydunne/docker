using Microservices.Shared.Events;
using System.Text.Json;
using Weather.Application;

namespace Weather.Infrastructure.Tests.OpenMeteo;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class OpenMeteoTests
{
    private readonly Fixture _fixture = new();
    private readonly OpenMeteoTestsContext _context = new();

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_weather()
    {
        var coordinates = _fixture.Create<Coordinates>();

        var weatherCodes = new[] { 0, 1, 2, 3, 45, 48, 51 };
        var weatherDescriptions = new[] { "Sunny", "Mainly Sunny", "Partly Cloudy", "Cloudy", "Foggy", "Rime Fog", "Light Drizzle" };
        var weatherImages = new[] { "http://openweathermap.org/img/wn/01d@2x.png", "http://openweathermap.org/img/wn/01d@2x.png", "http://openweathermap.org/img/wn/02d@2x.png", "http://openweathermap.org/img/wn/03d@2x.png", "http://openweathermap.org/img/wn/50d@2x.png", "http://openweathermap.org/img/wn/50d@2x.png", "http://openweathermap.org/img/wn/09d@2x.png" };

        var items = Enumerable.Range(0, 7).Select(day => new WeatherForecastItem(DateTimeOffset.Now.AddDays(day).ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, weatherCodes[day], weatherDescriptions[day], weatherImages[day], _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>())).ToArray();

        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context.WithWeather(coordinates, weather);
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        // Use JSON to compare steps collections
        Assert.That(JsonSerializer.Serialize(result), Is.EqualTo(JsonSerializer.Serialize(weather)));
    }

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_clear_description_for_unknown_wmo_code()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = new[] { new WeatherForecastItem(DateTimeOffset.Now.ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, int.MaxValue, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>()) };
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context.WithWeather(coordinates, weather);
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        Assert.That(result.Items![0].Description, Is.EqualTo("Clear"));
    }

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_null_image_url_for_unknown_wmo_code()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = new[] { new WeatherForecastItem(DateTimeOffset.Now.ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, int.MaxValue, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>()) };
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context.WithWeather(coordinates, weather);
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        Assert.That(result.Items![0].ImageUrl, Is.Null);
    }

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_clear_description_for_missing_file()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = new[] { new WeatherForecastItem(DateTimeOffset.Now.ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, 1, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>()) };
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context
            .WithWeather(coordinates, weather)
            .WithMissingJson();
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        Assert.That(result.Items![0].Description, Is.EqualTo("Clear"));
    }

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_null_image_url_for_missing_file()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = new[] { new WeatherForecastItem(DateTimeOffset.Now.ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, 1, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>()) };
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context
            .WithWeather(coordinates, weather)
            .WithMissingJson();
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        Assert.That(result.Items![0].ImageUrl, Is.Null);
    }

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_clear_description_for_invalid_file()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = new[] { new WeatherForecastItem(DateTimeOffset.Now.ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, 1, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>()) };
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context
            .WithWeather(coordinates, weather)
            .WithInvalidJson();
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        Assert.That(result.Items![0].Description, Is.EqualTo("Clear"));
    }

    [Test]
    public async Task MapQuest_GetWeatherAsync_returns_null_image_url_for_invalid_file()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = new[] { new WeatherForecastItem(DateTimeOffset.Now.ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, 1, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>()) };
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context
            .WithWeather(coordinates, weather)
            .WithInvalidJson();
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        Assert.That(result.Items![0].ImageUrl, Is.Null);
    }

    [Test]
    public void MapQuest_GetWeatherAsync_throws_WeatherException_for_bad_request()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithBadRequest();
        Assert.That(async () => await _context.Sut.GetWeatherAsync(coordinates, correlationId),
            Throws.TypeOf<WeatherException>().With.Property("Message").EqualTo("No weather forecast obtained from OpenMeteo."));
    }

    [Test]
    public void MapQuest_GetWeatherAsync_throws_WeatherException_for_no_result()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoResult();
        Assert.That(async () => await _context.Sut.GetWeatherAsync(coordinates, correlationId),
            Throws.TypeOf<WeatherException>().With.Property("Message").EqualTo("No weather forecast obtained from OpenMeteo."));
    }

    [Test]
    public void MapQuest_GetWeatherAsync_throws_for_exception()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        Assert.That(async () => await _context.Sut.GetWeatherAsync(coordinates, correlationId),
            Throws.TypeOf<WeatherException>().With.Property("Message").EqualTo(message));
    }
}
