using AspNet.KickStarter.FunctionalResult.Extensions;
using FluentValidation;
using Mapster;
using MediatR;
using PublicApi.Api.Models;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Queries.GetJobStatus;

namespace PublicApi.Api.HttpHandlers;

/// <summary>
/// The handler for requests relating to jobs.
/// </summary>
public class JobHandler
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobHandler"/> class.
    /// </summary>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    public JobHandler(ISender mediator) => _mediator = mediator;

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
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await _mediator.Send((idempotencyKey, request).Adapt<CreateJobCommand>());
        return result.Match(
            success => Results.Ok(success.Adapt<CreateJobResponse>()),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Get the status of an existing job.
    /// </summary>
    /// <param name="jobId">The id of the existing job.</param>
    /// <returns>The status, or 404 if unknown.</returns>
    internal async Task<IResult> GetJobStatusAsync(Guid jobId)
    {
        var result = await _mediator.Send(jobId.Adapt<GetJobStatusQuery>());
        return result.Match(
            success => success is null
                        ? Results.NotFound()
                        : Results.Ok(success.Adapt<GetJobStatusResponse>()),
            error => error.AsHttpResult());
    }
}
