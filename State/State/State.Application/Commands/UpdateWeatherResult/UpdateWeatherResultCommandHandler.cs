using AspNet.KickStarter.CQRS.Abstractions.Commands;
using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Application.Repositories;
using System.Diagnostics;

namespace State.Application.Commands.UpdateWeatherResult;

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
    public UpdateWeatherResultCommandHandler(IJobRepository jobRepository, ISender mediator, IUpdateWeatherResultCommandHandlerMetrics metrics, ILogger<UpdateWeatherResultCommandHandler> logger) : base(jobRepository, mediator, logger)
        => _metrics = metrics;

    /// <inheritdoc/>
    public async Task<Result> Handle(UpdateWeatherResultCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateWeatherResultCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

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
            return ex;
        }
    }
}
