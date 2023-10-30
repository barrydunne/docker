using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Geocoding.ExternalService.Tests.Dummy
{
    internal class DummyTestsContext
    {
        private readonly MockLogger<Geocoding.ExternalService.Dummy> _mockLogger;

        internal Geocoding.ExternalService.Dummy Sut { get; }

        public DummyTestsContext()
        {
            _mockLogger = new();

            Sut = new(_mockLogger.Object);
        }

        internal DummyTestsContext WithAddressCoordinates(string address, Coordinates coordinates)
        {
            Geocoding.ExternalService.Dummy.AddCoordinates(address, coordinates);
            return this;
        }
    }
}
