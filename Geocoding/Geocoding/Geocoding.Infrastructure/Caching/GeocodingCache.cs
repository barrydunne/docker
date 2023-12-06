using Geocoding.Application.Caching;
using Microservices.Shared.Events;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Geocoding.Infrastructure.Caching;

/// <inheritdoc/>
public class GeocodingCache : IGeocodingCache
{
    private readonly IRedisDatabase _redis;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeocodingCache"/> class.
    /// </summary>
    /// <param name="redis">The redis database that holds the cached coordinates.</param>
    /// <param name="logger">The logger to write to.</param>
    public GeocodingCache(IRedisDatabase redis, ILogger<GeocodingCache> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Coordinates?> GetAsync(string address, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(address))
            return null;
        var coordinates = await _redis.GetAsync<Coordinates>(GetKey(address));
        _logger.LogDebug("Cache {Result} for {Address}", coordinates is null ? "MISS" : "HIT", address);
        return coordinates;
    }

    /// <inheritdoc/>
    public async Task SetAsync(string address, Coordinates coordinates, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Cache set {Coordinates} for {Address}", coordinates, address);
        await _redis.AddAsync(GetKey(address), coordinates, ttl, When.Always);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string address, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Cache remove for {Address}", address);
        await _redis.RemoveAsync(GetKey(address));
    }

    private static string GetKey(string address) => address.ToLowerInvariant().Trim(); // Provide consistent key regardless of system's culture.
}
