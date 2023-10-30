using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using State.Logic.Commands;
using State.Repository.Models;

namespace State.Logic.Tests.CommandHandlers.UpdateGeocodingResultCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class UpdateGeocodingResultCommandHandlerTests
    {
        private readonly Fixture _fixture;
        private readonly UpdateGeocodingResultCommandHandlerTestsContext _context;

        public UpdateGeocodingResultCommandHandlerTests()
        {
            _fixture = new();
            _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
            _context = new();
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_metrics_increments_count()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateGeocodingResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_metrics_records_guard_time()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateGeocodingResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_metrics_records_update_time()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateGeocodingResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsUpdateTimeRecorded();
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_metrics_records_publish_time()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = _fixture.Build<UpdateGeocodingResultCommand>().With(_ => _.JobId, job.JobId).Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsPublishTimeRecorded();
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<UpdateGeocodingResultCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<UpdateGeocodingResultCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_guards_fail_for_missing_starting_coordinates()
        {
            var command = _fixture.Build<UpdateGeocodingResultCommand>()
                                  .With(_ => _.StartingCoordinates, (GeocodingCoordinates)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_guards_return_message_for_missing_starting_coordinates()
        {
            var command = _fixture.Build<UpdateGeocodingResultCommand>()
                                  .With(_ => _.StartingCoordinates, (GeocodingCoordinates)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'StartingCoordinates')"));
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_guards_fail_for_missing_destination_coordinates()
        {
            var command = _fixture.Build<UpdateGeocodingResultCommand>()
                                  .With(_ => _.DestinationCoordinates, (GeocodingCoordinates)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_guards_return_message_for_missing_destination()
        {
            var command = _fixture.Build<UpdateGeocodingResultCommand>()
                                  .With(_ => _.DestinationCoordinates, (GeocodingCoordinates)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'DestinationCoordinates')"));
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_returns_failure_for_no_job()
        {
            var command = _fixture.Create<UpdateGeocodingResultCommand>();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Job not yet available for update."));
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_publishes_LocationsReadyEvent_when_successful()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = new UpdateGeocodingResultCommand(job.JobId, _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create(), _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create());
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertLocationsReadyEventPublished(command);
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_sends_NotifyProcessingCompleteCommand_when_unsuccessful()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = new UpdateGeocodingResultCommand(job.JobId, _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, false).Create(), _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create());
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertNotifyProcessingCompleteCommandSent(job.JobId);
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_returns_failure_for_publish_exception()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = new UpdateGeocodingResultCommand(job.JobId, _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create(), _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create());
            _context.WithPublishException();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateGeocodingResultCommandHandler_returns_failure_for_send_exception()
        {
            var job = _fixture.Create<Job>();
            _context.WithJob(job);
            var command = new UpdateGeocodingResultCommand(job.JobId, _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create(), _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, false).Create());
            _context.WithSendException(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }
    }
}
