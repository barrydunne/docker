using AspNet.KickStarter;
using AspNet.KickStarter.HttpHandlers;
using Directions.Api.HttpHandlers;
using Microservices.Shared.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics.CodeAnalysis;

namespace Directions.Api
{
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
            app.MapGet<string>("/health/status", "GetHealthStatus", "Check API health.",
                (HealthHandler handler) => handler.GetStatus());

            app.MapGet<string>("/health/version", "GetVersion", "Get the API version.",
                async (HealthHandler handler) => await handler.GetVersionAsync());

            app.MapPost("/directions/locationsready", "LocationsReady", "Handle a LocationsReadyEvent message.",
                async (DirectionsHandler handler,
                       [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] LocationsReadyEvent request)
                    => await handler.LocationsReadyAsync(request));
        }
    }
}
