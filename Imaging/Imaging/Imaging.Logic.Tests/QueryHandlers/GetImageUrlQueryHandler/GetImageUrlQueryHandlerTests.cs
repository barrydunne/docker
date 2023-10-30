using Imaging.Logic.Queries;

namespace Imaging.Logic.Tests.QueryHandlers.GetImageUrlQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetImageUrlQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GetImageUrlQueryHandlerTestsContext _context = new();

        [Test]
        public async Task GetImageUrlQueryHandler_metrics_increments_count()
        {
            var query = _fixture.Create<GetImageUrlQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task GetImageUrlQueryHandler_metrics_records_external_time()
        {
            var query = _fixture.Create<GetImageUrlQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsExternalTimeRecorded();
        }

        [Test]
        public async Task GetImageUrlQueryHandler_returns_result_from_external()
        {
            var query = _fixture.Create<GetImageUrlQuery>();
            var imageUrl = _fixture.Create<string>();
            _context.WithExternalResult(imageUrl);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Value, Is.EqualTo(imageUrl));
        }

        [Test]
        public async Task GetImageUrlQueryHandler_returns_failure_on_exception()
        {
            var query = _fixture.Create<GetImageUrlQuery>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task GetImageUrlQueryHandler_returns_message_on_exception()
        {
            var query = _fixture.Create<GetImageUrlQuery>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(message));
        }
    }
}
