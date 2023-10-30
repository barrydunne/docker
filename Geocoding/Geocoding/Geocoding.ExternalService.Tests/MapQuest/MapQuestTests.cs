using Microservices.Shared.Events;

namespace Geocoding.ExternalService.Tests.MapQuest
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "ExternalService")]
    internal class MapQuestTests
    {
        private readonly Fixture _fixture = new();
        private readonly MapQuestTestsContext _context = new();

        /* NOTE: MapQuest seem to return coordinates regardless of bad input with no indicator of validity.
         * Some examples of input address and response:
         *
         * "UNKNOWN"
         *    39.77038, -80.86807
         *
         * "B38C7C23-DD62-4AA6-A886-C3D4051D2AD5"
         *    38.89037, -77.03196
         */

        [Test]
        public async Task MapQuest_GetCoordinatesAsync_returns_coordinates()
        {
            var address = _fixture.Create<string>();
            var correlationId = _fixture.Create<Guid>();
            var coordinates = _fixture.Create<Coordinates>();
            _context.WithAddressCoordinates(address, coordinates);
            var result = await _context.Sut.GetCoordinatesAsync(address, correlationId);
            Assert.That(result, Is.EqualTo(coordinates));
        }

        [Test]
        public void MapQuest_GetCoordinatesAsync_throws_GeocodingException_for_no_result()
        {
            var correlationId = _fixture.Create<Guid>();
            Assert.That(async () => await _context.Sut.GetCoordinatesAsync(string.Empty, correlationId),
                Throws.TypeOf<GeocodingException>().With.Property("Message").EqualTo("No geocoding result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetCoordinatesAsync_throws_for_invalid_api_key()
        {
            var address = _fixture.Create<string>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithSecretApiKey(_fixture.Create<string>());
            Assert.That(async () => await _context.Sut.GetCoordinatesAsync(address, correlationId),
                Throws.TypeOf<GeocodingException>().With.Property("Message").EqualTo("No geocoding result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetCoordinatesAsync_throws_for_no_api_key()
        {
            var address = _fixture.Create<string>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithoutSecretApiKey();
            Assert.That(async () => await _context.Sut.GetCoordinatesAsync(address, correlationId),
                Throws.TypeOf<GeocodingException>().With.Property("Message").EqualTo("No geocoding result obtained from MapQuest."));
        }

        [Test]
        public void MapQuest_GetCoordinatesAsync_throws_for_exception()
        {
            var address = _fixture.Create<string>();
            var correlationId = _fixture.Create<Guid>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            Assert.That(async () => await _context.Sut.GetCoordinatesAsync(address, correlationId),
                Throws.TypeOf<GeocodingException>().With.Property("Message").EqualTo(message));
        }
    }
}
