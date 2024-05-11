using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Microservices.Shared.Mocks;
using NSubstitute;

namespace Email.Application.Tests.Queries.GetEmailsSentBetweenTimes;

internal class GetEmailsSentBetweenTimesQueryValidatorTestsContext
{
    private readonly IGetEmailsSentBetweenTimesQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetEmailsSentBetweenTimesQueryValidator> _mockLogger;

    internal GetEmailsSentBetweenTimesQueryValidator Sut { get; }

    public GetEmailsSentBetweenTimesQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetEmailsSentBetweenTimesQueryHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal GetEmailsSentBetweenTimesQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
