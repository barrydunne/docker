using AspNet.KickStarter.FunctionalResult;
using Geocoding.Application.Commands.GeocodeAddresses;
using Geocoding.Application.Queries.GetAddressCoordinates;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
using System.Collections.Concurrent;

namespace Geocoding.Application.Tests.Commands.GeocodeAddresses;

internal class GeocodeAddressesCommandHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockQueue<GeocodingCompleteEvent> _mockQueue;
    private readonly ISender _mockMediator;
    private readonly IGeocodeAddressesCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<GeocodeAddressesCommandHandler> _mockLogger;
    private readonly ConcurrentBag<string> _invalidAddresses;
    private readonly ConcurrentBag<string> _exceptionAddresses;
    private readonly ConcurrentDictionary<string, Coordinates> _geocodedAddresses;

    private string? _withExceptionMessage;
    private bool _validStartingAddress;
    private bool _validDestinationAddress;

    internal GeocodeAddressesCommandHandler Sut { get; }

    public GeocodeAddressesCommandHandlerTestsContext()
    {
        _fixture = new();
        _mockQueue = new();
        _mockMetrics = Substitute.For<IGeocodeAddressesCommandHandlerMetrics>();
        _mockLogger = new();
        _geocodedAddresses = new();
        _invalidAddresses = new();
        _exceptionAddresses = new();

        _withExceptionMessage = null;
        _validStartingAddress = true;
        _validDestinationAddress = true;

        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<GetAddressCoordinatesQuery>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => _withExceptionMessage is not null ? throw new InvalidOperationException(_withExceptionMessage) : GeocodeAddress((GetAddressCoordinatesQuery)callInfo.ArgAt<IRequest<Result<Coordinates>>>(0)))
            .AndDoes(callInfo => _geocodedAddresses[((GetAddressCoordinatesQuery)callInfo.ArgAt<IRequest<Result<Coordinates>>>(0)).Address] = _fixture.Create<Coordinates>());

        Sut = new(_mockQueue, _mockMediator, _mockMetrics, _mockLogger);
    }

    private Task<Result<Coordinates>> GeocodeAddress(GetAddressCoordinatesQuery query) => Task.Run(() =>
    {
        var address = query.Address;
        return _exceptionAddresses.Contains(address)
            ? throw new InvalidDataException(GetError(address))
            : _invalidAddresses.Contains(address)
                ? Result<Coordinates>.FromError(GetError(address))
                : Result<Coordinates>.Success(_geocodedAddresses[address]);
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GeocodeAddressesCommandHandlerTestsContext AssertMetricsGeocodeTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGeocodeTime(Arg.Any<double>());
        return this;
    }

    internal GeocodeAddressesCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
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
            => _.JobId == command.JobId
            && _.StartingCoordinates.IsSuccessful == _validStartingAddress
            && _.StartingCoordinates.Coordinates == (_validStartingAddress ? _geocodedAddresses[command.StartingAddress] : null)
            && _.StartingCoordinates.Error == (_validStartingAddress ? null : GetError(command.StartingAddress))
            && _.DestinationCoordinates.IsSuccessful == _validDestinationAddress
            && _.DestinationCoordinates.Coordinates == (_validDestinationAddress ? _geocodedAddresses[command.DestinationAddress] : null)
            && _.DestinationCoordinates.Error == (_validDestinationAddress ? null : GetError(command.DestinationAddress)));
        Assert.That(published, Is.Not.Null);
        return this;
    }
}
