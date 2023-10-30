using Microservices.Shared.Events;
using PublicApi.Logic.Commands;
using PublicApi.Logic.Tests.Mocks;
using PublicApi.Repository.Models;

namespace PublicApi.Logic.Tests.CommandHandlers.CreateJobCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
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
        public async Task CreateJobCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<CreateJobCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
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
        public async Task CreateJobCommandHandler_guards_fail_for_missing_idempotency_key()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.IdempotencyKey, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_return_message_for_missing_idempotency_key()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.IdempotencyKey, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input IdempotencyKey was empty. (Parameter 'IdempotencyKey')"));
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_fail_for_missing_starting_address()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.StartingAddress, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_return_message_for_missing_starting_address()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.StartingAddress, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input StartingAddress was empty. (Parameter 'StartingAddress')"));
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_fail_for_missing_destination_address()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.DestinationAddress, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_return_message_for_missing_destination_address()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.DestinationAddress, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input DestinationAddress was empty. (Parameter 'DestinationAddress')"));
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_fail_for_missing_email()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.Email, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_return_message_for_missing_email()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.Email, string.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input Email was empty. (Parameter 'Email')"));
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
            Assert.That(result.Value, Is.EqualTo(job.JobId));
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
        public async Task CreateJobCommandHandler_returns_failure_on_exception()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.IdempotencyKey, MockJobRepository.FailingIdempotencyKey)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_returns_message_on_exception()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.IdempotencyKey, MockJobRepository.FailingIdempotencyKey)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(MockJobRepository.FailingIdempotencyKey));
        }
    }
}
