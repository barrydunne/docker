using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Email.Application.Tests.Mocks;
using Microservices.Shared.Mocks;

namespace Email.Application.Tests.Queries.GetEmailsBase;

internal class GetEmailsBaseQueryHandlerTestsContext
{
    private readonly MockEmailRepository _mockEmailRepository;
    private readonly IGetEmailsSentToRecipientQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetEmailsSentToRecipientQueryHandler> _mockLogger;

    internal GetEmailsSentToRecipientQueryHandler Sut { get; }

    public GetEmailsBaseQueryHandlerTestsContext()
    {
        _mockEmailRepository = new();
        _mockMetrics = Substitute.For<IGetEmailsSentToRecipientQueryHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockEmailRepository, _mockMetrics, _mockLogger);
    }

    internal GetEmailsBaseQueryHandlerTestsContext WithData(IEnumerable<SentEmail> data)
    {
        foreach (var email in data)
            _mockEmailRepository.Emails.Add(email);
        return this;
    }

    internal GetEmailsBaseQueryHandlerTestsContext WithException()
    {
        _mockEmailRepository.WithGetEmailsException();
        return this;
    }

    internal GetEmailsBaseQueryHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GetEmailsBaseQueryHandlerTestsContext AssertMetricsLoadTimeRecorded()
    {
        _mockMetrics.Received(1).RecordLoadTime(Arg.Any<double>());
        return this;
    }
}
