using CSharpFunctionalExtensions;
using Geocoding.Logic.Commands;
using Geocoding.Logic.Metrics;
using Geocoding.Logic.Queries;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using System.Collections.Concurrent;

namespace Geocoding.Logic.Tests.CommandHandlers.GeocodeAddressesCommandHandler
{
    internal class GeocodeAddressesCommandHandlerTestsContext
    {
        private readonly Fixture _fixture;
        private readonly MockQueue<GeocodingCompleteEvent> _mockQueue;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IGeocodeAddressesCommandHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Geocoding.Logic.CommandHandlers.GeocodeAddressesCommandHandler> _mockLogger;
        private readonly ConcurrentBag<string> _invalidAddresses;
        private readonly ConcurrentBag<string> _exceptionAddresses;
        private readonly ConcurrentDictionary<string, Coordinates> _geocodedAddresses;

        private string? _withExceptionMessage;
        private bool _validStartingAddress;
        private bool _validDestinationAddress;

        internal Geocoding.Logic.CommandHandlers.GeocodeAddressesCommandHandler Sut { get; }

        public GeocodeAddressesCommandHandlerTestsContext()
        {
            _fixture = new();
            _mockQueue = new();
            _mockMetrics = new();
            _mockLogger = new();
            _geocodedAddresses = new();
            _invalidAddresses = new();
            _exceptionAddresses = new();

            _withExceptionMessage = null;
            _validStartingAddress = true;
            _validDestinationAddress = true;

            _mockMediator = new();
            _mockMediator.Setup(_ => _.Send(It.IsAny<GetAddressCoordinatesQuery>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Result<Coordinates>> query, CancellationToken _) => _geocodedAddresses[((GetAddressCoordinatesQuery)query).Address] = _fixture.Create<Coordinates>())
                .Returns((IRequest<Result<Coordinates>> query, CancellationToken _) => (_withExceptionMessage is not null) ? throw new InvalidOperationException(_withExceptionMessage) : GeocodeAddress((GetAddressCoordinatesQuery)query));

            Sut = new(_mockQueue.Object, _mockMediator.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        private Task<Result<Coordinates>> GeocodeAddress(GetAddressCoordinatesQuery query) => Task.Run(() =>
        {
            var address = query.Address;
            return _exceptionAddresses.Contains(address)
                ? throw new InvalidDataException(GetError(address))
                : _invalidAddresses.Contains(address)
                    ? Result.Failure<Coordinates>(GetError(address))
                    : Result.Success(_geocodedAddresses[address]);
        });

        private string GetError(string address) => $"Invalid: {address}";

        internal GeocodeAddressesCommandHandlerTestsContext WithInvalidStartingAddress(GeocodeAddressesCommand command)
        {
            _validStartingAddress = false;
            _invalidAddresses.Add(command.StartingAddress);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext WithInvalidDestinationAddress(GeocodeAddressesCommand command)
        {
            _validDestinationAddress = false;
            _invalidAddresses.Add(command.DestinationAddress);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext WithStartingAddressException(GeocodeAddressesCommand command)
        {
            _validStartingAddress = false;
            _exceptionAddresses.Add(command.StartingAddress);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext WithDestinationAddressException(GeocodeAddressesCommand command)
        {
            _validDestinationAddress = false;
            _exceptionAddresses.Add(command.DestinationAddress);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext WithException(string message)
        {
            _withExceptionMessage = message;
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext AssertMetricsGuardTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext AssertMetricsGeocodeTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGeocodeTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext AssertAddressGeocoded(string address)
        {
            Assert.That(_geocodedAddresses.Keys, Does.Contain(address));
            return this;
        }

        internal GeocodeAddressesCommandHandlerTestsContext AssertGeocodingCompleteEventPublished(GeocodeAddressesCommand command)
        {
            var published = _mockQueue.Messages.FirstOrDefault(_
                => (_.JobId == command.JobId)
                && (_.StartingCoordinates.IsSuccessful == _validStartingAddress)
                && (_.StartingCoordinates.Coordinates == (_validStartingAddress ? _geocodedAddresses[command.StartingAddress] : null))
                && (_.StartingCoordinates.Error == (_validStartingAddress ? null : GetError(command.StartingAddress)))
                && (_.DestinationCoordinates.IsSuccessful == _validDestinationAddress)
                && (_.DestinationCoordinates.Coordinates == (_validDestinationAddress ? _geocodedAddresses[command.DestinationAddress] : null))
                && (_.DestinationCoordinates.Error == (_validDestinationAddress ? null : GetError(command.DestinationAddress))));
            Assert.That(published, Is.Not.Null);
            return this;
        }
    }
}
