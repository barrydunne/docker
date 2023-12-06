using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using State.Application.Commands.NotifyProcessingComplete;

namespace State.Application.Tests.Commands.NotifyProcessingComplete;

internal class NotifyProcessingCompleteCommandHandlerTestsContext
{
    private readonly MockQueue<ProcessingCompleteEvent> _mockQueue;
    private readonly Mock<INotifyProcessingCompleteCommandHandlerMetrics> _mockMetrics;
    private readonly MockLogger<NotifyProcessingCompleteCommandHandler> _mockLogger;

    internal NotifyProcessingCompleteCommandHandler Sut { get; }

    public NotifyProcessingCompleteCommandHandlerTestsContext()
    {
        _mockQueue = new();
        _mockMetrics = new();
        _mockLogger = new();

        Sut = new(_mockQueue.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext WithPublishException()
    {
        _mockQueue.Setup(_ => _.PublishAsync(It.IsAny<ProcessingCompleteEvent>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();
        return this;
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext AssertEventPublished(NotifyProcessingCompleteCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == command.JobId
            && _.Directions == command.Job.Directions
            && _.Weather == command.Job.WeatherForecast
            && _.Imaging == command.Job.ImagingResult);
        Assert.That(published, Is.Not.Null);
        return this;
    }
}
