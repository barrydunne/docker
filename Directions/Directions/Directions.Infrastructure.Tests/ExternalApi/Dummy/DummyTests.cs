using Microservices.Shared.Events;

namespace Directions.Infrastructure.Tests.ExternalApi.Dummy;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class DummyApiTests
{
    private readonly Fixture _fixture = new();
    private readonly DummyApiTestsContext _context = new();

    [Test]
    public async Task Dummy_GetDirectionsAsync_returns_known_directions()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var directions = _fixture.Build<Microservices.Shared.Events.Directions>().With(_ => _.IsSuccessful, true).With(_ => _.Error, (string?)null).Create();
        _context.WithDirections(startingCoordinates, destinationCoordinates, directions);
        var result = await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        Assert.That(result, Is.EqualTo(directions));
    }

    [Test]
    public async Task Dummy_GetDirectionsAsync_returns_random_directions()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var result = await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        Assert.That(result?.Steps?.Length ?? 0, Is.GreaterThan(0));
    }
}
