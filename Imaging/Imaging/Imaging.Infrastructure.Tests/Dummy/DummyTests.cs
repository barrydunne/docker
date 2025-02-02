﻿using Microservices.Shared.Events;

namespace Imaging.Infrastructure.Tests.Dummy;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class DummyApiTests
{
    private readonly Fixture _fixture = new();
    private readonly DummyApiTestsContext _context = new();

    [Test]
    public async Task Dummy_GetImageUrlAsync_returns_known_image_url()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var address = _fixture.Create<string>();
        var imageUrl = _fixture.Create<string>();
        _context.WithImageUrl(coordinates, imageUrl);
        var result = await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        result.ShouldBe(imageUrl);
    }

    [Test]
    public async Task Dummy_GetImageUrlAsync_returns_random_url()
    {
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var address = _fixture.Create<string>();
        var result = await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        result.ShouldNotBeNullOrEmpty();
    }
}
