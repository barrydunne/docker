using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Logic.Commands;
using State.Logic.Metrics;
using State.Repository;
using System.Diagnostics;

namespace State.Logic.CommandHandlers
{
    /// <summary>
    /// The handler for the <see cref="UpdateDirectionsResultCommand"/> command.
    /// </summary>
    internal class UpdateDirectionsResultCommandHandler : BaseUpdateResultCommandHandler, ICommandHandler<UpdateDirectionsResultCommand>
    {
        private readonly IUpdateDirectionsResultCommandHandlerMetrics _metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDirectionsResultCommandHandler"/> class.
        /// </summary>
        /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        /// <param name="metrics">The metrics provider for this handler.</param>
        /// <param name="logger">The logger to write to.</param>
        public UpdateDirectionsResultCommandHandler(IJobRepository jobRepository, IMediator mediator, IUpdateDirectionsResultCommandHandlerMetrics metrics, ILogger<UpdateDirectionsResultCommandHandler> logger) : base(jobRepository, mediator, logger)
            => _metrics = metrics;

        /// <inheritdoc/>
        public async Task<Result> Handle(UpdateDirectionsResultCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateDirectionsResultCommand), command.JobId);
            _metrics.IncrementCount();

            var stopwatch = Stopwatch.StartNew();

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure(guardResult.Error);

            try
            {
                await _jobRepository.UpdateJobStatusAsync(command.JobId, command.Directions.IsSuccessful, command.Directions, cancellationToken);
                _metrics.RecordUpdateTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                // Check if the job is complete after any individual task completes
                var completed = await IsJobCompletedAsync(command.JobId, cancellationToken);
                if (completed)
                    _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }

        private Result PerformGuardChecks(UpdateDirectionsResultCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(UpdateDirectionsResultCommand.JobId));
                Guard.Against.Null(command.Directions, nameof(UpdateDirectionsResultCommand.Directions));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to validate command properties. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }
    }
}
