using AspNet.KickStarter;
using AspNet.KickStarter.HttpHandlers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PublicApi.Api.HttpHandlers;
using PublicApi.Api.Models;
using System.Diagnostics.CodeAnalysis;

namespace PublicApi.Api
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

            app.MapGet<string>("/token", "GetToken", "Get a bearer token. For development use only.",
                async (TokenHandler handler) => await handler.GetToken());

            app.MapPost<CreateJobResponse>("/job", "CreateJob", "Create a new job.",
                async (JobHandler handler,
                       [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey,
                       [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] CreateJobRequest request,
                       IValidator<CreateJobRequest> validator)
                    => await handler.CreateJobAsync(idempotencyKey, request, validator))
                .RequireAuthorization("PublicApiScope");

            app.MapGet<GetJobStatusResponse>("/job/{jobId}", "GetJobStatus", "Get the status of an existing job.",
                async (JobHandler handler,
                       Guid jobId)
                    => await handler.GetJobStatusAsync(jobId))
                .RequireAuthorization("PublicApiScope");
        }
    }
}
