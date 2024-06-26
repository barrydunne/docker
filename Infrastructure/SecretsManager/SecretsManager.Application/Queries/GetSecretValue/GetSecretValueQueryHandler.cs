﻿using AspNet.KickStarter.CQRS.Abstractions.Queries;
using AspNet.KickStarter.FunctionalResult;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Queries.GetSecretValue;

/// <summary>
/// Handler for the GetSecretValueQuery query.
/// </summary>
internal class GetSecretValueQueryHandler : IQueryHandler<GetSecretValueQuery, string?>
{
    private readonly IRedisDatabase _redis;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSecretValueQueryHandler"/> class.
    /// </summary>
    /// <param name="redis">The redis database that holds the secrets.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetSecretValueQueryHandler(IRedisDatabase redis, ILogger<GetSecretValueQueryHandler> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<string?>> Handle(GetSecretValueQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetSecretValueQuery handler. Vault: {Vault}, Secret: {Secret}", query.Vault, query.Secret);
            var secrets = await _redis.GetAsync<Dictionary<string, string>>(query.Vault.ToSecretVaultName()) ?? new();
            if (secrets.TryGetValue(query.Secret, out var secret))
                return secret;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to find secret value. Vault: {Vault}, Secret: {Secret}", query.Vault, query.Secret);
        }
        return (string?)null;
    }
}
