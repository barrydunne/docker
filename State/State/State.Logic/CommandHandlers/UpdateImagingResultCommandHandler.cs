﻿using Ardalis.GuardClauses;
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
    /// The handler for the <see cref="UpdateImagingResultCommand"/> command.
    /// </summary>
    internal class UpdateImagingResultCommandHandler : BaseUpdateResultCommandHandler, ICommandHandler<UpdateImagingResultCommand>
    {
        private readonly IUpdateImagingResultCommandHandlerMetrics _metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateImagingResultCommandHandler"/> class.
        /// </summary>
        /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        /// <param name="metrics">The metrics provider for this handler.</param>
        /// <param name="logger">The logger to write to.</param>
        public UpdateImagingResultCommandHandler(IJobRepository jobRepository, IMediator mediator, IUpdateImagingResultCommandHandlerMetrics metrics, ILogger<UpdateImagingResultCommandHandler> logger) : base(jobRepository, mediator, logger)
            => _metrics = metrics;

        /// <inheritdoc/>
        public async Task<Result> Handle(UpdateImagingResultCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateImagingResultCommand), command.JobId);
            _metrics.IncrementCount();

            var stopwatch = Stopwatch.StartNew();

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure(guardResult.Error);

            try
            {
                await _jobRepository.UpdateJobStatusAsync(command.JobId, command.Imaging.IsSuccessful, command.Imaging, cancellationToken);
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

        private Result PerformGuardChecks(UpdateImagingResultCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(UpdateImagingResultCommand.JobId));
                Guard.Against.Null(command.Imaging, nameof(UpdateImagingResultCommand.Imaging));
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
