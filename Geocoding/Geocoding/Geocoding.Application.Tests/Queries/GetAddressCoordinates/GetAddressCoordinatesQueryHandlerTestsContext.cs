using Geocoding.Application.Caching;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using System.Collections.Concurrent;
using Geocoding.Application.ExternalApi;
using Geocoding.Application.Queries.GetAddressCoordinates;

namespace Geocoding.Application.Tests.Queries.GetAddressCoordinates;

internal class GetAddressCoordinatesQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly IExternalApi _mockExternalService;
    private readonly ConcurrentDictionary<string, Coordinates> _cache;
    private readonly IGeocodingCache _mockGeocodingCache;
    private readonly IGetAddressCoordinatesQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetAddressCoordinatesQueryHandler> _mockLogger;

    private Coordinates? _coordinates;
    private string? _withExceptionMessage;

    internal GetAddressCoordinatesQueryHandler Sut { get; }

    public GetAddressCoordinatesQueryHandlerTestsContext()
    {
        _fixture = new();
        _cache = new();
        _mockMetrics = Substitute.For<IGetAddressCoordinatesQueryHandlerMetrics>();
        _mockLogger = new();

        _coordinates = null;
        _withExceptionMessage = null;

        _mockGeocodingCache = Substitute.For<IGeocodingCache>();
        _mockGeocodingCache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => _cache.TryGetValue(callInfo.ArgAt<string>(0), out var coordinates) ? coordinates : null);
        _mockGeocodingCache
            .When(_ => _.SetAsync(Arg.Any<string>(), Arg.Any<Coordinates>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => _cache[callInfo.ArgAt<string>(0)] = callInfo.ArgAt<Coordinates>(1));

        _mockExternalService = Substitute.For<IExternalApi>();
        _mockExternalService
            .GetCoordinatesAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => GetCoordinates());

        Sut = new(_mockExternalService, _mockGeocodingCache, _mockMetrics, _mockLogger);
    }

    private Coordinates GetCoordinates()
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        return _coordinates ?? _fixture.Create<Coordinates>();
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext WithExternalResult(Coordinates coordinates)
    {
        _coordinates = coordinates;
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext WithCachedResult(string address, Coordinates coordinates)
    {
        _cache[address] = coordinates;
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertMetricsCacheGetTimeRecorded()
    {
        _mockMetrics.Received(1).RecordCacheGetTime(Arg.Any<double>());
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Received(1).RecordExternalTime(Arg.Any<double>());
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertMetricsCacheSetTimeRecorded()
    {
        _mockMetrics.Received(1).RecordCacheSetTime(Arg.Any<double>());
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertExternalServiceNotUsed(string address)
        => AssertExternalServiceUsed(address, 0);

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertExternalServiceUsed(string address)
        => AssertExternalServiceUsed(address, 1);

    private GetAddressCoordinatesQueryHandlerTestsContext AssertExternalServiceUsed(string address, int count)
    {
        _mockExternalService.Received(count).GetCoordinatesAsync(address, Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertCacheUpdated(string address)
    {
        _cache.ShouldContainKey(address);
        return this;
    }
}
