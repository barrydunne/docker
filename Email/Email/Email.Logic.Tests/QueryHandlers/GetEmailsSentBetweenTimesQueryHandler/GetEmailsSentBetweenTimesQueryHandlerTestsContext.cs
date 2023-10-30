using Email.Logic.Metrics;
using Email.Logic.Tests.Mocks;
using Email.Repository.Models;
using Microservices.Shared.Mocks;
using Moq;

namespace Email.Logic.Tests.QueryHandlers.GetEmailsSentBetweenTimesQueryHandler
{
    internal class GetEmailsSentBetweenTimesQueryHandlerTestsContext
    {
        private readonly MockEmailRepository _mockEmailRepository;
        private readonly Mock<IGetEmailsSentBetweenTimesQueryHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Email.Logic.QueryHandlers.GetEmailsSentBetweenTimesQueryHandler> _mockLogger;

        internal Email.Logic.QueryHandlers.GetEmailsSentBetweenTimesQueryHandler Sut { get; }

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
}
