using Microservices.Shared.Events;

namespace Geocoding.Application.Caching;

/// <summary>
/// Provides a cache store for geocoded coordinates.
/// </summary>
public interface IGeocodingCache
{
    /// <summary>
    /// Get the cached <see cref="Coordinates"/> for the given address.
    /// </summary>
    /// <param name="address">The address to get cached coordinates for.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The <see cref="Coordinates"/> for the requested address, or null if not found in the cache.</returns>
    Task<Coordinates?> GetAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Store the <see cref="Coordinates"/> in the cache.
    /// </summary>
    /// <param name="address">The address to cache coordinates for.</param>
    /// <param name="coordinates">The <see cref="Coordinates"/> to store.</param>
    /// <param name="ttl">The time to live for the cached item. After this expires the entity will be removed from the cache.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetAsync(string address, Coordinates coordinates, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove <see cref="Coordinates"/> from the cache.
    /// </summary>
    /// <param name="address">The address to remove.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RemoveAsync(string address, CancellationToken cancellationToken = default);
}
