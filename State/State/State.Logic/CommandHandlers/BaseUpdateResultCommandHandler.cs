using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microsoft.Extensions.Logging;
using State.Logic.Commands;
using State.Repository;

namespace State.Logic.CommandHandlers
{
    /// <summary>
    /// Provides common functionality for the UpdateXXXResultCommandHandlers.
    /// </summary>
    internal abstract class BaseUpdateResultCommandHandler
    {
        /// <summary>
        /// The repository for saving and retrieving jobs.
        /// </summary>
        protected readonly IJobRepository _jobRepository;

        /// <summary>
        /// The mediator to send commands and queries to.
        /// </summary>
        protected readonly IMediator _mediator;

        /// <summary>
        /// The logger to write to.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUpdateResultCommandHandler"/> class.
        /// </summary>
        /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        /// <param name="logger">The logger to write to.</param>
        protected BaseUpdateResultCommandHandler(IJobRepository jobRepository, IMediator mediator, ILogger logger)
        {
            _jobRepository = jobRepository;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Checks if all tasks for the job have completed and dispatches an NotifyJobStatusCommand command if they have.
        /// </summary>
        /// <param name="jobId">The id of the job to check.</param>
        /// <param name="cancellationToken">The token to cancel the operation.</param>
        /// <returns>True if all tasks are complete, false otherwise.</returns>
        protected async Task<bool> IsJobCompletedAsync(Guid jobId, CancellationToken cancellationToken)
        {
            var completionStatus = await _jobRepository.GetJobCompletionStatusAsync(jobId, cancellationToken);
            if (completionStatus is not null)
            {
                var job = await _jobRepository.GetJobIdByIdAsync(jobId, cancellationToken);
                Result result;
                if (completionStatus == true)
                {
                    _logger.LogInformation("Job has finished processing with a successful outcome. [{CorrelationId}]", jobId);
                    result = await _mediator.Send(new NotifyJobStatusUpdateCommand(jobId, JobStatus.Complete, null), cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Job has finished processing with an unsuccessful outcome. [{CorrelationId}]", jobId);
                    var errors = new[] { job!.Directions!.Error, job.WeatherForecast!.Error, job.ImagingResult!.Error };
                    result = await _mediator.Send(new NotifyJobStatusUpdateCommand(jobId, JobStatus.Complete, string.Join(" ", errors.Where(_ => !string.IsNullOrWhiteSpace(_)))), cancellationToken);
                }
                if (result.IsSuccess)
                    result = await _mediator.Send(new NotifyProcessingCompleteCommand(jobId, completionStatus.Value, job!), cancellationToken);

                if (result.IsFailure)
                    throw new StateException("Failed to send notification.");
            }
            return completionStatus is not null;
        }
    }
}
