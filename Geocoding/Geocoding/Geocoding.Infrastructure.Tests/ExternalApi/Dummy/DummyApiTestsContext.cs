using Geocoding.Infrastructure.ExternalApi.Dummy;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Geocoding.Infrastructure.Tests.ExternalApi.Dummy;

internal class DummyApiTestsContext
{
    private readonly MockLogger<DummyApi> _mockLogger;

    internal DummyApi Sut { get; }

    public DummyApiTestsContext()
    {
        _mockLogger = new();

        Sut = new(_mockLogger);
    }

    internal DummyApiTestsContext WithAddressCoordinates(string address, Coordinates coordinates)
    {
        DummyApi.AddCoordinates(address, coordinates);
        return this;
    }
}
