using Microservices.Shared.Events;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Models;
using PublicApi.Application.Tests.Mocks;

namespace PublicApi.Application.Tests.Commands.CreateJob;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class CreateJobCommandHandlerTests
{
    private readonly PublicApiFixture _fixture = new();
    private readonly CreateJobCommandHandlerTestsContext _context = new();

    [Test]
    public async Task CreateJobCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task CreateJobCommandHandler_metrics_records_idempotency_time()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsIdempotencyTimeRecorded();
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
    public async Task CreateJobCommandHandler_returns_same_job_id_for_known_idempotency_key()
    {
        var job = _fixture.Create<Job>();
        _context.WithExistingJob(job);

        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.IdempotencyKey, job.IdempotencyKey)
                              .Create();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Value.ShouldBe(job.JobId);
    }

    [Test]
    public async Task CreateJobCommandHandler_saves_job()
    {
        var command = _fixture.Create<CreateJobCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertJobSaved(result.Value);
    }

    [Test]
    public async Task CreateJobCommandHandler_saves_job_with_idempotency_key()
    {
        var command = _fixture.Create<CreateJobCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertJobIdempotencyKey(result.Value, command.IdempotencyKey);
    }

    [Test]
    public async Task CreateJobCommandHandler_saves_job_with_accepted_status()
    {
        var command = _fixture.Create<CreateJobCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertJobStatus(result.Value, JobStatus.Accepted);
    }

    [Test]
    public async Task CreateJobCommandHandler_publishes_job_created_event()
    {
        var command = _fixture.Create<CreateJobCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertJobCreatedEventPublished(result.Value, command);
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.IdempotencyKey, MockJobRepository.FailingIdempotencyKey)
                              .Create();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_message_on_exception()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.IdempotencyKey, MockJobRepository.FailingIdempotencyKey)
                              .Create();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Error!.Value.Message.ShouldBe(MockJobRepository.FailingIdempotencyKey);
    }
}
