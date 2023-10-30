using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Directions.ExternalService.Tests.Dummy
{
    internal class DummyTestsContext
    {
        private readonly MockLogger<Directions.ExternalService.Dummy> _mockLogger;

        internal Directions.ExternalService.Dummy Sut { get; }

        public DummyTestsContext()
        {
            _mockLogger = new();

            Sut = new(_mockLogger.Object);
        }

        internal DummyTestsContext WithDirections(Coordinates startingCoordinates, Coordinates destinationCoordinates, Microservices.Shared.Events.Directions directions)
        {
            Directions.ExternalService.Dummy.AddDirections(startingCoordinates, destinationCoordinates, directions);
            return this;
        }
    }
}
