using Mapster;
using MediatR;
using Microservices.Shared.Events;
using State.Application.Commands.CreateJob;
using State.Application.Commands.UpdateDirectionsResult;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Commands.UpdateImagingResult;
using State.Application.Commands.UpdateWeatherResult;

namespace State.Api.HttpHandlers;

/// <summary>
/// The handler for requests relating to state.
/// </summary>
public class StateHandler
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateHandler"/> class.
    /// </summary>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    public StateHandler(ISender mediator) => _mediator = mediator;

    /// <summary>
    /// Handle a JobCreatedEvent message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>OK or Problem.</returns>
    internal async Task<IResult> JobCreatedAsync(JobCreatedEvent message)
    {
        // No input validation is required as the API is just for development/testing purposes.
        var result = await _mediator.Send(message.Adapt<CreateJobCommand>());
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Handle a GeocodingCompleteEvent message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>OK or Problem.</returns>
    internal async Task<IResult> GeocodingCompleteAsync(GeocodingCompleteEvent message)
    {
        // No input validation is required as the API is just for development/testing purposes.
        var result = await _mediator.Send(message.Adapt<UpdateGeocodingResultCommand>());
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Handle a DirectionsCompleteEvent message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>OK or Problem.</returns>
    internal async Task<IResult> DirectionsCompleteAsync(DirectionsCompleteEvent message)
    {
        // No input validation is required as the API is just for development/testing purposes.
        var result = await _mediator.Send(message.Adapt<UpdateDirectionsResultCommand>());
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Handle a WeatherCompleteEvent message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>OK or Problem.</returns>
    internal async Task<IResult> WeatherCompleteAsync(WeatherCompleteEvent message)
    {
        // No input validation is required as the API is just for development/testing purposes.
        var result = await _mediator.Send(message.Adapt<UpdateWeatherResultCommand>());
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Handle a ImagingCompleteEvent message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>OK or Problem.</returns>
    internal async Task<IResult> ImagingCompleteAsync(ImagingCompleteEvent message)
    {
        // No input validation is required as the API is just for development/testing purposes.
        var result = await _mediator.Send(message.Adapt<UpdateImagingResultCommand>());
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }
}
