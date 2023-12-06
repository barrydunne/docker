using Microservices.Shared.Events;
using Moq;
using PublicApi.Application.Models;
using PublicApi.Application.Repositories;
using System.Collections.Concurrent;

namespace PublicApi.Application.Tests.Mocks;

internal class MockJobRepository : Mock<IJobRepository>
{
    internal const string FailingIdempotencyKey = "FailingIdempotencyKey";
    internal static Guid FailingJobId => Guid.Parse("aaaaaBAD-DA7A-aaaa-aaaa-aaaaaaaaaaaa");
    internal const string FailingJobIdError = "FailingJobIdError";

    internal ConcurrentBag<Job> Jobs { get; }

    internal MockJobRepository() : base(MockBehavior.Strict)
    {
        Jobs = new();

        Setup(_ => _.InsertAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Callback((Job job, CancellationToken _) => AddJob(job))
            .Returns(Task.CompletedTask);

        Setup(_ => _.GetJobByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, CancellationToken _) => GetJob(jobId));

        Setup(_ => _.GetJobIdByIdempotencyKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string idempotencyKey, CancellationToken _) => GetJobId(idempotencyKey));

        Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<JobStatus>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback((Guid jobId, JobStatus status, string? additionalInformation, CancellationToken _) => UpdateJob(jobId, status, additionalInformation))
            .Returns(Task.CompletedTask);
    }

    internal void AddJob(Job job)
    {
        if (job.IdempotencyKey == FailingIdempotencyKey)
            throw new InvalidOperationException(FailingIdempotencyKey);
        Jobs.Add(job);
    }        

    internal Job? GetJob(Guid jobId)
    {
        if (jobId == FailingJobId)
            throw new InvalidOperationException(FailingJobIdError);
        return Jobs.FirstOrDefault(_ => _.JobId == jobId);
    }

    internal Guid? GetJobId(string idempotencyKey) => Jobs.FirstOrDefault(_ => _.IdempotencyKey == idempotencyKey)?.JobId;

    internal void UpdateJob(Guid jobId, JobStatus status, string? additionalInformation)
    {
        var job = GetJob(jobId);
        if (job is null)
            return;
        job.Status = status;
        job.AdditionalInformation = additionalInformation;
    }
}
