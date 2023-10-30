using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Mocks;
using Moq;
using State.Logic.Commands;
using State.Logic.Metrics;
using State.Logic.Tests.Mocks;
using System.Collections.Concurrent;

namespace State.Logic.Tests.CommandHandlers.CreateJobCommandHandler
{
    internal class CreateJobCommandHandlerTestsContext
    {
        private readonly MockJobRepository _mockJobRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ICreateJobCommandHandlerMetrics> _mockMetrics;
        private readonly MockLogger<State.Logic.CommandHandlers.CreateJobCommandHandler> _mockLogger;
        private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
        private readonly ConcurrentBag<Guid> _invalidJobIds;
        private readonly ConcurrentBag<Guid> _exceptionJobIds;

        internal State.Logic.CommandHandlers.CreateJobCommandHandler Sut { get; }

        public CreateJobCommandHandlerTestsContext()
        {
            _mockJobRepository = new();
            _mockMetrics = new();
            _mockLogger = new();
            _notifyJobStatusUpdateCommands = new();
            _invalidJobIds = new();
            _exceptionJobIds = new();

            _mockMediator = new();
            _mockMediator.Setup(_ => _.Send(It.IsAny<NotifyJobStatusUpdateCommand>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Result> command, CancellationToken _) => _notifyJobStatusUpdateCommands.Add((NotifyJobStatusUpdateCommand)command))
                .Returns((IRequest<Result> command, CancellationToken _) => NotifyJobStatusUpdate((NotifyJobStatusUpdateCommand)command));

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

        internal CreateJobCommandHandlerTestsContext WithSendException(CreateJobCommand command)
        {
            _exceptionJobIds.Add(command.JobId);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext WithSendFailure(CreateJobCommand command)
        {
            _invalidJobIds.Add(command.JobId);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext AssertMetricsGuardTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext AssertMetricsSaveTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordSaveTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext AssertJobSaved(Guid jobId)
        {
            var job = _mockJobRepository.GetJob(jobId);
            Assert.That(job, Is.Not.Null);
            return this;
        }

        internal CreateJobCommandHandlerTestsContext AssertNotifyJobStatusUpdateCommandSent(Guid jobId)
        {
            Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_ => _.JobId == jobId));
            return this;
        }
    }
}
