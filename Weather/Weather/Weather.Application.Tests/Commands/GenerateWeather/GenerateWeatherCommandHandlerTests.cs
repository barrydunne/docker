using Weather.Application.Commands.GenerateWeather;

namespace Weather.Application.Tests.Commands.GenerateWeather;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class GenerateWeatherCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GenerateWeatherCommandHandlerTestsContext _context = new();

    [Test]
    public async Task GenerateWeatherCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_metrics_records_weather_time()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsWeatherTimeRecorded();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_metrics_records_publish_time()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_gets_weather()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertWeatherObtained(command);
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_returns_success_when_successful()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_returns_success_when_weather_fails()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        _context.WithInvalidCoordinates(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_returns_success_when_weather_exception()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        _context.WithCoordinatesException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_publishes_weather_complete_event_when_successful()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertWeatherCompleteEventPublished(command);
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_publishes_weather_complete_event_when_weather_fails()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        _context.WithInvalidCoordinates(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertWeatherCompleteEventPublished(command);
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_publishes_weather_complete_event_when_weather_exception()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        _context.WithCoordinatesException(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertWeatherCompleteEventPublished(command);
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task GenerateWeatherCommandHandler_returns_message_on_exception()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Error?.Message.ShouldBe(message);
    }
}
