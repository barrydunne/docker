using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Email.Application.Tests.Mocks;
using Microservices.Shared.Mocks;
using Moq;

namespace Email.Application.Tests.Queries.GetEmailsSentToRecipient;

internal class GetEmailsSentToRecipientQueryHandlerTestsContext
{
    private readonly MockEmailRepository _mockEmailRepository;
    private readonly Mock<IGetEmailsSentToRecipientQueryHandlerMetrics> _mockMetrics;
    private readonly MockLogger<GetEmailsSentToRecipientQueryHandler> _mockLogger;

    internal GetEmailsSentToRecipientQueryHandler Sut { get; }

    public GetEmailsSentToRecipientQueryHandlerTestsContext()
    {
        _mockEmailRepository = new();
        _mockMetrics = new();
        _mockLogger = new();

        Sut = new(_mockEmailRepository.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    internal GetEmailsSentToRecipientQueryHandlerTestsContext WithData(IEnumerable<SentEmail> data)
    {
        foreach (var email in data)
            _mockEmailRepository.Emails.Add(email);
        return this;
    }
}
