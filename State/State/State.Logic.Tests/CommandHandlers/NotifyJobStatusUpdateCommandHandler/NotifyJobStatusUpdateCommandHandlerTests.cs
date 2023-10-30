﻿using State.Logic.Commands;

namespace State.Logic.Tests.CommandHandlers.NotifyJobStatusUpdateCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
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
        public async Task NotifyJobStatusUpdateCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task NotifyJobStatusUpdateCommandHandler_metrics_records_publish_time()
        {
            var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsPublishTimeRecorded();
        }

        [Test]
        public async Task NotifyJobStatusUpdateCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<NotifyJobStatusUpdateCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task NotifyJobStatusUpdateCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<NotifyJobStatusUpdateCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task SaveImageCommandHandler_publishes_event_when_successful()
        {
            var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertEventPublished(command);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_failure_on_exception()
        {
            var command = _fixture.Create<NotifyJobStatusUpdateCommand>();
            _context.WithPublishException();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }
    }
}