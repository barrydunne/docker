﻿using Geocoding.Logic.Commands;
using MediatR;
using Microservices.Shared.Events;

namespace Geocoding.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to geocoding.
    /// </summary>
    public class GeocodingHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodingHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public GeocodingHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Handle a JobCreatedEvent message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> JobCreatedAsync(JobCreatedEvent message)
        {
            // No input validation is required as the API is just for development/testing purposes.
            var result = await _mediator.Send(new GeocodeAddressesCommand(message.JobId, message.StartingAddress, message.DestinationAddress));
            if (result.IsSuccess)
                return Results.Ok();
            return Results.Problem(result.Error);
        }
    }
}
