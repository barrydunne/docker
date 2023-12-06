using Microsoft.Extensions.Logging;
using Moq;
using State.Application.Commands.NotifyProcessingComplete;

namespace State.Application.Tests.Commands.NotifyProcessingComplete;

internal class NotifyProcessingCompleteCommandValidatorTestsContext
{
    private readonly Mock<INotifyProcessingCompleteCommandHandlerMetrics> _mockMetrics;

    internal NotifyProcessingCompleteCommandValidator Sut { get; }

    public NotifyProcessingCompleteCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<NotifyProcessingCompleteCommandValidator>>().Object);
    }

    internal NotifyProcessingCompleteCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
