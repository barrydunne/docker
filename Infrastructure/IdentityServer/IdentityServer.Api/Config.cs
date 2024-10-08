﻿using Duende.IdentityServer.Models;

namespace IdentityServer.Api;

/// <summary>
/// Provides simple configuration for scopes and clients in the IdentityServer.
/// </summary>
internal static class Config
{
    /// <summary>
    /// Gets the available scopes for the IdentityServer.
    /// </summary>
    internal static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new("publicapi", "Public API")
        };

    /// <summary>
    /// Gets the known clients for the IdentityServer.
    /// </summary>
    internal static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "publicapi" }
            }
        };
}
