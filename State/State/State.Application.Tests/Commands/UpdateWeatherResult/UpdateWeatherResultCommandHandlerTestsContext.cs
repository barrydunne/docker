using AspNet.KickStarter.CQRS;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using State.Application.Commands.CreateJob;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Commands.NotifyProcessingComplete;
using State.Application.Commands.UpdateWeatherResult;
using State.Application.Models;
using State.Application.Tests.Mocks;
using System.Collections.Concurrent;

namespace State.Application.Tests.Commands.UpdateWeatherResult;

internal class UpdateWeatherResultCommandHandlerTestsContext
{
    private readonly MockJobRepository _mockJobRepository;
    private readonly Mock<ISender> _mockMediator;
    private readonly Mock<IUpdateWeatherResultCommandHandlerMetrics> _mockMetrics;
    private readonly MockLogger<UpdateWeatherResultCommandHandler> _mockLogger;
    private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
    private readonly ConcurrentBag<NotifyProcessingCompleteCommand> _notifyProcessingCompleteCommands;
    private readonly ConcurrentBag<Guid> _invalidJobIds;
    private readonly ConcurrentBag<Guid> _exceptionJobIds;

    internal UpdateWeatherResultCommandHandler Sut { get; }

    public UpdateWeatherResultCommandHandlerTestsContext()
    {
        _mockJobRepository = new();
        _mockMetrics = new();
        _mockLogger = new();
        _notifyJobStatusUpdateCommands = new();
        _notifyProcessingCompleteCommands = new();
        _invalidJobIds = new();
        _exceptionJobIds = new();

        _mockMediator = new();
        _mockMediator.Setup(_ => _.Send(It.IsAny<NotifyJobStatusUpdateCommand>(), It.IsAny<CancellationToken>()))
            .Callback((IRequest<Result> command, CancellationToken _) => _notifyJobStatusUpdateCommands.Add((NotifyJobStatusUpdateCommand)command))
            .Returns((IRequest<Result> command, CancellationToken _) => NotifyJobStatusUpdate((NotifyJobStatusUpdateCommand)command));
        _mockMediator.Setup(_ => _.Send(It.IsAny<NotifyProcessingCompleteCommand>(), It.IsAny<CancellationToken>()))
            .Callback((IRequest<Result> command, CancellationToken _) => _notifyProcessingCompleteCommands.Add((NotifyProcessingCompleteCommand)command))
            .ReturnsAsync((IRequest<Result> command, CancellationToken _) => Result.Success());

        Sut = new(_mockJobRepository.Object, _mockMediator.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    private Task<Result> NotifyJobStatusUpdate(NotifyJobStatusUpdateCommand command) => Task.Run(() =>
    {
        return _exceptionJobIds.Contains(command.JobId)
            ? throw new InvalidDataException(GetError(command))
            : _invalidJobIds.Contains(command.JobId)
                ? Result.FromError(GetError(command))
                : Result.Success();
    });

    internal string GetError(CreateJobCommand command) => $"Invalid: {command.JobId}";
    internal string GetError(NotifyJobStatusUpdateCommand command) => $"Invalid: {command.JobId}";

    internal Job? GetJob(Guid jobId) => _mockJobRepository.GetJob(jobId);

    internal UpdateWeatherResultCommandHandlerTestsContext WithJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext WithJobRepositoryException()
    {
        _mockJobRepository.Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<WeatherForecast>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext WithSendException(UpdateWeatherResultCommand command)
    {
        _exceptionJobIds.Add(command.JobId);
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext WithSendFailure(UpdateWeatherResultCommand command)
    {
        _invalidJobIds.Add(command.JobId);
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertMetricsPublishTimeNotRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Never);
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertNotifyJobStatusUpdateCommandSent(Guid jobId, bool error)
    {
        Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_
            => _.JobId == jobId
            && string.IsNullOrWhiteSpace(_.Details) == !error));
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertNotifyProcessingCompleteCommandSent(Guid jobId)
    {
        Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_ => _.JobId == jobId));
        return this;
    }
}
