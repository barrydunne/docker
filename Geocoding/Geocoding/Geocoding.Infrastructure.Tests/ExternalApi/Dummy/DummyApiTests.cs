using Microservices.Shared.Events;

namespace Geocoding.Infrastructure.Tests.ExternalApi.Dummy;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class DummyApiTests
{
    private readonly Fixture _fixture = new();
    private readonly DummyApiTestsContext _context = new();

    [Test]
    public async Task Dummy_GetCoordinatesAsync_returns_known_coordinates()
    {
        var address = _fixture.Create<string>();
        var correlationId = _fixture.Create<Guid>();
        var coordinates = _fixture.Create<Coordinates>();
        _context.WithAddressCoordinates(address, coordinates);
        var result = await _context.Sut.GetCoordinatesAsync(address, correlationId);
        Assert.That(result, Is.EqualTo(coordinates));
    }

    [Test]
    public async Task Dummy_GetCoordinatesAsync_returns_random_coordinates()
    {
        var address = _fixture.Create<string>();
        var correlationId = _fixture.Create<Guid>();
        var result = await _context.Sut.GetCoordinatesAsync(address, correlationId);
        Assert.That(result.Latitude, Is.GreaterThan(0));
    }
}
