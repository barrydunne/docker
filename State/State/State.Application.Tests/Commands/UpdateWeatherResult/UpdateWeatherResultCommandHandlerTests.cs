using Microservices.Shared.Mocks;
using State.Application.Commands.UpdateWeatherResult;
using State.Application.Models;

namespace State.Application.Tests.Commands.UpdateWeatherResult;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class UpdateWeatherResultCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly UpdateWeatherResultCommandHandlerTestsContext _context;

    public UpdateWeatherResultCommandHandlerTests()
    {
        _fixture = new();
        _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
        _context = new();
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<UpdateWeatherResultCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_metrics_records_publish_time_for_complete_job()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .With(_ => _.WeatherSuccessful, true)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);
        var command = _fixture.Build<UpdateWeatherResultCommand>().With(_ => _.JobId, job.JobId).Create();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_metrics_records_publish_time_for_incomplete_job()
    {
        var job = _fixture.Build<Job>()
                          .Without(_ => _.DirectionsSuccessful)
                          .With(_ => _.WeatherSuccessful, true)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);
        var command = _fixture.Build<UpdateWeatherResultCommand>().With(_ => _.JobId, job.JobId).Create();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeNotRecorded();
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_updates_successful()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .Without(_ => _.WeatherSuccessful)
                          .Without(_ => _.WeatherForecast)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);
        var command = _fixture.Build<UpdateWeatherResultCommand>().With(_ => _.JobId, job.JobId).Create();
        await _context.Sut.Handle(command, CancellationToken.None);
        job = _context.GetJob(job.JobId);
        Assert.That(job?.WeatherSuccessful, Is.EqualTo(command.Weather.IsSuccessful));
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_updates_result()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .Without(_ => _.WeatherSuccessful)
                          .Without(_ => _.WeatherForecast)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);
        var command = _fixture.Build<UpdateWeatherResultCommand>().With(_ => _.JobId, job.JobId).Create();
        await _context.Sut.Handle(command, CancellationToken.None);
        job = _context.GetJob(job.JobId);
        Assert.That(job?.WeatherForecast, Is.EqualTo(command.Weather));
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_returns_error_for_repository_exception()
    {
        _context.WithJobRepositoryException();
        var command = _fixture.Create<UpdateWeatherResultCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_returns_error_for_send_exception()
    {
        var job = _fixture.Build<Job>()
              .With(_ => _.DirectionsSuccessful, true)
              .With(_ => _.WeatherSuccessful, true)
              .With(_ => _.ImagingSuccessful, true)
              .Create();
        _context.WithJob(job);

        var command = _fixture.Build<UpdateWeatherResultCommand>().With(_ => _.JobId, job.JobId).Create();
        _context.WithSendException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_returns_error_for_send_failure()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .With(_ => _.WeatherSuccessful, true)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);

        var command = _fixture.Build<UpdateWeatherResultCommand>().With(_ => _.JobId, job.JobId).Create();
        _context.WithSendFailure(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_sends_NotifyJobStatusUpdateCommand_on_success()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .With(_ => _.WeatherSuccessful, true)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);

        var command = new UpdateWeatherResultCommand(job.JobId, job.WeatherForecast!);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertNotifyJobStatusUpdateCommandSent(job.JobId, false);
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_sends_NotifyJobStatusUpdateCommand_on_partial_success()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .With(_ => _.WeatherSuccessful, true)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);

        var error = _fixture.Create<string>();
        var command = new UpdateWeatherResultCommand(job.JobId, new(false, null, error));
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertNotifyJobStatusUpdateCommandSent(job.JobId, true);
    }

    [Test]
    public async Task UpdateWeatherResultCommandHandler_sends_NotifyProcessingCompleteCommand()
    {
        var job = _fixture.Build<Job>()
                          .With(_ => _.DirectionsSuccessful, true)
                          .With(_ => _.WeatherSuccessful, true)
                          .With(_ => _.ImagingSuccessful, true)
                          .Create();
        _context.WithJob(job);

        var command = new UpdateWeatherResultCommand(job.JobId, job.WeatherForecast!);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertNotifyProcessingCompleteCommandSent(job.JobId);
    }
}
