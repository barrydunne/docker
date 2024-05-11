using Email.Api.Models;
using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Email.Application.Queries.GetEmailsSentToRecipient;
using FluentValidation;
using Mapster;
using MediatR;

namespace Email.Api.HttpHandlers;

/// <summary>
/// The handler for requests relating to email.
/// </summary>
public class SearchHandler
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchHandler"/> class.
    /// </summary>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    public SearchHandler(ISender mediator) => _mediator = mediator;

    /// <summary>
    /// Gets all emails sent to a specific recipient.
    /// </summary>
    /// <param name="request">The search parameters.</param>
    /// <param name="validator">The input validator.</param>
    /// <returns>A <see cref="SentEmailsResponse"/> containing the results.</returns>
    internal async Task<IResult> GetEmailsSentToRecipientAsync(GetEmailsSentToRecipientRequest request, IValidator<GetEmailsSentToRecipientRequest> validator)
    {
        // Input validation
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await _mediator.Send(request.Adapt<GetEmailsSentToRecipientQuery>());
        return result.Match(
            success => Results.Ok(success.Adapt<SentEmailsResponse>()),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Gets all emails sent between specific times.
    /// </summary>
    /// <param name="request">The search parameters.</param>
    /// <param name="validator">The input validator.</param>
    /// <returns>A <see cref="SentEmailsResponse"/> containing the results.</returns>
    internal async Task<IResult> GetEmailsSentBetweenTimesAsync(GetEmailsSentBetweenTimesRequest request, IValidator<GetEmailsSentBetweenTimesRequest> validator)
    {
        // Input validation
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await _mediator.Send(request.Adapt<GetEmailsSentBetweenTimesQuery>());
        return result.Match(
            success => Results.Ok(success.Adapt<SentEmailsResponse>()),
            error => error.AsHttpResult());
    }
}
