using Microsoft.Extensions.Caching.Memory;
using PublicApi.Application.Caching;
using PublicApi.Application.Models;

namespace PublicApi.Infrastructure.Caching;

/// <inheritdoc/>
public class JobCache : IJobCache, IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions() { SizeLimit = 100 }); // Room to store 100 jobs
    private bool _disposedValue;

    /// <inheritdoc/>
    public Job? Get(Guid jobId) => _cache.TryGetValue<Job>(jobId, out var job) ? job : null;

    /// <inheritdoc/>
    public void Set(Job job, TimeSpan ttl) => _cache.Set(job.JobId, job, new MemoryCacheEntryOptions { Size = 1, AbsoluteExpirationRelativeToNow = ttl });

    /// <inheritdoc/>
    public void Remove(Guid jobId) => _cache.Remove(jobId);

    /// <summary>
    /// Dispose of this processor.
    /// </summary>
    /// <param name="disposing">Whether to dispose of resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _cache.Dispose();
            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
