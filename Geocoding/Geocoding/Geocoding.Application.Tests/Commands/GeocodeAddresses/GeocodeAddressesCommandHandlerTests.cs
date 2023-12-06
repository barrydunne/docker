using Geocoding.Application.Commands.GeocodeAddresses;

namespace Geocoding.Application.Tests.Commands.GeocodeAddresses;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class GeocodeAddressesCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GeocodeAddressesCommandHandlerTestsContext _context = new();

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_records_geocode_time()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsGeocodeTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_records_publish_time()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsPublishTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_geocodes_starting_address()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertAddressGeocoded(command.StartingAddress);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_geocodes_destination_address()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertAddressGeocoded(command.DestinationAddress);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_successful()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_starting_address_fails()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithInvalidStartingAddress(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_destination_address_fails()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithInvalidDestinationAddress(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_starting_and_destination_addresses_fail()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context
            .WithInvalidStartingAddress(command)
            .WithInvalidDestinationAddress(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_starting_address_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithStartingAddressException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_destination_address_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithDestinationAddressException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_success_when_starting_and_destination_addresses_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context
            .WithStartingAddressException(command)
            .WithDestinationAddressException(command);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_successful()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_starting_address_fails()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithInvalidStartingAddress(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_destination_address_fails()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithInvalidDestinationAddress(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_starting_and_destination_addresses_fail()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context
            .WithInvalidStartingAddress(command)
            .WithInvalidDestinationAddress(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_starting_address_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithStartingAddressException(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_destination_address_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context.WithDestinationAddressException(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_publishes_geocoding_complete_event_when_starting_and_destination_addresses_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        _context
            .WithStartingAddressException(command)
            .WithDestinationAddressException(command);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertGeocodingCompleteEventPublished(command);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_message_on_exception()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.Error?.Message, Is.EqualTo(message));
    }
}
