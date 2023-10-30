using Email.Logic.Metrics;
using Email.Logic.Tests.Mocks;
using Email.Repository.Models;
using Microservices.Shared.Mocks;
using Moq;

namespace Email.Logic.Tests.QueryHandlers.GetEmailsBaseQueryHandler
{
    internal class GetEmailsBaseQueryHandlerTestsContext
    {
        private readonly MockEmailRepository _mockEmailRepository;
        private readonly Mock<IGetEmailsSentToRecipientQueryHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Email.Logic.QueryHandlers.GetEmailsSentToRecipientQueryHandler> _mockLogger;

        internal Email.Logic.QueryHandlers.GetEmailsSentToRecipientQueryHandler Sut { get; }

        public GetEmailsBaseQueryHandlerTestsContext()
        {
            _mockEmailRepository = new();
            _mockMetrics = new();
            _mockLogger = new();

            Sut = new(_mockEmailRepository.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        internal GetEmailsBaseQueryHandlerTestsContext WithData(IEnumerable<SentEmail> data)
        {
            foreach (var email in data)
                _mockEmailRepository.Emails.Add(email);
            return this;
        }

        internal GetEmailsBaseQueryHandlerTestsContext WithException()
        {
            _mockEmailRepository.Setup(_ => _.GetEmailsSentToRecipientAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();
            return this;
        }

        internal GetEmailsBaseQueryHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal GetEmailsBaseQueryHandlerTestsContext AssertMetricsLoadTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordLoadTime(It.IsAny<double>()), Times.Once);
            return this;
        }
    }
}
