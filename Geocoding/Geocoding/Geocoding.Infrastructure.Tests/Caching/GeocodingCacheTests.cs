using Microservices.Shared.Events;

namespace Geocoding.Infrastructure.Tests.Caching;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Caching")]
internal class GeocodingCacheTests
{
    private readonly Fixture _fixture = new();
    private readonly GeocodingCacheTestsContext _context = new();

    [Test]
    public async Task GeocodingCache_GetAsync_returns_null_for_unknown_address()
    {
        var cached = await _context.Sut.GetAsync(_fixture.Create<string>());
        cached.ShouldBeNull();
    }

    [Test]
    public async Task GeocodingCache_GetAsync_returns_null_for_empty_address()
    {
        var cached = await _context.Sut.GetAsync(string.Empty);
        cached.ShouldBeNull();
    }

    [Test]
    public async Task GeocodingCache_GetAsync_returns_null_for_blank_address()
    {
        var cached = await _context.Sut.GetAsync("   ");
        cached.ShouldBeNull();
    }

    [Test]
    public async Task GeocodingCache_GetAsync_returns_coordinates_for_known_key()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        await _context.Sut.SetAsync(address, coordinates, TimeSpan.FromSeconds(10));
        var cached = await _context.Sut.GetAsync(address);
        cached.ShouldBe(coordinates);
    }

    [Test]
    public async Task GeocodingCache_SetAsync_stores_coordinates()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        await _context.Sut.SetAsync(address, coordinates, TimeSpan.FromSeconds(10));
        var cached = await _context.Sut.GetAsync(address);
        cached.ShouldBe(coordinates);
    }

    [Test]
    public async Task GeocodingCache_RemoveAsync_removes_coordinates()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        await _context.Sut.SetAsync(address, coordinates, TimeSpan.FromSeconds(10));
        await _context.Sut.RemoveAsync(address);
        var cached = await _context.Sut.GetAsync(address);
        cached.ShouldBeNull();
    }
}
