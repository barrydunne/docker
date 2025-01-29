using Microservices.Shared.Events;

namespace Weather.Infrastructure.Tests.Dummy;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class DummyApiTests
{
    private readonly Fixture _fixture = new();
    private readonly DummyApiTestsContext _context = new();

    [Test]
    public async Task Dummy_GetWeatherAsync_returns_known_weather()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var items = Enumerable.Range(0, 7).Select(day => new WeatherForecastItem(DateTimeOffset.Now.AddDays(day).ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, _fixture.Create<int>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>())).ToArray();
        var weather = new WeatherForecast(true, items, null);
        var correlationId = _fixture.Create<Guid>();
        _context.WithWeather(coordinates, weather);
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        result.ShouldBe(weather);
    }

    [Test]
    public async Task Dummy_GetWeatherAsync_returns_random_weather()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var result = await _context.Sut.GetWeatherAsync(coordinates, correlationId);
        (result?.Items?.Length ?? 0).ShouldBeGreaterThan(0);
    }
}
