using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;

using EventDirections = Microservices.Shared.Events.Directions;
using Directions.Application.ExternalApi;
using Directions.Application.Queries.GetDirections;

namespace Directions.Application.Tests.QueryHandlers.GetDirectionsQueryHandler;

internal class GetDirectionsQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly Mock<IExternalApi> _mockExternalService;
    private readonly Mock<IGetDirectionsQueryHandlerMetrics> _mockMetrics;
    private readonly MockLogger<Queries.GetDirections.GetDirectionsQueryHandler> _mockLogger;

    private EventDirections? _directions;
    private string? _withExceptionMessage;

    internal Queries.GetDirections.GetDirectionsQueryHandler Sut { get; }

    public GetDirectionsQueryHandlerTestsContext()
    {
        _fixture = new();
        _mockMetrics = new();
        _mockLogger = new();

        _directions = null;
        _withExceptionMessage = null;

        _mockExternalService = new();
        _mockExternalService.Setup(_ => _.GetDirectionsAsync(It.IsAny<Coordinates>(), It.IsAny<Coordinates>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => GetDirections());

        Sut = new(_mockExternalService.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    private EventDirections GetDirections()
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        return _directions ?? _fixture.Build<EventDirections>().With(_ => _.IsSuccessful, true).With(_ => _.Error, (string?)null).Create();
    }

    internal GetDirectionsQueryHandlerTestsContext WithExternalResult(EventDirections directions)
    {
        _directions = directions;
        return this;
    }

    internal GetDirectionsQueryHandlerTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal GetDirectionsQueryHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal GetDirectionsQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordExternalTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
