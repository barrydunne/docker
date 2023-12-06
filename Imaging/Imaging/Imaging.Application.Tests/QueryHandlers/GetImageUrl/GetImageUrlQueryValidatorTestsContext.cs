using Imaging.Application.Queries.GetImageUrl;
using Microsoft.Extensions.Logging;
using Moq;

namespace Imaging.Application.Tests.QueryHandlers.GetImageUrl;

internal class GetImageUrlQueryValidatorTestsContext
{
    private readonly Mock<IGetImageUrlQueryHandlerMetrics> _mockMetrics;

    internal GetImageUrlQueryValidator Sut { get; }

    public GetImageUrlQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetImageUrlQueryValidator>>().Object);
    }

    internal GetImageUrlQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
