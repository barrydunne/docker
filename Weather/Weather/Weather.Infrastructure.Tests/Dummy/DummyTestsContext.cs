using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Weather.Infrastructure.ExternalApi.Dummy;

namespace Weather.Infrastructure.Tests.Dummy;

internal class DummyApiTestsContext
{
    private readonly MockLogger<DummyApi> _mockLogger;

    internal DummyApi Sut { get; }

    public DummyApiTestsContext()
    {
        _mockLogger = new();

        Sut = new(_mockLogger.Object);
    }

    internal DummyApiTestsContext WithWeather(Coordinates coordinates, WeatherForecast weather)
    {
        DummyApi.AddWeather(coordinates, weather);
        return this;
    }
}
