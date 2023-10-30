using Email.Logic.Metrics;
using Email.Logic.Tests.Mocks;
using Email.Repository.Models;
using Microservices.Shared.Mocks;
using Moq;

namespace Email.Logic.Tests.QueryHandlers.GetEmailsSentToRecipientQueryHandler
{
    internal class GetEmailsSentToRecipientQueryHandlerTestsContext
    {
        private readonly MockEmailRepository _mockEmailRepository;
        private readonly Mock<IGetEmailsSentToRecipientQueryHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Email.Logic.QueryHandlers.GetEmailsSentToRecipientQueryHandler> _mockLogger;

        internal Email.Logic.QueryHandlers.GetEmailsSentToRecipientQueryHandler Sut { get; }

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
}
