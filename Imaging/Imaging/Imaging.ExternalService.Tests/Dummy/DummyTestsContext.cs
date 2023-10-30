using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Imaging.ExternalService.Tests.Dummy
{
    internal class DummyTestsContext
    {
        private readonly MockLogger<Imaging.ExternalService.Dummy> _mockLogger;

        internal Imaging.ExternalService.Dummy Sut { get; }

        public DummyTestsContext()
        {
            _mockLogger = new();

            Sut = new(_mockLogger.Object);
        }

        internal DummyTestsContext WithImageUrl(Coordinates coordinates, string imageUrl)
        {
            Imaging.ExternalService.Dummy.AddImageUrl(coordinates, imageUrl);
            return this;
        }
    }
}
