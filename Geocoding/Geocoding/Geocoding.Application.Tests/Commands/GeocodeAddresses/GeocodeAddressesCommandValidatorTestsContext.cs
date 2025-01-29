using Geocoding.Application.Commands.GeocodeAddresses;
using Microservices.Shared.Mocks;

namespace Geocoding.Application.Tests.Commands.GeocodeAddresses;

internal class GeocodeAddressesCommandValidatorTestsContext
{
    private readonly IGeocodeAddressesCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<GeocodeAddressesCommandValidator> _mockLogger;

    internal GeocodeAddressesCommandValidator Sut { get; }

    public GeocodeAddressesCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGeocodeAddressesCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GeocodeAddressesCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
