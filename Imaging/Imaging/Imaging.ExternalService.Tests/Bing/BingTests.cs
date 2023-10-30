using Microservices.Shared.Events;

namespace Imaging.ExternalService.Tests.Bing
{
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
            Assert.That(result, Is.EqualTo(imageUrl));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_ImagingException_for_no_result()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoResult();
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_ImagingException_for_no_thumbnail()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoThumbnail();
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_ImagingException_for_no_value()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoValue();
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_ImagingException_for_bad_request()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithBadRequest();
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_ImagingException_for_no_data()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithNoData();
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_for_invalid_api_key()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithSecretApiKey(_fixture.Create<string>());
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_for_no_api_key()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            _context.WithoutSecretApiKey();
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo("No image result obtained from Bing."));
        }

        [Test]
        public void Bing_GetImageUrlAsync_throws_for_exception()
        {
            var address = _fixture.Create<string>();
            var coordinates = _fixture.Create<Coordinates>();
            var correlationId = _fixture.Create<Guid>();
            var message = _fixture.Create<string>();
            _context.WithException(message);
            Assert.That(async () => await _context.Sut.GetImageUrlAsync(address, coordinates, correlationId),
                Throws.TypeOf<ImagingException>().With.Property("Message").EqualTo(message));
        }
    }
}
