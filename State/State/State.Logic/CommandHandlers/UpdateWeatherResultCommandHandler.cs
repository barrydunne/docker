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
    /// The handler for the <see cref="UpdateWeatherResultCommand"/> command.
    /// </summary>
    internal class UpdateWeatherResultCommandHandler : BaseUpdateResultCommandHandler, ICommandHandler<UpdateWeatherResultCommand>
    {
        private readonly IUpdateWeatherResultCommandHandlerMetrics _metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateWeatherResultCommandHandler"/> class.
        /// </summary>
        /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        /// <param name="metrics">The metrics provider for this handler.</param>
        /// <param name="logger">The logger to write to.</param>
        public UpdateWeatherResultCommandHandler(IJobRepository jobRepository, IMediator mediator, IUpdateWeatherResultCommandHandlerMetrics metrics, ILogger<UpdateWeatherResultCommandHandler> logger) : base(jobRepository, mediator, logger)
            => _metrics = metrics;

        /// <inheritdoc/>
        public async Task<Result> Handle(UpdateWeatherResultCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateWeatherResultCommand), command.JobId);
            _metrics.IncrementCount();

            var stopwatch = Stopwatch.StartNew();

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure(guardResult.Error);

            try
            {
                await _jobRepository.UpdateJobStatusAsync(command.JobId, command.Weather.IsSuccessful, command.Weather, cancellationToken);
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

        private Result PerformGuardChecks(UpdateWeatherResultCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(UpdateWeatherResultCommand.JobId));
                Guard.Against.Null(command.Weather, nameof(UpdateWeatherResultCommand.Weather));
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
