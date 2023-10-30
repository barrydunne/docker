using Imaging.Logic.Commands;

namespace Imaging.Logic.Tests.CommandHandlers.SaveImageCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class SaveImageCommandHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly SaveImageCommandHandlerTestsContext _context = new();

        [Test]
        public async Task SaveImageCommandHandler_metrics_increments_count()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task SaveImageCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task SaveImageCommandHandler_metrics_records_imaging_time()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsImagingTimeRecorded();
        }

        [Test]
        public async Task SaveImageCommandHandler_metrics_records_download_time()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsDownloadTimeRecorded();
        }

        [Test]
        public async Task SaveImageCommandHandler_metrics_records_upload_time()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsUploadTimeRecorded();
        }

        [Test]
        public async Task SaveImageCommandHandler_metrics_records_publish_time()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsPublishTimeRecorded();
        }

        [Test]
        public async Task SaveImageCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<SaveImageCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SaveImageCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<SaveImageCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task SaveImageCommandHandler_gets_image_url()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertImageUrlObtained(command);
        }

        [Test]
        public async Task SaveImageCommandHandler_saves_image()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertImageSavedToCloud();
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_success_when_successful()
        {
            var command = _fixture.Create<SaveImageCommand>();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_success_when_save_image_fails()
        {
            var command = _fixture.Create<SaveImageCommand>();
            _context.WithInvalidCoordinates(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_success_when_save_image_exception()
        {
            var command = _fixture.Create<SaveImageCommand>();
            _context.WithCoordinatesException(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_failure_when_download_fails()
        {
            var command = _fixture.Create<SaveImageCommand>();
            _context.WithDownloadFailure(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_failure_when_upload_fails()
        {
            var command = _fixture.Create<SaveImageCommand>();
            _context.WithUploadFailure(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task SaveImageCommandHandler_publishes_imaging_complete_event_when_successful()
        {
            var command = _fixture.Create<SaveImageCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertImagingCompleteEventPublished(command);
        }

        [Test]
        public async Task SaveImageCommandHandler_publishes_imaging_complete_event_when_save_image_fails()
        {
            var command = _fixture.Create<SaveImageCommand>();
            _context.WithInvalidCoordinates(command);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertImagingCompleteEventPublished(command);
        }

        [Test]
        public async Task SaveImageCommandHandler_publishes_imaging_complete_event_when_save_image_exception()
        {
            var command = _fixture.Create<SaveImageCommand>();
            _context.WithCoordinatesException(command);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertImagingCompleteEventPublished(command);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_failure_on_exception()
        {
            var command = _fixture.Create<SaveImageCommand>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SaveImageCommandHandler_returns_message_on_exception()
        {
            var command = _fixture.Create<SaveImageCommand>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(message));
        }
    }
}
