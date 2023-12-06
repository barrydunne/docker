using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using PublicApi.Application.Caching;
using PublicApi.Application.Commands.UpdateStatus;
using PublicApi.Application.Models;
using PublicApi.Application.Tests.Mocks;
using System.Collections.Concurrent;

namespace PublicApi.Application.Tests.Commands.UpdateStatus;

internal class UpdateStatusCommandHandlerTestsContext
{
    private readonly MockJobRepository _mockJobRepository;
    private readonly ConcurrentDictionary<Guid, Job> _cache;
    private readonly Mock<IJobCache> _mockJobCache;
    private readonly Mock<IUpdateStatusCommandHandlerMetrics> _mockMetrics;
    private readonly MockLogger<UpdateStatusCommandHandler> _mockLogger;

    internal UpdateStatusCommandHandler Sut { get; }

    public UpdateStatusCommandHandlerTestsContext()
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

    internal UpdateStatusCommandHandlerTestsContext WithExistingJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertMetricsUpdateTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordUpdateTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertJobStatus(Guid jobId, JobStatus status)
    {
        var job = _mockJobRepository.GetJob(jobId);
        Assert.That(job?.Status, Is.EqualTo(status));
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertJobAdditionalInformation(Guid jobId, string? additionalInformation)
    {
        var job = _mockJobRepository.GetJob(jobId);
        Assert.That(job?.AdditionalInformation, Is.EqualTo(additionalInformation));
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertJobRemovedFromCache(Guid jobId)
    {
        _mockJobCache.Verify(_ => _.Remove(jobId), Times.Once);
        return this;
    }
}
