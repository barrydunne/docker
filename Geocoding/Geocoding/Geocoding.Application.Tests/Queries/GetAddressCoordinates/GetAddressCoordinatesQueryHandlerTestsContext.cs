using Geocoding.Application.Caching;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using System.Collections.Concurrent;
using Geocoding.Application.ExternalApi;
using Geocoding.Application.Queries.GetAddressCoordinates;

namespace Geocoding.Application.Tests.Queries.GetAddressCoordinates;

internal class GetAddressCoordinatesQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly Mock<IExternalApi> _mockExternalService;
    private readonly ConcurrentDictionary<string, Coordinates> _cache;
    private readonly Mock<IGeocodingCache> _mockGeocodingCache;
    private readonly Mock<IGetAddressCoordinatesQueryHandlerMetrics> _mockMetrics;
    private readonly MockLogger<GetAddressCoordinatesQueryHandler> _mockLogger;

    private Coordinates? _coordinates;
    private string? _withExceptionMessage;

    internal GetAddressCoordinatesQueryHandler Sut { get; }

    public GetAddressCoordinatesQueryHandlerTestsContext()
    {
        _fixture = new();
        _cache = new();
        _mockMetrics = new();
        _mockLogger = new();

        _coordinates = null;
        _withExceptionMessage = null;

        _mockGeocodingCache = new();
        _mockGeocodingCache.Setup(_ => _.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string address, CancellationToken _) => _cache.TryGetValue(address, out var coordinates) ? coordinates : null);
        _mockGeocodingCache.Setup(_ => _.SetAsync(It.IsAny<string>(), It.IsAny<Coordinates>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback((string address, Coordinates coordinates, TimeSpan _, CancellationToken _) => _cache[address] = coordinates);

        _mockExternalService = new();
        _mockExternalService.Setup(_ => _.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => GetCoordinates());

        Sut = new(_mockExternalService.Object, _mockGeocodingCache.Object, _mockMetrics.Object, _mockLogger.Object);
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
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordExternalTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertExternalServiceNotUsed(string address)
        => AssertExternalServiceUsed(address, Times.Never());

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertExternalServiceUsed(string address)
        => AssertExternalServiceUsed(address, Times.Once());

    private GetAddressCoordinatesQueryHandlerTestsContext AssertExternalServiceUsed(string address, Times times)
    {
        _mockExternalService.Verify(_ => _.GetCoordinatesAsync(address, It.IsAny<Guid>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    internal GetAddressCoordinatesQueryHandlerTestsContext AssertCacheUpdated(string address)
    {
        Assert.That(_cache.ContainsKey(address), Is.True);
        return this;
    }
}
