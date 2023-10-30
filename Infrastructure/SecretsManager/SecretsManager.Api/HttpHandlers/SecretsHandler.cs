﻿using MediatR;
using SecretsManager.Logic.Commands;
using SecretsManager.Logic.Queries;
using System.Net.Mime;
using System.Text;

namespace SecretsManager.Api.HttpHandlers
{
    /// <summary>
    /// The handler for requests relating to secrets.
    /// </summary>
    public class SecretsHandler
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretsHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        public SecretsHandler(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Get the names of all secret vaults.
        /// </summary>
        /// <returns>The names of all secret vaults.</returns>
        internal async Task<IResult> GetVaultsAsync()
        {
            var vaults = await _mediator.Send(new GetSecretVaultsQuery());
            return Results.Ok(vaults);
        }

        /// <summary>
        /// Get all secrets from a vault.
        /// </summary>
        /// <param name="vault">The vault name.</param>
        /// <returns>The secrets contained in the specified vault, or an empty Dictionary if not found.</returns>
        internal async Task<IResult> GetSecretsAsync(string vault)
        {
            var secrets = await _mediator.Send(new GetSecretsQuery(vault));
            return Results.Ok(secrets);
        }

        /// <summary>
        /// Get a single secret value from a vault.
        /// </summary>
        /// <param name="vault">The vault name.</param>
        /// <param name="secret">The name of the secret to get the value for.</param>
        /// <returns>The value of the secret from the specified vault, or null if the vault or secret are not found.</returns>
        internal async Task<IResult> GetSecretValueAsync(string vault, string secret)
        {
            var secrets = await _mediator.Send(new GetSecretValueQuery(vault, secret));
            return Results.Ok(secrets);
        }

        /// <summary>
        /// Set a single secret value in a vault.
        /// </summary>
        /// <param name="request">The request containing the value.</param>
        /// <param name="vault">The vault name.</param>
        /// <param name="secret">The secret name.</param>
        /// <returns>OK or Problem.</returns>
        internal async Task<IResult> SetSecretValueAsync(HttpRequest request, string vault, string secret)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var value = await reader.ReadToEndAsync();
            if (request.ContentType == MediaTypeNames.Application.Json)
                value = value.Trim('"');
            var result = await _mediator.Send(new SetSecretValueCommand(vault, secret, value));
            return result.IsSuccess ? Results.Ok() : Results.Problem(result.Error);
        }
    }
}
