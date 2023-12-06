using Microservices.Shared.Events;
using State.Application.Models;

namespace State.Application.Repositories;

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
    /// Get whether the job has completed all processing successfully or unsuccessfully.
    /// </summary>
    /// <param name="jobId">The JobId of the job to retrieve.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>True if all tasks have completed successfully, false if all tasks have completed but not all successfully, null if not all tasks have completed.</returns>
    Task<bool?> GetJobCompletionStatusAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the geocoding status of an existing job.
    /// </summary>
    /// <param name="jobId">The JobId of the job to update.</param>
    /// <param name="isSuccessful">Whether the task completed successfully.</param>
    /// <param name="result">The result of the task.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The count of records matched for update (0 or 1).</returns>
    Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, GeocodingResult result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the directions status of an existing job.
    /// </summary>
    /// <param name="jobId">The JobId of the job to update.</param>
    /// <param name="isSuccessful">Whether the task completed successfully.</param>
    /// <param name="result">The result of the task.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The count of records matched for update (0 or 1).</returns>
    Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, Directions result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the weather status of an existing job.
    /// </summary>
    /// <param name="jobId">The JobId of the job to update.</param>
    /// <param name="isSuccessful">Whether the task completed successfully.</param>
    /// <param name="result">The result of the task.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The count of records matched for update (0 or 1).</returns>
    Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, WeatherForecast result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the imaging status of an existing job.
    /// </summary>
    /// <param name="jobId">The JobId of the job to update.</param>
    /// <param name="isSuccessful">Whether the task completed successfully.</param>
    /// <param name="result">The result of the task.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The count of records matched for update (0 or 1).</returns>
    Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, ImagingResult result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete any existing job with the given id.
    /// </summary>
    /// <param name="jobId">The id of the job to delete.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The count of records matched for deletion (0 or 1).</returns>
    Task<long> DeleteJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default);
}
