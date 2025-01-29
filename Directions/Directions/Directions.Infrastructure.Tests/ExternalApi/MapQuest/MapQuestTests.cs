using Directions.Application;
using Microservices.Shared.Events;
using System.Text.Json;

namespace Directions.Infrastructure.Tests.ExternalApi.MapQuest;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class MapQuestTests
{
    private readonly Fixture _fixture = new();
    private readonly MapQuestTestsContext _context = new();

    [Test]
    public async Task MapQuest_GetDirectionsAsync_returns_directions()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var steps = _fixture.CreateMany<DirectionsStep>().ToArray();
        var directions = new Microservices.Shared.Events.Directions(true, steps.Sum(_ => _.TravelTimeSeconds), steps.Sum(_ => _.DistanceKm), steps, null);
        _context.WithDirections(startingCoordinates, destinationCoordinates, directions);
        var result = await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        // Use JSON to compare steps collections
        JsonSerializer.Serialize(result).ShouldBe(JsonSerializer.Serialize(directions));
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_returns_directions_with_empty_narrative_if_null()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNullNarrative();
        var result = await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        result.Steps!.Where(_ => _.Description == string.Empty).ShouldHaveSingleItem();
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_throws_DirectionsException_for_no_result()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoResult();
        async Task<Microservices.Shared.Events.Directions> func() => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        var ex = await func().ShouldThrowAsync<DirectionsException>();
        ex.Message.ShouldBe("No directions result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_throws_DirectionsException_for_no_legs()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoLegs();
        async Task<Microservices.Shared.Events.Directions> func() => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        var ex = await func().ShouldThrowAsync<DirectionsException>();
        ex.Message.ShouldBe("No directions result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_throws_DirectionsException_for_no_maneuvers()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoManeuvers();
        async Task<Microservices.Shared.Events.Directions> func() => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        var ex = await func().ShouldThrowAsync<DirectionsException>();
        ex.Message.ShouldBe("No directions result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_throws_for_invalid_api_key()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithSecretApiKey(_fixture.Create<string>());
        async Task<Microservices.Shared.Events.Directions> func() => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        var ex = await func().ShouldThrowAsync<DirectionsException>();
        ex.Message.ShouldBe("No directions result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_throws_for_no_api_key()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithoutSecretApiKey();
        async Task<Microservices.Shared.Events.Directions> func() => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        var ex = await func().ShouldThrowAsync<DirectionsException>();
        ex.Message.ShouldBe("No directions result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetDirectionsAsync_throws_for_exception()
    {
        var startingCoordinates = _fixture.Create<Coordinates>();
        var destinationCoordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        async Task<Microservices.Shared.Events.Directions> func() => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
        var ex = await func().ShouldThrowAsync<DirectionsException>();
        ex.Message.ShouldBe(message);
    }
}
