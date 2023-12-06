using Microsoft.Extensions.Logging;
using Moq;
using State.Application.Commands.UpdateImagingResult;

namespace State.Application.Tests.Commands.UpdateImagingResult;

internal class UpdateImagingResultCommandValidatorTestsContext
{
    private readonly Mock<IUpdateImagingResultCommandHandlerMetrics> _mockMetrics;

    internal UpdateImagingResultCommandValidator Sut { get; }

    public UpdateImagingResultCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<UpdateImagingResultCommandValidator>>().Object);
    }

    internal UpdateImagingResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
