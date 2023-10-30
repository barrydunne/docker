using State.Logic.Commands;
using State.Logic.Tests.Mocks;

namespace State.Logic.Tests.CommandHandlers.CreateJobCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
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
        public async Task CreateJobCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<CreateJobCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
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
        public async Task CreateJobCommandHandler_saves_job()
        {
            var command = _fixture.Create<CreateJobCommand>();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
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
        public async Task CreateJobCommandHandler_returns_failure_on_insert_exception()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.StartingAddress, MockJobRepository.FailingStartingAddress)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_returns_message_on_insert_exception()
        {
            var command = _fixture.Build<CreateJobCommand>()
                                  .With(_ => _.StartingAddress, MockJobRepository.FailingStartingAddress)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(MockJobRepository.FailingStartingAddress));
        }

        [Test]
        public async Task CreateJobCommandHandler_returns_failure_on_send_exception()
        {
            var command = _fixture.Create<CreateJobCommand>();
            _context.WithSendException(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_returns_message_on_send_exception()
        {
            var command = _fixture.Create<CreateJobCommand>();
            _context.WithSendException(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(_context.GetError(command)));
        }

        [Test]
        public async Task CreateJobCommandHandler_returns_failure_on_send_failure()
        {
            var command = _fixture.Create<CreateJobCommand>();
            _context.WithSendFailure(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_returns_message_on_send_failure()
        {
            var command = _fixture.Create<CreateJobCommand>();
            _context.WithSendFailure(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(_context.GetError(command)));
        }
    }
}
