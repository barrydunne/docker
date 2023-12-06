using Email.Application.Queries.GetEmailsSentToRecipient;
using Microsoft.Extensions.Logging;
using Moq;

namespace Email.Application.Tests.Queries.GetEmailsSentToRecipient;

internal class GetEmailsSentToRecipientQueryValidatorTestsContext
{
    private readonly Mock<IGetEmailsSentToRecipientQueryHandlerMetrics> _mockMetrics;

    internal GetEmailsSentToRecipientQueryValidator Sut { get; }

    public GetEmailsSentToRecipientQueryValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<GetEmailsSentToRecipientQueryValidator>>().Object);
    }

    internal GetEmailsSentToRecipientQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
