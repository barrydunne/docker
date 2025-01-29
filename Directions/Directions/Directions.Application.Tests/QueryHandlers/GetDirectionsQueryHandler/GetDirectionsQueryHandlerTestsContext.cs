using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Directions.Application.ExternalApi;
using Directions.Application.Queries.GetDirections;

using EventDirections = Microservices.Shared.Events.Directions;

namespace Directions.Application.Tests.QueryHandlers.GetDirectionsQueryHandler;

internal class GetDirectionsQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly IExternalApi _mockExternalService;
    private readonly IGetDirectionsQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<Queries.GetDirections.GetDirectionsQueryHandler> _mockLogger;

    private EventDirections? _directions;
    private string? _withExceptionMessage;

    internal Queries.GetDirections.GetDirectionsQueryHandler Sut { get; }

    public GetDirectionsQueryHandlerTestsContext()
    {
        _fixture = new();
        _mockMetrics = Substitute.For<IGetDirectionsQueryHandlerMetrics>();
        _mockLogger = new();

        _directions = null;
        _withExceptionMessage = null;

        _mockExternalService = Substitute.For<IExternalApi>();
        _mockExternalService
            .GetDirectionsAsync(Arg.Any<Coordinates>(), Arg.Any<Coordinates>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => GetDirections());

        Sut = new(_mockExternalService, _mockMetrics, _mockLogger);
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GetDirectionsQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Received(1).RecordExternalTime(Arg.Any<double>());
        return this;
    }
}
