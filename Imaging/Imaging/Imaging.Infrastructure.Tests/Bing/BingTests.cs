using Imaging.Application;
using Microservices.Shared.Events;

namespace Imaging.Infrastructure.Tests.Bing;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "ExternalService")]
internal class BingTests
{
    private readonly Fixture _fixture = new();
    private readonly BingTestsContext _context = new();

    [Test]
    public async Task Bing_GetImageUrlAsync_returns_image_url()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var imageUrl = _fixture.Create<string>();
        _context.WithImageUrl(address, imageUrl);
        var result = await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        result.ShouldBe(imageUrl);
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_ImagingException_for_no_result()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoResult();
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("No image result obtained from Bing.");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_ImagingException_for_no_thumbnail()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoThumbnail();
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("No image result obtained from Bing.");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_ImagingException_for_no_value()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoValue();
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("No image result obtained from Bing.");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_ImagingException_for_bad_request()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithBadRequest();
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("No image result obtained from Bing.");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_ImagingException_for_no_data()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithNoData();
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("No image result obtained from Bing.");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_for_invalid_api_key()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithSecretApiKey(_fixture.Create<string>());
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("No image result obtained from Bing.");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_for_no_api_key()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        _context.WithoutSecretApiKey();
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe("Value cannot be null. (Parameter 'value')");
    }

    [Test]
    public async Task Bing_GetImageUrlAsync_throws_for_exception()
    {
        var address = _fixture.Create<string>();
        var coordinates = _fixture.Create<Coordinates>();
        var correlationId = _fixture.Create<Guid>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        async Task<string?> func() => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId);
        var ex = await func().ShouldThrowAsync<ImagingException>();
        ex.Message.ShouldBe(message);
    }
}
