using PublicApi.Logic.Queries;
using PublicApi.Logic.Tests.Mocks;
using PublicApi.Repository.Models;

namespace PublicApi.Logic.Tests.QueryHandlers.GetJobStatusQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetJobStatusQueryHandlerTests
    {
        private readonly PublicApiFixture _fixture = new();
        private readonly GetJobStatusQueryHandlerTestsContext _context = new();

        [Test]
        public async Task GetJobStatusQueryHandler_metrics_increments_count()
        {
            var query = _fixture.Create<GetJobStatusQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task GetJobStatusQueryHandler_metrics_records_cache_get_time()
        {
            var query = _fixture.Create<GetJobStatusQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsCacheGetTimeRecorded();
        }

        [Test]
        public async Task GetJobStatusQueryHandler_metrics_records_load_time()
        {
            var query = _fixture.Create<GetJobStatusQuery>();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsLoadTimeRecorded();
        }

        [Test]
        public async Task GetJobStatusQueryHandler_metrics_records_cache_set_time()
        {
            var job = _fixture.Create<Job>();
            _context.WithExistingJob(job);
            var query = _fixture.Build<GetJobStatusQuery>()
                                .With(_ => _.JobId, job.JobId)
                                .Create();
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsCacheSetTimeRecorded();
        }

        [Test]
        public async Task GetJobStatusQueryHandler_returns_job_if_exists()
        {
            var job = _fixture.Create<Job>();
            _context.WithExistingJob(job);
            var query = _fixture.Build<GetJobStatusQuery>()
                                .With(_ => _.JobId, job.JobId)
                                .Create();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.EqualTo(job));
        }

        [Test]
        public async Task GetJobStatusQueryHandler_returns_null_if_not_exists()
        {
            var query = _fixture.Create<GetJobStatusQuery>();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetJobStatusQueryHandler_returns_job_from_cache_if_exists()
        {
            var job = _fixture.Create<Job>();
            _context.WithExistingCachedJob(job);
            var query = _fixture.Build<GetJobStatusQuery>()
                                .With(_ => _.JobId, job.JobId)
                                .Create();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.EqualTo(job));
            _context.AssertRepositoryNotUsed(job.JobId);
        }

        [Test]
        public async Task GetJobStatusQueryHandler_returns_job_from_repository_if_not_exists_in_cache()
        {
            var job = _fixture.Create<Job>();
            _context.WithExistingJob(job);
            var query = _fixture.Build<GetJobStatusQuery>()
                                .With(_ => _.JobId, job.JobId)
                                .Create();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.EqualTo(job));
            _context.AssertRepositoryUsed(job.JobId);
        }

        [Test]
        public async Task GetJobStatusQueryHandler_saves_job_in_cache()
        {
            var job = _fixture.Create<Job>();
            _context.WithExistingJob(job);
            var query = _fixture.Build<GetJobStatusQuery>()
                                .With(_ => _.JobId, job.JobId)
                                .Create();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.EqualTo(job));
            _context.AssertCacheUpdated(job.JobId);
        }

        [Test]
        public async Task GetJobStatusQueryHandler_returns_null_if_exception()
        {
            var query = _fixture.Build<GetJobStatusQuery>()
                                .With(_ => _.JobId, MockJobRepository.FailingJobId)
                                .Create();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.Null);
        }
    }
}
