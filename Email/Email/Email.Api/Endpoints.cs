using AspNet.KickStarter;
using AspNet.KickStarter.HttpHandlers;
using Email.Api.HttpHandlers;
using Email.Api.Models;
using FluentValidation;
using Microservices.Shared.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics.CodeAnalysis;

namespace Email.Api
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

            app.MapPost("/email/processingcomplete", "ProcessingComplete", "Handle a ProcessingCompleteEvent message.",
                async (EmailHandler handler,
                       [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ProcessingCompleteEvent request)
                    => await handler.ProcessingCompleteAsync(request));

            app.MapGet<SentEmailsResponse>("/search/recipient/{email}", "GetEmailsSentToRecipient", "Gets all emails sent to a specific recipient.",
                async (SearchHandler handler,
                       HttpContext httpContext,
                       IValidator<GetEmailsSentToRecipientRequest> validator,
                       string email)
                    => await handler.GetEmailsSentToRecipientAsync(new GetEmailsSentToRecipientRequest(email, GetPageSize(httpContext), GetPageNumber(httpContext)), validator));

            app.MapGet<SentEmailsResponse>("/search/times/{fromUnixSeconds}/{toUnixSeconds}", "GetEmailsSentBetweenTimes", "Gets all emails sent between specific times.",
                async (SearchHandler handler,
                       HttpContext httpContext,
                       IValidator<GetEmailsSentBetweenTimesRequest> validator,
                       long fromUnixSeconds,
                       long toUnixSeconds)
                    => await handler.GetEmailsSentBetweenTimesAsync(new GetEmailsSentBetweenTimesRequest(fromUnixSeconds, toUnixSeconds, GetPageSize(httpContext), GetPageNumber(httpContext)), validator));
        }

        private static int GetPageSize(HttpContext httpContext) => GetIntParameter(httpContext, "pageSize", 20);
        private static int GetPageNumber(HttpContext httpContext) => GetIntParameter(httpContext, "pageNumber", 1);
        private static int GetIntParameter(HttpContext httpContext, string key, ushort defaultValue) => int.Parse(httpContext.Request.Query[key].FirstOrDefault() ?? defaultValue.ToString());
    }
}
