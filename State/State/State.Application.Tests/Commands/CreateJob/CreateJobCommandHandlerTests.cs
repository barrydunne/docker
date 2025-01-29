using State.Application.Commands.CreateJob;
using State.Application.Tests.Mocks;

namespace State.Application.Tests.Commands.CreateJob;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class CreateJobCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly CreateJobCommandHandlerTestsContext _context = new();

    [Test]
    public async Task CreateJobCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task CreateJobCommandHandler_metrics_records_save_time()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsSaveTimeRecorded();
    }

    [Test]
    public async Task CreateJobCommandHandler_metrics_records_publish_time()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task CreateJobCommandHandler_saves_job()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertJobSaved(command.JobId);
    }

    [Test]
    public async Task CreateJobCommandHandler_sends_notify_job_status_update_command()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertNotifyJobStatusUpdateCommandSent(command.JobId);
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_error_on_insert_exception()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.StartingAddress, MockJobRepository.FailingStartingAddress)
                              .Create();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_message_on_insert_exception()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.StartingAddress, MockJobRepository.FailingStartingAddress)
                              .Create();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Error?.Message.ShouldBe(MockJobRepository.FailingStartingAddress);
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_error_on_send_exception()
    {
        var command = _fixture.Create<CreateJobCommand>();
        _context.WithSendException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_message_on_send_exception()
    {
        var command = _fixture.Create<CreateJobCommand>();
        _context.WithSendException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Error?.Message.ShouldBe(_context.GetError(command));
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_error_on_send_failure()
    {
        var command = _fixture.Create<CreateJobCommand>();
        _context.WithSendFailure(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_message_on_send_failure()
    {
        var command = _fixture.Create<CreateJobCommand>();
        _context.WithSendFailure(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Error?.Message.ShouldBe(_context.GetError(command));
    }
}
