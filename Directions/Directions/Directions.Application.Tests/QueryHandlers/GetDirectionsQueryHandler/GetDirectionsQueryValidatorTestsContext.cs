using Directions.Application.Queries.GetDirections;
using Microservices.Shared.Mocks;

namespace Directions.Application.Tests.QueryHandlers.GetDirectionsQueryHandler;

internal class GetDirectionsQueryValidatorTestsContext
{
    private readonly IGetDirectionsQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetDirectionsQueryValidator> _mockLogger;

    internal GetDirectionsQueryValidator Sut { get; }

    public GetDirectionsQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetDirectionsQueryHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GetDirectionsQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
