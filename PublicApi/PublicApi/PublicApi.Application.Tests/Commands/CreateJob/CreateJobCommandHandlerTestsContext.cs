using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Models;
using PublicApi.Application.Tests.Mocks;

namespace PublicApi.Application.Tests.Commands.CreateJob;

internal class CreateJobCommandHandlerTestsContext
{
    private readonly MockQueue<JobCreatedEvent> _mockQueue;
    private readonly MockJobRepository _mockJobRepository;
    private readonly Mock<ICreateJobCommandHandlerMetrics> _mockMetrics;
    private readonly MockLogger<CreateJobCommandHandler> _mockLogger;

    internal CreateJobCommandHandler Sut { get; }

    public CreateJobCommandHandlerTestsContext()
    {
        _mockQueue = new();
        _mockJobRepository = new();
        _mockMetrics = new();
        _mockLogger = new();

        Sut = new(_mockQueue.Object, _mockJobRepository.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    internal CreateJobCommandHandlerTestsContext WithExistingJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsIdempotencyTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordIdempotencyTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsSaveTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordSaveTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobSaved(Guid jobId)
    {
        var job = _mockJobRepository.GetJob(jobId);
        Assert.That(job, Is.Not.Null);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobIdempotencyKey(Guid jobId, string idempotencyKey)
    {
        var job = _mockJobRepository.GetJob(jobId);
        Assert.That(job?.IdempotencyKey, Is.EqualTo(idempotencyKey));
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobStatus(Guid jobId, JobStatus status)
    {
        var job = _mockJobRepository.GetJob(jobId);
        Assert.That(job?.Status, Is.EqualTo(status));
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobCreatedEventPublished(Guid jobId, CreateJobCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == jobId
            && _.StartingAddress == command.StartingAddress
            && _.DestinationAddress == command.DestinationAddress);
        Assert.That(published, Is.Not.Null);
        return this;
    }
}
