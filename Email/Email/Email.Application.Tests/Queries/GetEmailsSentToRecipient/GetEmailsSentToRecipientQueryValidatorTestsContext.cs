using Email.Application.Queries.GetEmailsSentToRecipient;
using Microservices.Shared.Mocks;

namespace Email.Application.Tests.Queries.GetEmailsSentToRecipient;

internal class GetEmailsSentToRecipientQueryValidatorTestsContext
{
    private readonly IGetEmailsSentToRecipientQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetEmailsSentToRecipientQueryValidator> _mockLogger;

    internal GetEmailsSentToRecipientQueryValidator Sut { get; }

    public GetEmailsSentToRecipientQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetEmailsSentToRecipientQueryHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GetEmailsSentToRecipientQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
