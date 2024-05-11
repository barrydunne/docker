using Microservices.Shared.Events;
using PublicApi.Application.Models;
using PublicApi.Application.Repositories;
using System.Collections.Concurrent;

namespace PublicApi.Application.Tests.Mocks;

internal class MockJobRepository : IJobRepository
{
    internal const string FailingIdempotencyKey = "FailingIdempotencyKey";
    internal static Guid FailingJobId => Guid.Parse("aaaaaBAD-DA7A-aaaa-aaaa-aaaaaaaaaaaa");
    internal const string FailingJobIdError = "FailingJobIdError";

    private readonly ConcurrentBag<Guid> _getJobRequests;

    internal ConcurrentBag<Job> Jobs { get; }

    internal IReadOnlyCollection<Guid> JobRequests => _getJobRequests;

    internal MockJobRepository()
    {
        Jobs = new();
        _getJobRequests = new();
    }

    public Task<Job?> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
        => Task.FromResult(GetJob(jobId));

    public Task<Guid?> GetJobIdByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
        => Task.FromResult(GetJobId(idempotencyKey));

    public Task InsertAsync(Job job, CancellationToken cancellationToken = default)
    {
        AddJob(job);
        return Task.CompletedTask;
    }

    public Task UpdateJobStatusAsync(Guid jobId, JobStatus status, string? additionalInformation, CancellationToken cancellationToken = default)
    {
        UpdateJob(jobId, status, additionalInformation);
        return Task.CompletedTask;
    }

    internal void AddJob(Job job)
    {
        if (job.IdempotencyKey == FailingIdempotencyKey)
            throw new InvalidOperationException(FailingIdempotencyKey);
        Jobs.Add(job);
    }        

    internal Job? GetJob(Guid jobId)
    {
        _getJobRequests.Add(jobId);
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
