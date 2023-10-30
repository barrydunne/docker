using MediatR;
using Microservices.Shared.Events;
using State.Logic.Commands;

namespace State.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to state.
    /// </summary>
    public class StateHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public StateHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Handle a JobCreatedEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> JobCreatedAsync(JobCreatedEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new CreateJobCommand(message.JobId, message.StartingAddress, message.DestinationAddress, message.Email));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }

        /// <summary>
        /// Handle a GeocodingCompleteEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> GeocodingCompleteAsync(GeocodingCompleteEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new UpdateGeocodingResultCommand(message.JobId, message.StartingCoordinates, message.DestinationCoordinates));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }

        /// <summary>
        /// Handle a DirectionsCompleteEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> DirectionsCompleteAsync(DirectionsCompleteEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new UpdateDirectionsResultCommand(message.JobId, message.Directions));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }

        /// <summary>
        /// Handle a WeatherCompleteEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> WeatherCompleteAsync(WeatherCompleteEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new UpdateWeatherResultCommand(message.JobId, message.Weather));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }

        /// <summary>
        /// Handle a ImagingCompleteEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> ImagingCompleteAsync(ImagingCompleteEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new UpdateImagingResultCommand(message.JobId, message.Imaging));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }
    }
}
