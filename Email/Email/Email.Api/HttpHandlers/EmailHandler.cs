using Email.Logic.Commands;
using MediatR;
using Microservices.Shared.Events;

namespace Email.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to email.
    /// </summary>
    public class EmailHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public EmailHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Handle a ProcessingCompleteEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> ProcessingCompleteAsync(ProcessingCompleteEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new SendEmailCommand(message.JobId, message.Email, message.StartingAddress, message.DestinationAddress, message.Directions, message.Weather, message.Imaging));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }
    }
}
