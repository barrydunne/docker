using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Models;

namespace State.Application.Tests.Commands.UpdateGeocodingResult;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
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
    public async Task UpdateGeocodingResultCommandHandler_returns_error_for_no_job()
    {
        var command = _fixture.Create<UpdateGeocodingResultCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.Error?.Message.ShouldBe("Job not yet available for update.");
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
    public async Task UpdateGeocodingResultCommandHandler_returns_error_for_publish_exception()
    {
        var job = _fixture.Create<Job>();
        _context.WithJob(job);
        var command = new UpdateGeocodingResultCommand(job.JobId, _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create(), _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create());
        _context.WithPublishException();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task UpdateGeocodingResultCommandHandler_returns_error_for_send_exception()
    {
        var job = _fixture.Create<Job>();
        _context.WithJob(job);
        var command = new UpdateGeocodingResultCommand(job.JobId, _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, true).Create(), _fixture.Build<GeocodingCoordinates>().With(_ => _.IsSuccessful, false).Create());
        _context.WithSendException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }
}
