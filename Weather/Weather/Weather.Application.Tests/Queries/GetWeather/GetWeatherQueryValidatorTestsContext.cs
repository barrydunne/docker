using Microservices.Shared.Mocks;
using Weather.Application.Queries.GetWeather;

namespace Weather.Application.Tests.Queries.GetWeather;

internal class GetWeatherQueryValidatorTestsContext
{
    private readonly IGetWeatherQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetWeatherQueryValidator> _mockLogger;

    internal GetWeatherQueryValidator Sut { get; }

    public GetWeatherQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetWeatherQueryHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GetWeatherQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
