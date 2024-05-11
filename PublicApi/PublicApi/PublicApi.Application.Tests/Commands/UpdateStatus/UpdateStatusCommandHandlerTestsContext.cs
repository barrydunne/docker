using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
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
    private readonly IJobCache _mockJobCache;
    private readonly IUpdateStatusCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateStatusCommandHandler> _mockLogger;

    internal UpdateStatusCommandHandler Sut { get; }

    public UpdateStatusCommandHandlerTestsContext()
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

        _mockMetrics = Substitute.For<IUpdateStatusCommandHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockJobRepository, _mockJobCache, _mockMetrics, _mockLogger);
    }

    internal UpdateStatusCommandHandlerTestsContext WithExistingJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal UpdateStatusCommandHandlerTestsContext AssertMetricsUpdateTimeRecorded()
    {
        _mockMetrics.Received(1).RecordUpdateTime(Arg.Any<double>());
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
        _mockJobCache.Received(1).Remove(jobId);
        return this;
    }
}
