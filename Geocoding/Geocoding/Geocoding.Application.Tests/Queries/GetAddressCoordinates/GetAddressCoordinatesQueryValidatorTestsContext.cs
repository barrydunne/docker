using Geocoding.Application.Queries.GetAddressCoordinates;
using Microservices.Shared.Mocks;

namespace Geocoding.Application.Tests.Queries.GetAddressCoordinates;

internal class GetAddressCoordinatesQueryValidatorTestsContext
{
    private readonly IGetAddressCoordinatesQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetAddressCoordinatesQueryValidator> _mockLogger;

    internal GetAddressCoordinatesQueryValidator Sut { get; }

    public GetAddressCoordinatesQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetAddressCoordinatesQueryHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GetAddressCoordinatesQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
