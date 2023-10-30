using Email.Api.Models;
using Email.Logic.Queries;
using FluentValidation;
using MediatR;

namespace Email.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to email.
    /// </summary>
    public class SearchHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public SearchHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets all emails sent to a specific recipient.
        /// </summary>
        /// <param name="request">The search parameters.</param>
        /// <param name="validator">The input validator.</param>
        /// <returns>A <see cref="SentEmailsResponse"/> containing the results.</returns>
        internal async Task<IResult> GetEmailsSentToRecipientAsync(GetEmailsSentToRecipientRequest request, IValidator<GetEmailsSentToRecipientRequest> validator)
        {
            // Input validation
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var results = await _mediator.Send(new GetEmailsSentToRecipientQuery(request.RecipientEmail, request.PageSize, request.PageNumber));
            return Results.Ok(new SentEmailsResponse(results));
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
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var results = await _mediator.Send(new GetEmailsSentBetweenTimesQuery(DateTimeOffset.FromUnixTimeSeconds(request.FromUnixSeconds).UtcDateTime, DateTimeOffset.FromUnixTimeSeconds(request.ToUnixSeconds).UtcDateTime, request.PageSize, request.PageNumber));
            return Results.Ok(new SentEmailsResponse(results));
        }
    }
}
