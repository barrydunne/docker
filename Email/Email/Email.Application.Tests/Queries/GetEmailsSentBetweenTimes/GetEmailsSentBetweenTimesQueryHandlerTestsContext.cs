using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Email.Application.Tests.Mocks;
using Microservices.Shared.Mocks;
using Moq;

namespace Email.Application.Tests.Queries.GetEmailsSentBetweenTimes;

internal class GetEmailsSentBetweenTimesQueryHandlerTestsContext
{
    private readonly MockEmailRepository _mockEmailRepository;
    private readonly Mock<IGetEmailsSentBetweenTimesQueryHandlerMetrics> _mockMetrics;
    private readonly MockLogger<GetEmailsSentBetweenTimesQueryHandler> _mockLogger;

    internal GetEmailsSentBetweenTimesQueryHandler Sut { get; }

    public GetEmailsSentBetweenTimesQueryHandlerTestsContext()
    {
        _mockEmailRepository = new();
        _mockMetrics = new();
        _mockLogger = new();

        Sut = new(_mockEmailRepository.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    internal GetEmailsSentBetweenTimesQueryHandlerTestsContext WithData(IEnumerable<SentEmail> data)
    {
        foreach (var email in data)
            _mockEmailRepository.Emails.Add(email);
        return this;
    }
}
