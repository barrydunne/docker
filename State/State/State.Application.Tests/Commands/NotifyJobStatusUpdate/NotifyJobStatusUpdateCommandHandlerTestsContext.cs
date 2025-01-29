using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using State.Application.Commands.NotifyJobStatusUpdate;

namespace State.Application.Tests.Commands.NotifyJobStatusUpdate;

internal class NotifyJobStatusUpdateCommandHandlerTestsContext
{
    private readonly MockQueue<JobStatusUpdateEvent> _mockQueue;
    private readonly INotifyJobStatusUpdateCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<NotifyJobStatusUpdateCommandHandler> _mockLogger;

    internal NotifyJobStatusUpdateCommandHandler Sut { get; }

    public NotifyJobStatusUpdateCommandHandlerTestsContext()
    {
        _mockQueue = new();
        _mockMetrics = Substitute.For<INotifyJobStatusUpdateCommandHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockQueue, _mockMetrics, _mockLogger);
    }

    internal NotifyJobStatusUpdateCommandHandlerTestsContext WithPublishException()
    {
        _mockQueue.WithPublishException();
        return this;
    }

    internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal NotifyJobStatusUpdateCommandHandlerTestsContext AssertEventPublished(NotifyJobStatusUpdateCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == command.JobId
            && _.Status == command.Status
            && _.Details == command.Details);
        published.ShouldNotBeNull();
        return this;
    }
}
