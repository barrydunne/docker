using Microsoft.Extensions.Logging;
using Moq;
using State.Application.Commands.NotifyJobStatusUpdate;

namespace State.Application.Tests.Commands.NotifyJobStatusUpdate;

internal class NotifyJobStatusUpdateCommandValidatorTestsContext
{
    private readonly Mock<INotifyJobStatusUpdateCommandHandlerMetrics> _mockMetrics;

    internal NotifyJobStatusUpdateCommandValidator Sut { get; }

    public NotifyJobStatusUpdateCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<NotifyJobStatusUpdateCommandValidator>>().Object);
    }

    internal NotifyJobStatusUpdateCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
