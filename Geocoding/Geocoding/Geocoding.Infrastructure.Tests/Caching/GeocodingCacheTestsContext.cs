using Geocoding.Infrastructure.Caching;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Collections.Concurrent;

namespace Geocoding.Infrastructure.Tests.Caching;

internal class GeocodingCacheTestsContext
{
    private readonly ConcurrentDictionary<string, Coordinates> _cache;
    private readonly IRedisDatabase _mockRedisDatabase;
    private readonly MockLogger<GeocodingCache> _mockLogger;

    internal GeocodingCache Sut { get; }

    public GeocodingCacheTestsContext()
    {
        _cache = new();

        _mockRedisDatabase = Substitute.For<IRedisDatabase>();
        _mockRedisDatabase
            .GetAsync<Coordinates>(Arg.Any<string>(), Arg.Any<CommandFlags>())
            .Returns(callInfo => _cache.TryGetValue(callInfo.ArgAt<string>(0), out var coordinates) ? coordinates : null);
        _mockRedisDatabase
            .AddAsync<Coordinates>(Arg.Any<string>(), Arg.Any<Coordinates>(), Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>(), Arg.Any<HashSet<string>?>())
            .Returns(true)
            .AndDoes(callInfo => _cache[callInfo.ArgAt<string>(0)] = callInfo.ArgAt<Coordinates>(1));
        _mockRedisDatabase
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CommandFlags>())
            .Returns(true)
            .AndDoes(callInfo => _cache.TryRemove(callInfo.ArgAt<string>(0), out var _));

        _mockLogger = new();

        Sut = new(_mockRedisDatabase, _mockLogger);
    }
}
