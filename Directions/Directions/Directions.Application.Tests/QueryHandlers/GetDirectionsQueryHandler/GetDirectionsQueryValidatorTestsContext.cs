using Directions.Application.Queries.GetDirections;
using Microsoft.Extensions.Logging;
using Moq;

namespace Directions.Application.Tests.QueryHandlers.GetDirectionsQueryHandler;

internal class GetDirectionsQueryValidatorTestsContext
{
    private readonly Mock<IGetDirectionsQueryHandlerMetrics> _mockMetrics;

    internal GetDirectionsQueryValidator Sut { get; }

    public GetDirectionsQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetDirectionsQueryValidator>>().Object);
    }

    internal GetDirectionsQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
