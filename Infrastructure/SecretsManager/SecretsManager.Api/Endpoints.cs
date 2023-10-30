using AspNet.KickStarter;
using AspNet.KickStarter.HttpHandlers;
using SecretsManager.Api.HttpHandlers;
using System.Diagnostics.CodeAnalysis;

namespace SecretsManager.Api
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
        internal static void Map(WebApplication app)
        {
            app.MapGet<string>("/health/status", "GetHealthStatus", "Check API health",
                (HealthHandler handler) => handler.GetStatus());

            app.MapGet<string>("/health/version", "GetVersion", "Get the API version",
                async (HealthHandler handler) => await handler.GetVersionAsync());

            app.MapGet<string[]>("/secrets/vaults", "GetVaults", "Get available secret vaults",
                async (SecretsHandler handler) => await handler.GetVaultsAsync());

            app.MapGet<Dictionary<string, string>>("/secrets/vaults/{vault}", "GetSecrets", "Get secrets from a vault",
                async (SecretsHandler handler, string vault) => await handler.GetSecretsAsync(vault));

            app.MapGet<string?>("/secrets/vaults/{vault}/{secret}", "GetSecretValue", "Get secret value from a vault",
                async (SecretsHandler handler, string vault, string secret) => await handler.GetSecretValueAsync(vault, secret));

            // Using HttpContext and not [FromBody] to allow simple requests without Content-Type: application/json
            app.MapPost("/secrets/vaults/{vault}/{secret}", "SetSecretValue", "Set secret value in a vault",
                async (SecretsHandler handler, HttpContext httpContext, string vault, string secret) => await handler.SetSecretValueAsync(httpContext.Request, vault, secret));
        }
    }
}
