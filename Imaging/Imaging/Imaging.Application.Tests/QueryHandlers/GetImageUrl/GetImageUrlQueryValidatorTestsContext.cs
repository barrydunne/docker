using Imaging.Application.Queries.GetImageUrl;
using Microservices.Shared.Mocks;

namespace Imaging.Application.Tests.QueryHandlers.GetImageUrl;

internal class GetImageUrlQueryValidatorTestsContext
{
    private readonly IGetImageUrlQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetImageUrlQueryValidator> _mockLogger;

    internal GetImageUrlQueryValidator Sut { get; }

    public GetImageUrlQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetImageUrlQueryHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GetImageUrlQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
