using Geocoding.Logic.Queries;
using Microservices.Shared.Events;

namespace Geocoding.Logic.Tests.QueryHandlers.GetAddressCoordinatesQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
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
        public async Task GeocodeAddressesCommandHandler_metrics_records_external_time()
        {
            var query = _fixture.Create<GetAddressCoordinatesQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsExternalTimeRecorded();
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_returns_result_from_external()
        {
            var query = _fixture.Create<GetAddressCoordinatesQuery>();
            var coordinates = _fixture.Create<Coordinates>();
            _context.WithExternalResult(coordinates);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Value, Is.EqualTo(coordinates));
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
            var coordinates = _fixture.Create<Coordinates>();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertExternalServiceUsed(query.Address);
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_saves_coordinates_in_cache()
        {
            var query = _fixture.Create<GetAddressCoordinatesQuery>();
            var coordinates = _fixture.Create<Coordinates>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertCacheUpdated(query.Address);
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_returns_failure_on_exception()
        {
            var query = _fixture.Create<GetAddressCoordinatesQuery>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_returns_message_on_exception()
        {
            var query = _fixture.Create<GetAddressCoordinatesQuery>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(message));
        }
    }
}
