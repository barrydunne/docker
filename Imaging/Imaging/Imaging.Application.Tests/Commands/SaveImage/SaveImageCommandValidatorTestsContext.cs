using Imaging.Application.Commands.SaveImage;
using Microservices.Shared.Mocks;
using NSubstitute;

namespace Imaging.Application.Tests.Commands.SaveImage;

internal class SaveImageCommandValidatorTestsContext
{
    private readonly ISaveImageCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<SaveImageCommandValidator> _mockLogger;

    internal SaveImageCommandValidator Sut { get; }

    public SaveImageCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<ISaveImageCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal SaveImageCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
