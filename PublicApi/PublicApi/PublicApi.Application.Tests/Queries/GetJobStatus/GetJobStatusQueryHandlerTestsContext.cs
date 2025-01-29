using Microservices.Shared.Mocks;
using PublicApi.Application.Caching;
using PublicApi.Application.Models;
using PublicApi.Application.Queries.GetJobStatus;
using PublicApi.Application.Tests.Mocks;
using System.Collections.Concurrent;

namespace PublicApi.Application.Tests.Queries.GetJobStatus;

internal class GetJobStatusQueryHandlerTestsContext
{
    private readonly MockJobRepository _mockJobRepository;
    private readonly ConcurrentDictionary<Guid, Job> _cache;
    private readonly IJobCache _mockJobCache;
    private readonly IGetJobStatusQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetJobStatusQueryHandler> _mockLogger;

    internal GetJobStatusQueryHandler Sut { get; }

    public GetJobStatusQueryHandlerTestsContext()
    {
        _mockJobRepository = new();
        _cache = new();
        _mockJobCache = Substitute.For<IJobCache>();
        _mockJobCache
            .Get(Arg.Any<Guid>())
            .Returns(callInfo => _cache.TryGetValue(callInfo.ArgAt<Guid>(0), out var job) ? job : null);
        _mockJobCache
            .When(_ => _.Set(Arg.Any<Job>(), Arg.Any<TimeSpan>()))
            .Do(callInfo =>
            {
                var job = callInfo.ArgAt<Job>(0);
                _cache[job.JobId] = job;
            });
        _mockJobCache
            .When(_ => _.Remove(Arg.Any<Guid>()))
            .Do(callInfo => _cache.Remove(callInfo.ArgAt<Guid>(0), out var _));

        _mockMetrics = Substitute.For<IGetJobStatusQueryHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockJobRepository, _mockJobCache, _mockMetrics, _mockLogger);
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GetJobStatusQueryHandlerTestsContext AssertMetricsCacheGetTimeRecorded()
    {
        _mockMetrics.Received(1).RecordCacheGetTime(Arg.Any<double>());
        return this;
    }

    internal GetJobStatusQueryHandlerTestsContext AssertMetricsLoadTimeRecorded()
    {
        _mockMetrics.Received(1).RecordLoadTime(Arg.Any<double>());
        return this;
    }

    internal GetJobStatusQueryHandlerTestsContext AssertMetricsCacheSetTimeRecorded()
    {
        _mockMetrics.Received(1).RecordCacheSetTime(Arg.Any<double>());
        return this;
    }

    internal GetJobStatusQueryHandlerTestsContext AssertRepositoryUsed(Guid jobId)
        => AssertJobRequests(jobId, 1);

    internal GetJobStatusQueryHandlerTestsContext AssertRepositoryNotUsed(Guid jobId)
        => AssertJobRequests(jobId, 0);

    private GetJobStatusQueryHandlerTestsContext AssertJobRequests(Guid jobId, int count)
    {
        _mockJobRepository.JobRequests.Count(_ => _ == jobId).ShouldBe(count);
        return this;
    }

    internal GetJobStatusQueryHandlerTestsContext AssertCacheUpdated(Guid jobId)
    {
        _cache.ShouldContainKey(jobId);
        return this;
    }
}
