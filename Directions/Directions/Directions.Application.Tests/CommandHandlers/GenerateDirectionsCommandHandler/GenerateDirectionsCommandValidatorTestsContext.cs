using Directions.Application.Commands.GenerateDirections;
using Microservices.Shared.Mocks;

namespace Directions.Application.Tests.CommandHandlers.GenerateDirectionsCommandHandler;

internal class GenerateDirectionsCommandValidatorTestsContext
{
    private readonly IGenerateDirectionsCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<GenerateDirectionsCommandValidator> _mockLogger;

    internal GenerateDirectionsCommandValidator Sut { get; }

    public GenerateDirectionsCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGenerateDirectionsCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GenerateDirectionsCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
