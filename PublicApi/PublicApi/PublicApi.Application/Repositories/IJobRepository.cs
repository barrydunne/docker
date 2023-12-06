using Microservices.Shared.Events;
using PublicApi.Application.Models;

namespace PublicApi.Application.Repositories;

/// <summary>
/// Repository for Job documents.
/// </summary>
public interface IJobRepository
{
    /// <summary>
    /// Insert a new job into the repository.
    /// </summary>
    /// <param name="job">The job to insert.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task InsertAsync(Job job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a previously inserted job that has the given JobId.
    /// </summary>
    /// <param name="jobId">The JobId of the job to retrieve.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The matching job, or null if no match found.</returns>
    Task<Job?> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a previously inserted job that has the given idempotency key and was created within the previous 7 days.
    /// </summary>
    /// <param name="idempotencyKey">The idempotency key of the job to retrieve.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The id of the matching existing job, or null if no match found.</returns>
    Task<Guid?> GetJobIdByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the status of an existing job.
    /// </summary>
    /// <param name="jobId">The JobId of the job to update.</param>
    /// <param name="status">The new status.</param>
    /// <param name="additionalInformation">The new additional information.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpdateJobStatusAsync(Guid jobId, JobStatus status, string? additionalInformation, CancellationToken cancellationToken = default);
}
