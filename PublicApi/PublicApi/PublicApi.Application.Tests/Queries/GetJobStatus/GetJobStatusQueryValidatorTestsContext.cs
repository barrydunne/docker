using Microsoft.Extensions.Logging;
using Moq;
using PublicApi.Application.Queries.GetJobStatus;

namespace PublicApi.Application.Tests.Queries.GetJobStatus;

internal class GetJobStatusQueryValidatorTestsContext
{
    private readonly Mock<IGetJobStatusQueryHandlerMetrics> _mockMetrics;

    internal GetJobStatusQueryValidator Sut { get; }

    public GetJobStatusQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetJobStatusQueryValidator>>().Object);
    }

    internal GetJobStatusQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
