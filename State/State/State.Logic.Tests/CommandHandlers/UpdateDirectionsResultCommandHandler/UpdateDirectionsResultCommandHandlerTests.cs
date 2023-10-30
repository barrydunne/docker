using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using State.Logic.Commands;
using State.Repository.Models;

namespace State.Logic.Tests.CommandHandlers.UpdateDirectionsResultCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class UpdateDirectionsResultCommandHandlerTests
    {
        private readonly Fixture _fixture;
        private readonly UpdateDirectionsResultCommandHandlerTestsContext _context;

        public UpdateDirectionsResultCommandHandlerTests()
        {
            _fixture = new();
            _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
            _context = new();
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_metrics_increments_count()
        {
            var command = _fixture.Create<UpdateDirectionsResultCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<UpdateDirectionsResultCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_metrics_records_publish_time_for_complete_job()
        {
            var job = _fixture.Build<Job>()
                              .With(_ => _.DirectionsSuccessful, true)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateDirectionsResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsPublishTimeRecorded();
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_metrics_records_publish_time_for_incomplete_job()
        {
            var job = _fixture.Build<Job>()
                              .With(_ => _.DirectionsSuccessful, true)
                              .Without(_ => _.WeatherSuccessful)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateDirectionsResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsPublishTimeNotRecorded();
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<UpdateDirectionsResultCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<UpdateDirectionsResultCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_guards_fail_for_missing_directions()
        {
            var command = _fixture.Build<UpdateDirectionsResultCommand>()
                                  .With(_ => _.Directions, (Directions)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_guards_return_message_for_missing_directions()
        {
            var command = _fixture.Build<UpdateDirectionsResultCommand>()
                                  .With(_ => _.Directions, (Directions)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'Directions')"));
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_updates_successful()
        {
            var job = _fixture.Build<Job>()
                              .Without(_ => _.DirectionsSuccessful)
                              .Without(_ => _.Directions)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateDirectionsResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            job = _context.GetJob(job.JobId);
            Assert.That(job?.DirectionsSuccessful, Is.EqualTo(command.Directions.IsSuccessful));
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_updates_result()
        {
            var job = _fixture.Build<Job>()
                              .Without(_ => _.DirectionsSuccessful)
                              .Without(_ => _.Directions)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateDirectionsResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            job = _context.GetJob(job.JobId);
            Assert.That(job?.Directions, Is.EqualTo(command.Directions));
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_returns_failure_for_repository_exception()
        {
            _context.WithJobRepositoryException();
            var command = _fixture.Create<UpdateDirectionsResultCommand>();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_returns_failure_for_send_exception()
        {
            var job = _fixture.Build<Job>()
                  .With(_ => _.DirectionsSuccessful, true)
                  .With(_ => _.WeatherSuccessful, true)
                  .With(_ => _.ImagingSuccessful, true)
                  .Create();
            _context.WithJob(job);

            var command = _fixture.Build<UpdateDirectionsResultCommand>().With(_ => _.JobId, job.JobId).Create();
            _context.WithSendException(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_returns_failure_for_send_failure()
        {
            var job = _fixture.Build<Job>()
                              .With(_ => _.DirectionsSuccessful, true)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);

            var command = _fixture.Build<UpdateDirectionsResultCommand>().With(_ => _.JobId, job.JobId).Create();
            _context.WithSendFailure(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_sends_NotifyJobStatusUpdateCommand_on_success()
        {
            var job = _fixture.Build<Job>()
                              .With(_ => _.DirectionsSuccessful, true)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);

            var command = new UpdateDirectionsResultCommand(job.JobId, job.Directions!);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertNotifyJobStatusUpdateCommandSent(job.JobId, false);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_sends_NotifyJobStatusUpdateCommand_on_partial_success()
        {
            var job = _fixture.Build<Job>()
                              .With(_ => _.DirectionsSuccessful, true)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);

            var error = _fixture.Create<string>();
            var command = new UpdateDirectionsResultCommand(job.JobId, new(false, null, null, null, error));
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertNotifyJobStatusUpdateCommandSent(job.JobId, true);
        }

        [Test]
        public async Task UpdateDirectionsResultCommandHandler_sends_NotifyProcessingCompleteCommand()
        {
            var job = _fixture.Build<Job>()
                              .With(_ => _.DirectionsSuccessful, true)
                              .With(_ => _.WeatherSuccessful, true)
                              .With(_ => _.ImagingSuccessful, true)
                              .Create();
            _context.WithJob(job);

            var command = new UpdateDirectionsResultCommand(job.JobId, job.Directions!);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertNotifyProcessingCompleteCommandSent(job.JobId);
        }
    }
}
