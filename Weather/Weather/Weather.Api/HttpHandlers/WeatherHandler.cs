using MediatR;
using Microservices.Shared.Events;
using Weather.Logic.Commands;

namespace Weather.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to weather.
    /// </summary>
    public class WeatherHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public WeatherHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Handle a LocationsReadyEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> LocationsReadyAsync(LocationsReadyEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new GenerateWeatherCommand(message.JobId, message.DestinationCoordinates));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }
    }
}
