using Microservices.Shared.Events;
using System.Text.Json;

namespace Directions.ExternalService.Tests.MapQuest
{
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
            Assert.That(JsonSerializer.Serialize(result), Is.EqualTo(JsonSerializer.Serialize(directions)));
        }

        [Test]
        public async Task MapQuest_GetDirectionsAsync_returns_directions_with_empty_narrative_if_null()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNullNarrative();
            var result = await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId);
            Assert.That(result.Steps, Has.Exactly(1).Matches<DirectionsStep>(_ => _.Description == string.Empty));
        }

        [Test]
        public void MapQuest_GetDirectionsAsync_throws_DirectionsException_for_no_result()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoResult();
            Assert.That(async () => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId),
                Throws.TypeOf<DirectionsException>().With.Property("Message").EqualTo("No directions result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetDirectionsAsync_throws_DirectionsException_for_no_legs()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoLegs();
            Assert.That(async () => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId),
                Throws.TypeOf<DirectionsException>().With.Property("Message").EqualTo("No directions result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetDirectionsAsync_throws_DirectionsException_for_no_maneuvers()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoManeuvers();
            Assert.That(async () => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId),
                Throws.TypeOf<DirectionsException>().With.Property("Message").EqualTo("No directions result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetDirectionsAsync_throws_for_invalid_api_key()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithSecretApiKey(_fixture.Create<string>());
            Assert.That(async () => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId),
                Throws.TypeOf<DirectionsException>().With.Property("Message").EqualTo("No directions result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetDirectionsAsync_throws_for_no_api_key()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithoutSecretApiKey();
            Assert.That(async () => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId),
                Throws.TypeOf<DirectionsException>().With.Property("Message").EqualTo("No directions result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetDirectionsAsync_throws_for_exception()
        {
            var startingCoordinates = _fixture.Create<Coordinates>();
            var destinationCoordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            Assert.That(async () => await _context.Sut.GetDirectionsAsync(startingCoordinates, destinationCoordinates, correlationId),
                Throws.TypeOf<DirectionsException>().With.Property("Message").EqualTo(message));
        }
    }
}
