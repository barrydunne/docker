using Microservices.Shared.Mocks;
using State.Application.Commands.NotifyProcessingComplete;

namespace State.Application.Tests.Commands.NotifyProcessingComplete;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class NotifyProcessingCompleteCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly NotifyProcessingCompleteCommandHandlerTestsContext _context;

    public NotifyProcessingCompleteCommandHandlerTests()
    {
        _fixture = new();
        _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
        _context = new();
    }

    [Test]
    public async Task NotifyProcessingCompleteCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<NotifyProcessingCompleteCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task NotifyProcessingCompleteCommandHandler_metrics_records_publish_time()
    {
        var command = _fixture.Create<NotifyProcessingCompleteCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task SaveImageCommandHandler_publishes_event_when_successful()
    {
        var command = _fixture.Create<NotifyProcessingCompleteCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertEventPublished(command);
    }

    [Test]
    public async Task SaveImageCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Create<NotifyProcessingCompleteCommand>();
        _context.WithPublishException();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }
}
