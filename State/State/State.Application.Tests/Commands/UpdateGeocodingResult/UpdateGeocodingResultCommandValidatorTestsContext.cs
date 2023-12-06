using Microsoft.Extensions.Logging;
using Moq;
using State.Application.Commands.UpdateGeocodingResult;

namespace State.Application.Tests.Commands.UpdateGeocodingResult;

internal class UpdateGeocodingResultCommandValidatorTestsContext
{
    private readonly Mock<IUpdateGeocodingResultCommandHandlerMetrics> _mockMetrics;

    internal UpdateGeocodingResultCommandValidator Sut { get; }

    public UpdateGeocodingResultCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<UpdateGeocodingResultCommandValidator>>().Object);
    }

    internal UpdateGeocodingResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
