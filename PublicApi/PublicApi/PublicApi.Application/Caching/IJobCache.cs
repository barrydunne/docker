using PublicApi.Application.Models;

namespace PublicApi.Application.Caching;

/// <summary>
/// Provides a cache store for <see cref="Job"/> entities.
/// </summary>
public interface IJobCache
{
    /// <summary>
    /// Get the cached <see cref="Job"/> with the given id.
    /// </summary>
    /// <param name="jobId">The id of the job to get.</param>
    /// <returns>The <see cref="Job"/> with the requested id, or null if not found in the cache.</returns>
    Job? Get(Guid jobId);

    /// <summary>
    /// Store the <see cref="Job"/> in the cache.
    /// </summary>
    /// <param name="job">The job to store.</param>
    /// <param name="ttl">The time to live for the cached item. After this expires the entity will be removed from the cache.</param>
    void Set(Job job, TimeSpan ttl);

    /// <summary>
    /// Remove a <see cref="Job"/> from the cache.
    /// </summary>
    /// <param name="jobId">The id of the job to remove.</param>
    void Remove(Guid jobId);
}
