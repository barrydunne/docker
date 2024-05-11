using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Email.Application.Tests.Mocks;
using Microservices.Shared.Mocks;
using NSubstitute;

namespace Email.Application.Tests.Queries.GetEmailsSentToRecipient;

internal class GetEmailsSentToRecipientQueryHandlerTestsContext
{
    private readonly MockEmailRepository _mockEmailRepository;
    private readonly IGetEmailsSentToRecipientQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetEmailsSentToRecipientQueryHandler> _mockLogger;

    internal GetEmailsSentToRecipientQueryHandler Sut { get; }

    public GetEmailsSentToRecipientQueryHandlerTestsContext()
    {
        _mockEmailRepository = new();
        _mockMetrics = Substitute.For<IGetEmailsSentToRecipientQueryHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockEmailRepository, _mockMetrics, _mockLogger);
    }

    internal GetEmailsSentToRecipientQueryHandlerTestsContext WithData(IEnumerable<SentEmail> data)
    {
        foreach (var email in data)
            _mockEmailRepository.Emails.Add(email);
        return this;
    }
}
