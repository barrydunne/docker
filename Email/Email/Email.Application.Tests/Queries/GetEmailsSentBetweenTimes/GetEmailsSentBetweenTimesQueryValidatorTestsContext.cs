using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Microsoft.Extensions.Logging;
using Moq;

namespace Email.Application.Tests.Queries.GetEmailsSentBetweenTimes;

internal class GetEmailsSentBetweenTimesQueryValidatorTestsContext
{
    private readonly Mock<IGetEmailsSentBetweenTimesQueryHandlerMetrics> _mockMetrics;

    internal GetEmailsSentBetweenTimesQueryValidator Sut { get; }

    public GetEmailsSentBetweenTimesQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetEmailsSentBetweenTimesQueryValidator>>().Object);
    }

    internal GetEmailsSentBetweenTimesQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
