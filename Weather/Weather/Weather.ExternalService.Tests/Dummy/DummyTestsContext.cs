using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Weather.ExternalService.Tests.Dummy
{
    internal class DummyTestsContext
    {
        private readonly MockLogger<Weather.ExternalService.Dummy> _mockLogger;

        internal Weather.ExternalService.Dummy Sut { get; }

        public DummyTestsContext()
        {
            _mockLogger = new();

            Sut = new(_mockLogger.Object);
        }

        internal DummyTestsContext WithWeather(Coordinates coordinates, WeatherForecast weather)
        {
            Weather.ExternalService.Dummy.AddWeather(coordinates, weather);
            return this;
        }
    }
}
