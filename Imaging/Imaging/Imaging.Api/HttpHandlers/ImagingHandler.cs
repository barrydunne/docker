using Imaging.Logic.Commands;
using MediatR;
using Microservices.Shared.Events;

namespace Imaging.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to imaging.
    /// </summary>
    public class ImagingHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public ImagingHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Handle a LocationsReadyEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> LocationsReadyAsync(LocationsReadyEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new SaveImageCommand(message.JobId, message.DestinationAddress, message.DestinationCoordinates));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }
    }
}
