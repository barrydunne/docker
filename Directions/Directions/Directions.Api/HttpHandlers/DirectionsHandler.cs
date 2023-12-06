using Directions.Application.Commands.GenerateDirections;
using Mapster;
using MediatR;
using Microservices.Shared.Events;

namespace Directions.Api.HttpHandlers;

/// <summary>
/// The handler for requests relating to directions.
/// </summary>
public class DirectionsHandler
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionsHandler"/> class.
    /// </summary>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    public DirectionsHandler(ISender mediator) => _mediator = mediator;

    /// <summary>
    /// Handle a LocationsReadyEvent message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>OK or Problem.</returns>
    internal async Task<IResult> LocationsReadyAsync(LocationsReadyEvent message)
    {
        // No input validation is required as the API is just for development/testing purposes.
        var result = await _mediator.Send(message.Adapt<GenerateDirectionsCommand>());
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }
}
