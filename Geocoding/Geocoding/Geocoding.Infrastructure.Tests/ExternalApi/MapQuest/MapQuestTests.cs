using Geocoding.Application;
using Microservices.Shared.Events;

namespace Geocoding.Infrastructure.Tests.ExternalApi.MapQuest;

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
        result.ShouldBe(coordinates);
    }

    [Test]
    public async Task MapQuest_GetCoordinatesAsync_throws_GeocodingException_for_no_result()
    {
        var correlationId = _fixture.Create<Guid>();
        async Task<Coordinates> func() => await _context.Sut.GetCoordinatesAsync(string.Empty, correlationId);
        var ex = await func().ShouldThrowAsync<GeocodingException>();
        ex.Message.ShouldBe("No geocoding result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetCoordinatesAsync_throws_for_invalid_api_key()
    {
        var address = _fixture.Create<string>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithSecretApiKey(_fixture.Create<string>());
        async Task<Coordinates> func() => await _context.Sut.GetCoordinatesAsync(address, correlationId);
        var ex = await func().ShouldThrowAsync<GeocodingException>();
        ex.Message.ShouldBe("No geocoding result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetCoordinatesAsync_throws_for_no_api_key()
    {
        var address = _fixture.Create<string>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithoutSecretApiKey();
        async Task<Coordinates> func() => await _context.Sut.GetCoordinatesAsync(address, correlationId);
        var ex = await func().ShouldThrowAsync<GeocodingException>();
        ex.Message.ShouldBe("No geocoding result obtained from MapQuest.");
    }

    [Test]
    public async Task MapQuest_GetCoordinatesAsync_throws_for_exception()
    {
        var address = _fixture.Create<string>();
        var correlationId = _fixture.Create<Guid>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        async Task<Coordinates> func() => await _context.Sut.GetCoordinatesAsync(address, correlationId);
        var ex = await func().ShouldThrowAsync<GeocodingException>();
        ex.Message.ShouldBe(message);
    }
}
