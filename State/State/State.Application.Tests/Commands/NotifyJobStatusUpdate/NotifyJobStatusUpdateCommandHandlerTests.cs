﻿using State.Application.Commands.NotifyJobStatusUpdate;

namespace State.Application.Tests.Commands.NotifyJobStatusUpdate;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class NotifyJobStatusUpdateCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly NotifyJobStatusUpdateCommandHandlerTestsContext _context = new();

    [Test]
    public async Task NotifyJobStatusUpdateCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task NotifyJobStatusUpdateCommandHandler_metrics_records_publish_time()
    {
        var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task SaveImageCommandHandler_publishes_event_when_successful()
    {
        var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertEventPublished(command);
    }

    [Test]
    public async Task SaveImageCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
        _context.WithPublishException();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }
}
