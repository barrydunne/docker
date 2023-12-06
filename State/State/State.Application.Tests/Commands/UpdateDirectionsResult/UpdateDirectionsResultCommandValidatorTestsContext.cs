using Microsoft.Extensions.Logging;
using Moq;
using State.Application.Commands.UpdateDirectionsResult;

namespace State.Application.Tests.Commands.UpdateDirectionsResult;

internal class UpdateDirectionsResultCommandValidatorTestsContext
{
    private readonly Mock<IUpdateDirectionsResultCommandHandlerMetrics> _mockMetrics;

    internal UpdateDirectionsResultCommandValidator Sut { get; }

    public UpdateDirectionsResultCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<UpdateDirectionsResultCommandValidator>>().Object);
    }

    internal UpdateDirectionsResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
