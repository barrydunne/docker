using Geocoding.Application.Queries.GetAddressCoordinates;
using Microservices.Shared.Events;

namespace Geocoding.Application.Tests.Queries.GetAddressCoordinates;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetAddressCoordinatesQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GetAddressCoordinatesQueryHandlerTestsContext _context = new();

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_increments_count()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_records_cache_get_time()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertMetricsCacheGetTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_records_external_time()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertMetricsExternalTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_records_cache_set_time()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertMetricsCacheSetTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_result_from_external()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        var coordinates = _fixture.Create<Coordinates>();
        _context.WithExternalResult(coordinates);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.Value.ShouldBe(coordinates);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_result_from_cache_if_exists()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        var coordinates = _fixture.Create<Coordinates>();
        _context.WithCachedResult(query.Address, coordinates);
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertExternalServiceNotUsed(query.Address);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_result_from_external_service_if_not_exists_in_cache()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertExternalServiceUsed(query.Address);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_saves_coordinates_in_cache()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertCacheUpdated(query.Address);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_error_on_exception()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_message_on_exception()
    {
        var query = _fixture.Create<GetAddressCoordinatesQuery>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.Error?.Message.ShouldBe(message);
    }
}
