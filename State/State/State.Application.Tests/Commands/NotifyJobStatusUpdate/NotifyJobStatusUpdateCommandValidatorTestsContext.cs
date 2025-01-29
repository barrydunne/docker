using Microservices.Shared.Mocks;
using State.Application.Commands.NotifyJobStatusUpdate;

namespace State.Application.Tests.Commands.NotifyJobStatusUpdate;

internal class NotifyJobStatusUpdateCommandValidatorTestsContext
{
    private readonly INotifyJobStatusUpdateCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<NotifyJobStatusUpdateCommandValidator> _mockLogger;

    internal NotifyJobStatusUpdateCommandValidator Sut { get; }

    public NotifyJobStatusUpdateCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<INotifyJobStatusUpdateCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal NotifyJobStatusUpdateCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
