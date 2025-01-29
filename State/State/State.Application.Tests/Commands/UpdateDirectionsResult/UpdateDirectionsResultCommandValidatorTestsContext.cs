using Microservices.Shared.Mocks;
using State.Application.Commands.UpdateDirectionsResult;

namespace State.Application.Tests.Commands.UpdateDirectionsResult;

internal class UpdateDirectionsResultCommandValidatorTestsContext
{
    private readonly IUpdateDirectionsResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateDirectionsResultCommandValidator> _mockLogger;

    internal UpdateDirectionsResultCommandValidator Sut { get; }

    public UpdateDirectionsResultCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IUpdateDirectionsResultCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal UpdateDirectionsResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
