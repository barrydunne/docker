using Geocoding.Infrastructure.Caching;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Collections.Concurrent;

namespace Geocoding.Infrastructure.Tests.Caching;

internal class GeocodingCacheTestsContext
{
    private readonly ConcurrentDictionary<string, Coordinates> _cache;
    private readonly Mock<IRedisDatabase> _mockRedisDatabase;
    private readonly MockLogger<GeocodingCache> _mockLogger;

    internal GeocodingCache Sut { get; }

    public GeocodingCacheTestsContext()
    {
        _cache = new();

        _mockRedisDatabase = new();
        _mockRedisDatabase.Setup(_ => _.GetAsync<Coordinates>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((string key, CommandFlags _) => _cache.TryGetValue(key, out var coordinates) ? coordinates : null);
        _mockRedisDatabase.Setup(_ => _.AddAsync<Coordinates>(It.IsAny<string>(), It.IsAny<Coordinates>(), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>(), It.IsAny<HashSet<string>?>()))
            .Callback((string key, Coordinates value, TimeSpan _, When _, CommandFlags _, HashSet<string>? _) => _cache[key] = value)
            .ReturnsAsync(() => true);
        _mockRedisDatabase.Setup(_ => _.RemoveAsync(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .Callback((string key, CommandFlags _) => _cache.TryRemove(key, out var _))
            .ReturnsAsync(() => true);

        _mockLogger = new();

        Sut = new(_mockRedisDatabase.Object, _mockLogger.Object);
    }
}
