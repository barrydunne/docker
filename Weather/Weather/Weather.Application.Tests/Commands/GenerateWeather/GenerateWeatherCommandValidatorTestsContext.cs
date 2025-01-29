using Microservices.Shared.Mocks;
using Weather.Application.Commands.GenerateWeather;

namespace Weather.Application.Tests.Commands.GenerateWeather;

internal class GenerateWeatherCommandValidatorTestsContext
{
    private readonly IGenerateWeatherCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<GenerateWeatherCommandValidator> _mockLogger;

    internal GenerateWeatherCommandValidator Sut { get; }

    public GenerateWeatherCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGenerateWeatherCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GenerateWeatherCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
