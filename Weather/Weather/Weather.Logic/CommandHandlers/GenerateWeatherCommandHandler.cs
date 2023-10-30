﻿using System.Diagnostics;
using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Weather.Logic.Commands;
using Weather.Logic.Metrics;
using Weather.Logic.Queries;

namespace Weather.Logic.CommandHandlers
{
    /// <summary>
    /// The handler for the <see cref="GenerateWeatherCommand"/> command.
    /// </summary>
    internal class GenerateWeatherCommandHandler : ICommandHandler<GenerateWeatherCommand>
    {
        private readonly IQueue<WeatherCompleteEvent> _completeQueue;
        private readonly IMediator _mediator;
        private readonly IGenerateWeatherCommandHandlerMetrics _metrics;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateWeatherCommandHandler"/> class.
        /// </summary>
        /// <param name="completeQueue">The queue for publishing <see cref="WeatherCompleteEvent"/> events to.</param>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        /// <param name="metrics">The metrics provider for this handler.</param>
        /// <param name="logger">The logger to write to.</param>
        public GenerateWeatherCommandHandler(IQueue<WeatherCompleteEvent> completeQueue, IMediator mediator, IGenerateWeatherCommandHandlerMetrics metrics, ILogger<GenerateWeatherCommandHandler> logger)
        {
            _completeQueue = completeQueue;
            _mediator = mediator;
            _metrics = metrics;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Result> Handle(GenerateWeatherCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GenerateWeatherCommand), command.JobId);
            _metrics.IncrementCount();

            var stopwatch = Stopwatch.StartNew();

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure<Guid>(guardResult.Error);

            try
            {
                var weatherTask = _mediator.Send(new GetWeatherQuery(command.JobId, command.Coordinates), cancellationToken);
                try
                {
                    await weatherTask;
                }
                catch (Exception ex)
                {
                    // This should not happen due to exception handling in the query handler that will return a failed Result instead of throwing an exception. Added for safety.
                    _logger.LogError(ex, "Unexpected error waiting for weather. [{CorrelationId}]", command.JobId);
                }
                var weatherResult = weatherTask.GetTaskResult();
                _metrics.RecordWeatherTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                var completeEvent = CreateWeatherCompleteEvent(command, weatherResult);
                await PublishEventAsync(command, completeEvent, cancellationToken);
                _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weather. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }

        private Result PerformGuardChecks(GenerateWeatherCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(GenerateWeatherCommand.JobId));
                Guard.Against.Null(command.Coordinates, nameof(GenerateWeatherCommand.Coordinates));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to validate command properties. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }

        private WeatherCompleteEvent CreateWeatherCompleteEvent(GenerateWeatherCommand command, Result<WeatherForecast> result)
        {
            var weather = result.IsSuccess
                ? result.Value
                : new WeatherForecast(false, null, result.Error);
            return new(command.JobId, weather);
        }

        private async Task PublishEventAsync(GenerateWeatherCommand command, WeatherCompleteEvent completeEvent, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Publishing weather complete event. {Event} [{CorrelationId}]", completeEvent, command.JobId);
            await _completeQueue.PublishAsync(completeEvent, cancellationToken);
            _logger.LogInformation("Published weather complete event. [{CorrelationId}]", command.JobId);
        }
    }
}
