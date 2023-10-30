using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using State.Logic.Commands;
using State.Logic.Metrics;
using State.Logic.Tests.Mocks;
using State.Repository.Models;
using System.Collections.Concurrent;

namespace State.Logic.Tests.CommandHandlers.UpdateDirectionsResultCommandHandler
{
    internal class UpdateDirectionsResultCommandHandlerTestsContext
    {
        private readonly MockJobRepository _mockJobRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IUpdateDirectionsResultCommandHandlerMetrics> _mockMetrics;
        private readonly MockLogger<State.Logic.CommandHandlers.UpdateDirectionsResultCommandHandler> _mockLogger;
        private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
        private readonly ConcurrentBag<NotifyProcessingCompleteCommand> _notifyProcessingCompleteCommands;
        private readonly ConcurrentBag<Guid> _invalidJobIds;
        private readonly ConcurrentBag<Guid> _exceptionJobIds;

        internal State.Logic.CommandHandlers.UpdateDirectionsResultCommandHandler Sut { get; }

        public UpdateDirectionsResultCommandHandlerTestsContext()
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
                    ? Result.Failure(GetError(command))
                    : Result.Success();
        });

        internal string GetError(CreateJobCommand command) => $"Invalid: {command.JobId}";
        internal string GetError(NotifyJobStatusUpdateCommand command) => $"Invalid: {command.JobId}";

        internal Job? GetJob(Guid jobId) => _mockJobRepository.GetJob(jobId);

        internal UpdateDirectionsResultCommandHandlerTestsContext WithJob(Job job)
        {
            _mockJobRepository.AddJob(job);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext WithJobRepositoryException()
        {
            _mockJobRepository.Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<Directions>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext WithSendException(UpdateDirectionsResultCommand command)
        {
            _exceptionJobIds.Add(command.JobId);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext WithSendFailure(UpdateDirectionsResultCommand command)
        {
            _invalidJobIds.Add(command.JobId);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext AssertMetricsGuardTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext AssertMetricsPublishTimeNotRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Never);
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext AssertNotifyJobStatusUpdateCommandSent(Guid jobId, bool error)
        {
            Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_ 
                => (_.JobId == jobId)
                && (string.IsNullOrWhiteSpace(_.Details) == !error)));
            return this;
        }

        internal UpdateDirectionsResultCommandHandlerTestsContext AssertNotifyProcessingCompleteCommandSent(Guid jobId)
        {
            Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_ => _.JobId == jobId));
            return this;
        }
    }
}
