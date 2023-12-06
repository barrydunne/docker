using Microsoft.Extensions.Logging;
using Moq;
using Weather.Application.Queries.GetWeather;

namespace Weather.Application.Tests.Queries.GetWeather;

internal class GetWeatherQueryValidatorTestsContext
{
    private readonly Mock<IGetWeatherQueryHandlerMetrics> _mockMetrics;

    internal GetWeatherQueryValidator Sut { get; }

    public GetWeatherQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetWeatherQueryValidator>>().Object);
    }

    internal GetWeatherQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
