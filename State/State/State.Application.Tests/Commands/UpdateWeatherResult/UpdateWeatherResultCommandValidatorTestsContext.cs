using Microservices.Shared.Mocks;
using State.Application.Commands.UpdateWeatherResult;

namespace State.Application.Tests.Commands.UpdateWeatherResult;

internal class UpdateWeatherResultCommandValidatorTestsContext
{
    private readonly IUpdateWeatherResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateWeatherResultCommandValidator> _mockLogger;

    internal UpdateWeatherResultCommandValidator Sut { get; }

    public UpdateWeatherResultCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IUpdateWeatherResultCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal UpdateWeatherResultCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
