using Directions.Application.Commands.GenerateDirections;
using Microsoft.Extensions.Logging;
using Moq;

namespace Directions.Application.Tests.CommandHandlers.GenerateDirectionsCommandHandler;

internal class GenerateDirectionsCommandValidatorTestsContext
{
    private readonly Mock<IGenerateDirectionsCommandHandlerMetrics> _mockMetrics;

    internal GenerateDirectionsCommandValidator Sut { get; }

    public GenerateDirectionsCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GenerateDirectionsCommandValidator>>().Object);
    }

    internal GenerateDirectionsCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
