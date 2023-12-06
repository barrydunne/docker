using Geocoding.Application.Commands.GeocodeAddresses;
using Microsoft.Extensions.Logging;
using Moq;

namespace Geocoding.Application.Tests.Commands.GeocodeAddresses;

internal class GeocodeAddressesCommandValidatorTestsContext
{
    private readonly Mock<IGeocodeAddressesCommandHandlerMetrics> _mockMetrics;

    internal GeocodeAddressesCommandValidator Sut { get; }

    public GeocodeAddressesCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GeocodeAddressesCommandValidator>>().Object);
    }

    internal GeocodeAddressesCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
