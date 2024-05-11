using Directions.Infrastructure.ExternalApi.Dummy;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Directions.Infrastructure.Tests.ExternalApi.Dummy;

internal class DummyApiTestsContext
{
    private readonly MockLogger<DummyApi> _mockLogger;

    internal DummyApi Sut { get; }

    public DummyApiTestsContext()
    {
        _mockLogger = new();

        Sut = new(_mockLogger);
    }

    internal DummyApiTestsContext WithDirections(Coordinates startingCoordinates, Coordinates destinationCoordinates, Microservices.Shared.Events.Directions directions)
    {
        DummyApi.AddDirections(startingCoordinates, destinationCoordinates, directions);
        return this;
    }
}
