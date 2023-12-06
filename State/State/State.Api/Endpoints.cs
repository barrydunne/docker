using AspNet.KickStarter;
using Microservices.Shared.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using State.Api.HttpHandlers;
using System.Diagnostics.CodeAnalysis;

namespace State.Api;

/// <summary>
/// Maps the endpoints used in the API.
/// </summary>
internal static class Endpoints
{
    /// <summary>
    /// Maps the endpoints used in the API.
    /// </summary>
    /// <param name="app">The application to add the routes to.</param>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:Parameters should be on same line or separate lines", Justification = "Readability")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Readability")]
    internal static void Map(WebApplication app)
    {
        app.MapPost("/state/jobcreated", "JobCreated", "Handle a JobCreatedEvent message.",
            async (StateHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JobCreatedEvent request)
                => await handler.JobCreatedAsync(request));

        app.MapPost("/state/geocodingcomplete", "GeocodingComplete", "Handle a GeocodingCompleteEvent message.",
            async (StateHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] GeocodingCompleteEvent request)
                => await handler.GeocodingCompleteAsync(request));

        app.MapPost("/state/directionscomplete", "DirectionsComplete", "Handle a DirectionsCompleteEvent message.",
            async (StateHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] DirectionsCompleteEvent request)
                => await handler.DirectionsCompleteAsync(request));

        app.MapPost("/state/weathercomplete", "WeatherComplete", "Handle a WeatherCompleteEvent message.",
            async (StateHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] WeatherCompleteEvent request)
                => await handler.WeatherCompleteAsync(request));

        app.MapPost("/state/imagingcomplete", "ImagingComplete", "Handle a ImagingCompleteEvent message.",
            async (StateHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ImagingCompleteEvent request)
                => await handler.ImagingCompleteAsync(request));
    }
}
