using Microservices.Shared.Events;
using Weather.Logic.Commands;

namespace Weather.Logic.Tests.CommandHandlers.GenerateWeatherCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class GenerateWeatherCommandHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GenerateWeatherCommandHandlerTestsContext _context = new();

        [Test]
        public async Task GenerateWeatherCommandHandler_metrics_increments_count()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_metrics_records_weather_time()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsWeatherTimeRecorded();
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_metrics_records_publish_time()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsPublishTimeRecorded();
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<GenerateWeatherCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<GenerateWeatherCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_guards_return_message_for_missing_coordinates()
        {
            var command = _fixture.Build<GenerateWeatherCommand>()
                                  .With(_ => _.Coordinates, (Coordinates)null!)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Value cannot be null. (Parameter 'Coordinates')"));
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_gets_weather()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertWeatherObtained(command);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_returns_success_when_successful()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_returns_success_when_weather_fails()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            _context.WithInvalidCoordinates(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_returns_success_when_weather_exception()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            _context.WithCoordinatesException(command);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_publishes_weather_complete_event_when_successful()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertWeatherCompleteEventPublished(command);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_publishes_weather_complete_event_when_weather_fails()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            _context.WithInvalidCoordinates(command);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertWeatherCompleteEventPublished(command);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_publishes_weather_complete_event_when_weather_exception()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            _context.WithCoordinatesException(command);
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertWeatherCompleteEventPublished(command);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_returns_failure_on_exception()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task GenerateWeatherCommandHandler_returns_message_on_exception()
        {
            var command = _fixture.Create<GenerateWeatherCommand>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(message));
        }
    }
}
