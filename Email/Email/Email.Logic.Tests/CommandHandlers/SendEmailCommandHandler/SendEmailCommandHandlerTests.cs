using Email.Logic.Commands;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Email.Logic.Tests.CommandHandlers.SendEmailCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class SendEmailCommandHandlerTests
    {
        private readonly Fixture _fixture;
        private readonly SendEmailCommandHandlerTestsContext _context;

        public SendEmailCommandHandlerTests()
        {
            _fixture = new();
            _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
            _context = new();
        }

        [Test]
        public async Task SendEmailCommandHandler_metrics_increments_count()
        {
            var command = _fixture.Create<SendEmailCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task SendEmailCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<SendEmailCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task SendEmailCommandHandler_metrics_records_image_time()
        {
            var command = _fixture.Create<SendEmailCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsImageTimeRecorded();
        }

        [Test]
        public async Task SendEmailCommandHandler_metrics_records_generate_time()
        {
            var command = _fixture.Create<SendEmailCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGenerateTimeRecorded();
        }

        [Test]
        public async Task SendEmailCommandHandler_metrics_records_email_time()
        {
            var command = _fixture.Create<SendEmailCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsEmailTimeRecorded();
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_fail_for_missing_directions()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Directions, (Directions)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_return_message_for_missing_directions()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Directions, (Directions)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'Directions')"));
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_fail_for_missing_weather()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Weather, (WeatherForecast)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_return_message_for_missing_weather()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Weather, (WeatherForecast)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'Weather')"));
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_fail_for_missing_imaging()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Imaging, (ImagingResult)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SendEmailCommandHandler_guards_return_message_for_missing_imaging()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Imaging, (ImagingResult)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'Imaging')"));
        }

        [Test]
        public async Task SendEmailCommandHandler_generates_html_without_image_if_unavailable()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                                  .Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertHtmlGeneratedWithoutImage();
        }

        [Test]
        public async Task SendEmailCommandHandler_generates_html_with_image_if_available()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                                  .Create();
            _context.WithImage(command.Imaging!.ImagePath!);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertHtmlGeneratedWithImage();
        }

        [Test]
        public async Task SendEmailCommandHandler_sends_email_without_image_if_unavailable()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                                  .Create();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertEmailSentWithoutImage();
        }

        [Test]
        public async Task SendEmailCommandHandler_sends_email_with_image_if_available()
        {
            var command = _fixture.Build<SendEmailCommand>()
                                  .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                                  .Create();
            _context.WithImage(command.Imaging!.ImagePath!);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertEmailSentWithImage();
        }

        [Test]
        public async Task SendEmailCommandHandler_returns_failure_if_fail_to_send_email()
        {
            var command = _fixture.Create<SendEmailCommand>();
            _context.WithSendFailure();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task SendEmailCommandHandler_returns_failure_on_exception()
        {
            var command = _fixture.Create<SendEmailCommand>();
            _context.WithSendException();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }
    }
}
