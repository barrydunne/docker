using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using State.Application.Commands.NotifyProcessingComplete;

namespace State.Application.Tests.Commands.NotifyProcessingComplete;

internal class NotifyProcessingCompleteCommandHandlerTestsContext
{
    private readonly MockQueue<ProcessingCompleteEvent> _mockQueue;
    private readonly INotifyProcessingCompleteCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<NotifyProcessingCompleteCommandHandler> _mockLogger;

    internal NotifyProcessingCompleteCommandHandler Sut { get; }

    public NotifyProcessingCompleteCommandHandlerTestsContext()
    {
        _mockQueue = new();
        _mockMetrics = Substitute.For<INotifyProcessingCompleteCommandHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockQueue, _mockMetrics, _mockLogger);
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext WithPublishException()
    {
        _mockQueue.WithPublishException();
        return this;
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal NotifyProcessingCompleteCommandHandlerTestsContext AssertEventPublished(NotifyProcessingCompleteCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == command.JobId
            && _.Directions == command.Job.Directions
            && _.Weather == command.Job.WeatherForecast
            && _.Imaging == command.Job.ImagingResult);
        published.ShouldNotBeNull();
        return this;
    }
}
