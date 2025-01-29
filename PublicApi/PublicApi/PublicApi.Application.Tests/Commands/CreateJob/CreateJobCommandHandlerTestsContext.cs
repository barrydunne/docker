using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Models;
using PublicApi.Application.Tests.Mocks;

namespace PublicApi.Application.Tests.Commands.CreateJob;

internal class CreateJobCommandHandlerTestsContext
{
    private readonly MockQueue<JobCreatedEvent> _mockQueue;
    private readonly MockJobRepository _mockJobRepository;
    private readonly ICreateJobCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<CreateJobCommandHandler> _mockLogger;

    internal CreateJobCommandHandler Sut { get; }

    public CreateJobCommandHandlerTestsContext()
    {
        _mockQueue = new();
        _mockJobRepository = new();
        _mockMetrics = Substitute.For<ICreateJobCommandHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockQueue, _mockJobRepository, _mockMetrics, _mockLogger);
    }

    internal CreateJobCommandHandlerTestsContext WithExistingJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsIdempotencyTimeRecorded()
    {
        _mockMetrics.Received(1).RecordIdempotencyTime(Arg.Any<double>());
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsSaveTimeRecorded()
    {
        _mockMetrics.Received(1).RecordSaveTime(Arg.Any<double>());
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobSaved(Guid jobId)
    {
        var job = _mockJobRepository.GetJob(jobId);
        job.ShouldNotBeNull();
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobIdempotencyKey(Guid jobId, string idempotencyKey)
    {
        var job = _mockJobRepository.GetJob(jobId);
        job?.IdempotencyKey.ShouldBe(idempotencyKey);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobStatus(Guid jobId, JobStatus status)
    {
        var job = _mockJobRepository.GetJob(jobId);
        job?.Status.ShouldBe(status);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobCreatedEventPublished(Guid jobId, CreateJobCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == jobId
            && _.StartingAddress == command.StartingAddress
            && _.DestinationAddress == command.DestinationAddress);
        published.ShouldNotBeNull();
        return this;
    }
}
