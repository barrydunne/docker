using Microsoft.Extensions.Logging;
using Moq;
using Weather.Application.Commands.GenerateWeather;

namespace Weather.Application.Tests.Commands.GenerateWeather;

internal class GenerateWeatherCommandValidatorTestsContext
{
    private readonly Mock<IGenerateWeatherCommandHandlerMetrics> _mockMetrics;

    internal GenerateWeatherCommandValidator Sut { get; }

    public GenerateWeatherCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GenerateWeatherCommandValidator>>().Object);
    }

    internal GenerateWeatherCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
