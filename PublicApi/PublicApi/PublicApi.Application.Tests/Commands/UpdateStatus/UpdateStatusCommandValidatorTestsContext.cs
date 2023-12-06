using Microsoft.Extensions.Logging;
using Moq;
using PublicApi.Application.Commands.UpdateStatus;

namespace PublicApi.Application.Tests.Commands.UpdateStatus;

internal class UpdateStatusCommandValidatorTestsContext
{
    private readonly Mock<IUpdateStatusCommandHandlerMetrics> _mockMetrics;

    internal UpdateStatusCommandValidator Sut { get; }

    public UpdateStatusCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<UpdateStatusCommandValidator>>().Object);
    }

    internal UpdateStatusCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
