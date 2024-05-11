using Imaging.Infrastructure.ExternalApi.Dummy;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Imaging.Infrastructure.Tests.Dummy;

internal class DummyApiTestsContext
{
    private readonly MockLogger<DummyApi> _mockLogger;

    internal DummyApi Sut { get; }

    public DummyApiTestsContext()
    {
        _mockLogger = new();

        Sut = new(_mockLogger);
    }

    internal DummyApiTestsContext WithImageUrl(Coordinates coordinates, string imageUrl)
    {
        DummyApi.AddImageUrl(coordinates, imageUrl);
        return this;
    }
}
