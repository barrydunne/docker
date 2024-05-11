using Microservices.Shared.Mocks;
using NSubstitute;
using State.Application.Commands.NotifyProcessingComplete;

namespace State.Application.Tests.Commands.NotifyProcessingComplete;

internal class NotifyProcessingCompleteCommandValidatorTestsContext
{
    private readonly INotifyProcessingCompleteCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<NotifyProcessingCompleteCommandValidator> _mockLogger;

    internal NotifyProcessingCompleteCommandValidator Sut { get; }

    public NotifyProcessingCompleteCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<INotifyProcessingCompleteCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal NotifyProcessingCompleteCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
