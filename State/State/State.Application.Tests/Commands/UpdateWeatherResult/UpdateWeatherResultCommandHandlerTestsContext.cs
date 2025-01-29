using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.Mocks;
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
    private readonly ISender _mockMediator;
    private readonly IUpdateWeatherResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateWeatherResultCommandHandler> _mockLogger;
    private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
    private readonly ConcurrentBag<NotifyProcessingCompleteCommand> _notifyProcessingCompleteCommands;
    private readonly ConcurrentBag<Guid> _invalidJobIds;
    private readonly ConcurrentBag<Guid> _exceptionJobIds;

    internal UpdateWeatherResultCommandHandler Sut { get; }

    public UpdateWeatherResultCommandHandlerTestsContext()
    {
        _mockJobRepository = new();
        _mockMetrics = Substitute.For<IUpdateWeatherResultCommandHandlerMetrics>();
        _mockLogger = new();
        _notifyJobStatusUpdateCommands = new();
        _notifyProcessingCompleteCommands = new();
        _invalidJobIds = new();
        _exceptionJobIds = new();
        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<NotifyJobStatusUpdateCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => NotifyJobStatusUpdate((NotifyJobStatusUpdateCommand)callInfo.ArgAt<IRequest<Result>>(0)))
            .AndDoes(callInfo => _notifyJobStatusUpdateCommands.Add((NotifyJobStatusUpdateCommand)callInfo.ArgAt<IRequest<Result>>(0)));
        _mockMediator
            .Send(Arg.Any<NotifyProcessingCompleteCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Result.Success())
            .AndDoes(callInfo => _notifyProcessingCompleteCommands.Add((NotifyProcessingCompleteCommand)callInfo.ArgAt<IRequest<Result>>(0)));

        Sut = new(_mockJobRepository, _mockMediator, _mockMetrics, _mockLogger);
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
        _mockJobRepository.WithWriteException();
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertMetricsPublishTimeNotRecorded()
    {
        _mockMetrics.Received(0).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertNotifyJobStatusUpdateCommandSent(Guid jobId, bool error)
    {
        _notifyJobStatusUpdateCommands.Where(_ => _.JobId == jobId && string.IsNullOrWhiteSpace(_.Details) == !error).ShouldHaveSingleItem();
        return this;
    }

    internal UpdateWeatherResultCommandHandlerTestsContext AssertNotifyProcessingCompleteCommandSent(Guid jobId)
    {
        _notifyJobStatusUpdateCommands.Where(_ => _.JobId == jobId).ShouldHaveSingleItem();
        return this;
    }
}
