using Directions.Application.Commands.GenerateDirections;

namespace Directions.Application.Tests.CommandHandlers.GenerateDirectionsCommandHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class GenerateDirectionsCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GenerateDirectionsCommandHandlerTestsContext _context = new();

    [Test]
    public async Task GenerateDirectionsCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_metrics_records_directions_time()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsDirectionsTimeRecorded();
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_metrics_records_publish_time()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_gets_directions()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertDirectionsObtained(command);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_returns_success_when_successful()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_returns_success_when_directions_fails()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        _context.WithInvalidCoordinates(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_returns_success_when_directions_exception()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        _context.WithCoordinatesException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_publishes_directions_complete_event_when_successful()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertDirectionsCompleteEventPublished(command);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_publishes_directions_complete_event_when_directions_fails()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        _context.WithInvalidCoordinates(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertDirectionsCompleteEventPublished(command);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_publishes_directions_complete_event_when_directions_exception()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        _context.WithCoordinatesException(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertDirectionsCompleteEventPublished(command);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task GenerateDirectionsCommandHandler_returns_message_on_exception()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.Error?.Message, Is.EqualTo(message));
    }
}
