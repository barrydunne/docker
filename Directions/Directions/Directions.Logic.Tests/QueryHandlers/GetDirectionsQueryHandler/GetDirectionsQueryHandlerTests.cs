using Directions.Logic.Queries;

namespace Directions.Logic.Tests.QueryHandlers.GetDirectionsQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetDirectionsQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GetDirectionsQueryHandlerTestsContext _context = new();

        [Test]
        public async Task GeocodeAddressesCommandHandler_metrics_increments_count()
        {
            var query = _fixture.Create<GetDirectionsQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_metrics_records_external_time()
        {
            var query = _fixture.Create<GetDirectionsQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsExternalTimeRecorded();
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_returns_result_from_external()
        {
            var query = _fixture.Create<GetDirectionsQuery>();
            var directions = _fixture.Create<Microservices.Shared.Events.Directions>();
            _context.WithExternalResult(directions);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Value, Is.EqualTo(directions));
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_returns_failure_on_exception()
        {
            var query = _fixture.Create<GetDirectionsQuery>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task GeocodeAddressesCommandHandler_returns_message_on_exception()
        {
            var query = _fixture.Create<GetDirectionsQuery>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(message));
        }
    }
}
