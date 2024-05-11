using Microservices.Shared.Mocks;
using NSubstitute;
using State.Application.Commands.UpdateImagingResult;

namespace State.Application.Tests.Commands.UpdateImagingResult;

internal class UpdateImagingResultCommandValidatorTestsContext
{
    private readonly IUpdateImagingResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateImagingResultCommandValidator> _mockLogger;

    internal UpdateImagingResultCommandValidator Sut { get; }

    public UpdateImagingResultCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IUpdateImagingResultCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal UpdateImagingResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
