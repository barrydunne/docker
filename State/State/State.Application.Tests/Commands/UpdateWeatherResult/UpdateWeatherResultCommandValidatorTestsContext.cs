using Microsoft.Extensions.Logging;
using Moq;
using State.Application.Commands.UpdateWeatherResult;

namespace State.Application.Tests.Commands.UpdateWeatherResult;

internal class UpdateWeatherResultCommandValidatorTestsContext
{
    private readonly Mock<IUpdateWeatherResultCommandHandlerMetrics> _mockMetrics;

    internal UpdateWeatherResultCommandValidator Sut { get; }

    public UpdateWeatherResultCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<UpdateWeatherResultCommandValidator>>().Object);
    }

    internal UpdateWeatherResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
