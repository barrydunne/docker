using Microservices.Shared.Mocks;
using Moq;
using PublicApi.Logic.Caching;
using PublicApi.Logic.Metrics;
using PublicApi.Logic.Tests.Mocks;
using PublicApi.Repository.Models;
using System.Collections.Concurrent;

namespace PublicApi.Logic.Tests.QueryHandlers.GetJobStatusQueryHandler
{
    internal class GetJobStatusQueryHandlerTestsContext
    {
        private readonly MockJobRepository _mockJobRepository;
        private readonly ConcurrentDictionary<Guid, Job> _cache;
        private readonly Mock<IJobCache> _mockJobCache;
        private readonly Mock<IGetJobStatusQueryHandlerMetrics> _mockMetrics;
        private readonly MockLogger<PublicApi.Logic.QueryHandlers.GetJobStatusQueryHandler> _mockLogger;

        internal PublicApi.Logic.QueryHandlers.GetJobStatusQueryHandler Sut { get; }

        public GetJobStatusQueryHandlerTestsContext()
        {
            _mockJobRepository = new();
            _cache = new();
            _mockJobCache = new(MockBehavior.Strict);
            _mockJobCache.Setup(_ => _.Get(It.IsAny<Guid>())).Returns((Guid jobId) => _cache.TryGetValue(jobId, out var job) ? job : null);
            _mockJobCache.Setup(_ => _.Set(It.IsAny<Job>(), It.IsAny<TimeSpan>())).Callback((Job job, TimeSpan _) => _cache[job.JobId] = job);
            _mockJobCache.Setup(_ => _.Remove(It.IsAny<Guid>())).Callback((Guid jobId) => _cache.Remove(jobId, out var _));
            _mockMetrics = new();
            _mockLogger = new();

            Sut = new(_mockJobRepository.Object, _mockJobCache.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        internal GetJobStatusQueryHandlerTestsContext WithExistingJob(Job job)
        {
            _mockJobRepository.AddJob(job);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext WithExistingCachedJob(Job job)
        {
            _cache[job.JobId] = job;
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertMetricsCacheGetTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordCacheGetTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertMetricsLoadTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordLoadTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertMetricsCacheSetTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordCacheSetTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertRepositoryUsed(Guid jobId)
        {
            _mockJobRepository.Verify(_ => _.GetJobIdByIdAsync(jobId, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertRepositoryNotUsed(Guid jobId)
        {
            _mockJobRepository.Verify(_ => _.GetJobIdByIdAsync(jobId, It.IsAny<CancellationToken>()), Times.Never);
            return this;
        }

        internal GetJobStatusQueryHandlerTestsContext AssertCacheUpdated(Guid jobId)
        {
            Assert.That(_cache.ContainsKey(jobId), Is.True);
            return this;
        }
    }
}
