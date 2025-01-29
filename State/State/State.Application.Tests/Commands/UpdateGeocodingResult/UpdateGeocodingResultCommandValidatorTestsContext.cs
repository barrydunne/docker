using Microservices.Shared.Mocks;
using State.Application.Commands.UpdateGeocodingResult;

namespace State.Application.Tests.Commands.UpdateGeocodingResult;

internal class UpdateGeocodingResultCommandValidatorTestsContext
{
    private readonly IUpdateGeocodingResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateGeocodingResultCommandValidator> _mockLogger;

    internal UpdateGeocodingResultCommandValidator Sut { get; }

    public UpdateGeocodingResultCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IUpdateGeocodingResultCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal UpdateGeocodingResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
