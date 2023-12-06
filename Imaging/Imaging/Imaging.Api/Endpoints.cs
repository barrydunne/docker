using AspNet.KickStarter;
using Imaging.Api.HttpHandlers;
using Microservices.Shared.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics.CodeAnalysis;

namespace Imaging.Api;

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
        app.MapPost("/imaging/locationsready", "LocationsReady", "Handle a LocationsReadyEvent message.",
            async (ImagingHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] LocationsReadyEvent request)
                => await handler.LocationsReadyAsync(request));
    }
}
