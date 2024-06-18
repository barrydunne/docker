using AspNet.KickStarter.FunctionalResult.Extensions;
using MediatR;
using SecretsManager.Application.Commands.SetSecretValue;
using SecretsManager.Application.Queries.GetSecrets;
using SecretsManager.Application.Queries.GetSecretValue;
using SecretsManager.Application.Queries.GetSecretVaults;
using System.Net.Mime;
using System.Text;

namespace SecretsManager.Api.HttpHandlers;

/// <summary>
/// The handler for requests relating to secrets.
/// </summary>
public class SecretsHandler
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretsHandler"/> class.
    /// </summary>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    public SecretsHandler(ISender mediator) => _mediator = mediator;

    /// <summary>
    /// Get the names of all secret vaults.
    /// </summary>
    /// <returns>The names of all secret vaults.</returns>
    internal async Task<IResult> GetVaultsAsync()
    {
        var result = await _mediator.Send(new GetSecretVaultsQuery());
        return result.Match(
            success => Results.Ok(result.Value),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Get all secrets from a vault.
    /// </summary>
    /// <param name="vault">The vault name.</param>
    /// <returns>The secrets contained in the specified vault, or an empty Dictionary if not found.</returns>
    internal async Task<IResult> GetSecretsAsync(string vault)
    {
        var result = await _mediator.Send(new GetSecretsQuery(vault));
        return result.Match(
            success => Results.Ok(result.Value),
            error => error.AsHttpResult());
    }

    /// <summary>
    /// Get a single secret value from a vault.
    /// </summary>
    /// <param name="vault">The vault name.</param>
    /// <param name="secret">The name of the secret to get the value for.</param>
    /// <returns>The value of the secret from the specified vault, or null if the vault or secret are not found.</returns>
    internal async Task<IResult> GetSecretValueAsync(string vault, string secret)
    {
        var result = await _mediator.Send(new GetSecretValueQuery(vault, secret));
        return result.Match(
            success => Results.Ok(result.Value),
            error => error.AsHttpResult());
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
        return result.Match(
            () => Results.Ok(),
            error => error.AsHttpResult());
    }
}
