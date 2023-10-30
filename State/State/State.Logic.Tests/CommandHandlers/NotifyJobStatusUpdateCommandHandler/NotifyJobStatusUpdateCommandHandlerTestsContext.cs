using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using State.Logic.Commands;
using State.Logic.Metrics;

namespace State.Logic.Tests.CommandHandlers.NotifyJobStatusUpdateCommandHandler
{
    internal class NotifyJobStatusUpdateCommandHandlerTestsContext
    {
        private readonly MockQueue<JobStatusUpdateEvent> _mockQueue;
        private readonly Mock<INotifyJobStatusUpdateCommandHandlerMetrics> _mockMetrics;
        private readonly MockLogger<State.Logic.CommandHandlers.NotifyJobStatusUpdateCommandHandler> _mockLogger;

        internal State.Logic.CommandHandlers.NotifyJobStatusUpdateCommandHandler Sut { get; }

        public NotifyJobStatusUpdateCommandHandlerTestsContext()
        {
            _mockQueue = new();
            _mockMetrics = new();
            _mockLogger = new();

            Sut = new(_mockQueue.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        internal NotifyJobStatusUpdateCommandHandlerTestsContext WithPublishException()
        {
            _mockQueue.Setup(_ => _.PublishAsync(It.IsAny<JobStatusUpdateEvent>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();
            return this;
        }

        internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertMetricsGuardTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertEventPublished(NotifyJobStatusUpdateCommand command)
        {
            var published = _mockQueue.Messages.FirstOrDefault(_
                => (_.JobId == command.JobId)
                && (_.Status == command.Status)
                && (_.Details == command.Details));
            Assert.That(published, Is.Not.Null);
            return this;
        }
    }
}
