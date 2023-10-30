using FluentValidation;
using MediatR;
using PublicApi.Api.Models;
using PublicApi.Logic.Commands;
using PublicApi.Logic.Queries;

namespace PublicApi.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to jobs.
    /// </summary>
    public class JobHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public JobHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Create a new processing job.
        /// </summary>
        /// <param name="idempotencyKey">The request idempotency key used to prevent creation of duplicate jobs.</param>
        /// <param name="request">The job creation request.</param>
        /// <param name="validator">The input validator.</param>
        /// <returns>A <see cref="CreateJobResponse"/> containing the new job id.</returns>
        internal async Task<IResult> CreateJobAsync(string? idempotencyKey, CreateJobRequest request, IValidator<CreateJobRequest> validator)
        {
            // Idempotency key will be enforced by nginx, but add check here for safety
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return Results.BadRequest("Idempotency key header is required");

            // Input validation
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var result = await _mediator.Send(new CreateJobCommand(idempotencyKey, request.StartingAddress, request.DestinationAddress, request.Email));
            if (result.IsSuccess)
                return Results.Ok(new CreateJobResponse { JobId = result.Value });
            return Results.Problem(result.Error);
        }

        /// <summary>
        /// Get the status of an existing job.
        /// </summary>
        /// <param name="jobId">The id of the existing job.</param>
        /// <returns>The status, or 404 if unknown.</returns>
        internal async Task<IResult> GetJobStatusAsync(Guid jobId)
        {
            var job = await _mediator.Send(new GetJobStatusQuery(jobId));
            if (job is null)
                return Results.NotFound();

            return Results.Ok(new GetJobStatusResponse { Status = job.Status, AdditionalInformation = job.AdditionalInformation });
        }
    }
}
