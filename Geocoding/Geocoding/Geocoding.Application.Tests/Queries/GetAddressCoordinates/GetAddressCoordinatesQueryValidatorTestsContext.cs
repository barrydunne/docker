using Geocoding.Application.Queries.GetAddressCoordinates;
using Microsoft.Extensions.Logging;
using Moq;

namespace Geocoding.Application.Tests.Queries.GetAddressCoordinates;

internal class GetAddressCoordinatesQueryValidatorTestsContext
{
    private readonly Mock<IGetAddressCoordinatesQueryHandlerMetrics> _mockMetrics;

    internal GetAddressCoordinatesQueryValidator Sut { get; }

    public GetAddressCoordinatesQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetAddressCoordinatesQueryValidator>>().Object);
    }

    internal GetAddressCoordinatesQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
