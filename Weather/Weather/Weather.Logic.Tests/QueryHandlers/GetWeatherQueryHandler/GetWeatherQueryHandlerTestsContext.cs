using Weather.ExternalService;
using Weather.Logic.Metrics;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;

namespace Weather.Logic.Tests.QueryHandlers.GetWeatherQueryHandler
{
    internal class GetWeatherQueryHandlerTestsContext
    {
        private readonly Fixture _fixture;
        private readonly Mock<IExternalService> _mockExternalService;
        private readonly Mock<IGetWeatherQueryHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Weather.Logic.QueryHandlers.GetWeatherQueryHandler> _mockLogger;

        private WeatherForecast? _weather;
        private string? _withExceptionMessage;

        internal Weather.Logic.QueryHandlers.GetWeatherQueryHandler Sut { get; }

        public GetWeatherQueryHandlerTestsContext()
        {
            _fixture = new();
            _mockMetrics = new();
            _mockLogger = new();

            _weather = null;
            _withExceptionMessage = null;

            _mockExternalService = new();
            _mockExternalService.Setup(_ => _.GetWeatherAsync(It.IsAny<Coordinates>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => GetWeather());

            Sut = new(_mockExternalService.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        private WeatherForecast GetWeather()
        {
            if (_withExceptionMessage is not null)
                throw new InvalidOperationException(_withExceptionMessage);
            return _weather ?? CreateWeatherForecast();
        }
        internal WeatherForecast CreateWeatherForecast() => new(true, Enumerable.Range(0, 7).Select(day => new WeatherForecastItem(DateTimeOffset.Now.AddDays(day).ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, _fixture.Create<int>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>())).ToArray(), null);

        internal GetWeatherQueryHandlerTestsContext WithExternalResult(WeatherForecast weather)
        {
            _weather = weather;
            return this;
        }

        internal GetWeatherQueryHandlerTestsContext WithException(string message)
        {
            _withExceptionMessage = message;
            return this;
        }

        internal GetWeatherQueryHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal GetWeatherQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordExternalTime(It.IsAny<double>()), Times.Once);
            return this;
        }
    }
}
