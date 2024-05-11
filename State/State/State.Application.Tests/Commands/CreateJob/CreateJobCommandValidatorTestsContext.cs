using Microservices.Shared.Mocks;
using NSubstitute;
using State.Application.Commands.CreateJob;

namespace State.Application.Tests.Commands.CreateJob;

internal class CreateJobCommandValidatorTestsContext
{
    private readonly ICreateJobCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<CreateJobCommandValidator> _mockLogger;

    internal CreateJobCommandValidator Sut { get; }

    public CreateJobCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<ICreateJobCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal CreateJobCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
