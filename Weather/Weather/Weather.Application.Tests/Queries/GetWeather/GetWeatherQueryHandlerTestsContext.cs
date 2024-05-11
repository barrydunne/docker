using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
using Weather.Application.ExternalApi;
using Weather.Application.Queries.GetWeather;

namespace Weather.Application.Tests.Queries.GetWeather;

internal class GetWeatherQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly IExternalApi _mockExternalService;
    private readonly IGetWeatherQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetWeatherQueryHandler> _mockLogger;

    private WeatherForecast? _weather;
    private string? _withExceptionMessage;

    internal GetWeatherQueryHandler Sut { get; }

    public GetWeatherQueryHandlerTestsContext()
    {
        _fixture = new();
        _mockMetrics = Substitute.For<IGetWeatherQueryHandlerMetrics>();
        _mockLogger = new();

        _weather = null;
        _withExceptionMessage = null;

        _mockExternalService = Substitute.For<IExternalApi>();
        _mockExternalService
            .GetWeatherAsync(Arg.Any<Coordinates>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => GetWeather());

        Sut = new(_mockExternalService, _mockMetrics, _mockLogger);
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GetWeatherQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Received(1).RecordExternalTime(Arg.Any<double>());
        return this;
    }
}
